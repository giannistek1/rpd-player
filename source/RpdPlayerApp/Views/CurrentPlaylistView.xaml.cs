using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Maui.Views;
using RpdPlayerApp.Architecture;
using RpdPlayerApp.Enums;
using RpdPlayerApp.Managers;
using RpdPlayerApp.Models;
using RpdPlayerApp.Repositories;
using RpdPlayerApp.Services;
using RpdPlayerApp.ViewModels;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace RpdPlayerApp.Views;

public partial class CurrentPlaylistView : ContentView
{
    public event EventHandler? PlaySongPart;

    public event EventHandler? BackToPlaylists;


    internal Slider ProgressSlider;
    internal TimeSpan Progress;
    internal MainPage? ParentPage { get; set; }

    internal readonly CurrentPlaylistViewModel _viewModel = new();

    public CurrentPlaylistView()
    {
        InitializeComponent();
        BindingContext = _viewModel;
        ProgressSlider = PlaylistProgressSlider;
        CurrentPlaylistListView.DragDropController!.UpdateSource = true;
    }

    internal void InitializeView()
    {
        BackImageButton.BackgroundColor = (Color)Application.Current!.Resources["BackgroundColor"];
        BackImageButton.Source = IconManager.BackIcon;

        CurrentPlaylistManager.Instance.ChosenPlaylist!.Segments.CollectionChanged += CollectionChanged;
    }

    private void CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        CurrentPlaylistManager.Instance.RecalculatePlaylistTimingsAndIndices(ref CurrentPlaylistManager.Instance.ChosenPlaylist!.Segments);
    }

    internal async void BackButtonClicked(object? sender, EventArgs e)
    {
        // To hide soft keyboard programmatically.
        //PlaylistNameEntry.IsEnabled = false;
        //PlaylistNameEntry.IsEnabled = true;

        Playlist playlist = CurrentPlaylistManager.Instance.ChosenPlaylist!;

        if (playlist!.IsCloudPlaylist && playlist.Owner.Equals(AppState.Username))
        {
            CacheState.CloudPlaylists = null;
            await PlaylistRepository.SaveCloudPlaylist(id: playlist.Id,
                                                        creationDate: playlist.CreationDate,
                                                        name: playlist.Name,
                                                        playlist.LengthInSeconds,
                                                        playlist.Count,
                                                        playlist.Segments.ToList(),
                                                        isPublic: playlist.IsPublic);
        }

        // Reload cache.
        CacheState.LocalPlaylists = null;
        await PlaylistsManager.SavePlaylistLocally(playlist, playlist.Name);

        CurrentPlaylistManager.Instance.ChosenPlaylist!.Segments.CollectionChanged -= CollectionChanged;

        BackToPlaylists!.Invoke(sender, e);
    }

    #region Playlist

    internal void PlayPlaylistButtonClicked(object? sender, EventArgs e)
    {
        CurrentPlaylistManager.PlayCurrentPlaylist();
        //PlaySongPart!.Invoke(sender, e);
    }

    internal async void SavePlaylistButtonClicked(object? sender, EventArgs e) => await PlaylistsManager.SavePlaylistLocally(CurrentPlaylistManager.Instance.ChosenPlaylist!, CurrentPlaylistManager.Instance.ChosenPlaylist!.Name);


    internal void ToggleCloudModePressed(object? sender, EventArgs e)
    {
        AppState.UsingCloudMode = !AppState.UsingCloudMode;

        string toastText = AppState.UsingCloudMode ? "Playlist will save online" : "Playlist will save locally";
        General.ShowToast(toastText);

        ParentPage?.SetupLibraryOrCurrentPlaylistToolbar();
    }

    // Refreshes UI values.
    // TODO: To ViewModel
    internal void RefreshCurrentPlaylist()
    {
        var playlist = CurrentPlaylistManager.Instance.ChosenPlaylist;

        if (playlist!.Segments is not null)
        {
            CurrentPlaylistListView.ItemsSource = playlist.Segments;

            LengthLabel.Text = String.Format("{0:hh\\:mm\\:ss}", playlist.Length);

            CountLabel.Text = $" of {playlist.Segments.Count}";

            BoygroupCountLabel.Text = $"BG: {playlist.Segments.AsEnumerable().Count(s => s.Artist?.GroupType == GroupType.BG)}";
            GirlgroupCountLabel.Text = $"GG: {playlist.Segments.AsEnumerable().Count(s => s.Artist?.GroupType == GroupType.GG)}";
        }
    }
    // TODO: To ViewModel
    internal void RefreshProgress()
    {
        if (CurrentPlaylistManager.Instance.ChosenPlaylist == CurrentPlaylistManager.Instance.CurrentlyPlayingPlaylist)
        {
            CurrentProgressLabel.Text = String.Format("{0:hh\\:mm\\:ss}", Progress);
        }
    }

    internal async void ClearPlaylistImageButtonClicked(object? sender, EventArgs e)
    {
        bool accept = await ParentPage!.DisplayAlert("Confirmation", $"Clear {CurrentPlaylistManager.Instance.ChosenPlaylist!.Name}?", "Yes", "No");
        if (accept)
        {
            CurrentPlaylistManager.Instance.ClearCurrentPlaylist();
            RefreshCurrentPlaylist();
        }
    }

    /// <summary> Sets ChosenPlaylist to null. </summary>
    public void ResetCurrentPlaylist()
    {
        if (CurrentPlaylistManager.Instance.ChosenPlaylist!.Segments is not null)
        {
            CurrentPlaylistListView.ItemsSource = null;
        }
    }

    private async void EditPlaylistnameImageButtonClicked(object sender, EventArgs e)
    {
        Playlist playlist = CurrentPlaylistManager.Instance.ChosenPlaylist!;
        InputPromptResult result = await General.ShowInputPrompt("Playlist name: ", playlist.Name);
        playlist.Name = result.Text;
        ParentPage!.Title = playlist.Name;
    }

    private void ShufflePlaylistImageButtonClicked(object sender, EventArgs e)
    {
        CurrentPlaylistManager.Instance.ChosenPlaylist!.Segments = General.RandomizePlaylist([.. CurrentPlaylistManager.Instance.ChosenPlaylist.Segments]).ToObservableCollection();
        CurrentPlaylistManager.Instance.RecalculatePlaylistTimingsAndIndices(ref CurrentPlaylistManager.Instance.ChosenPlaylist!.Segments);
        CurrentPlaylistListView.ItemsSource = CurrentPlaylistManager.Instance.ChosenPlaylist.Segments;
    }

    private void MixedShufflePlaylistImageButtonClicked(object sender, EventArgs e)
    {
        CurrentPlaylistManager.Instance.ChosenPlaylist!.Segments = General.RandomizeAndAlternatePlaylist([.. CurrentPlaylistManager.Instance.ChosenPlaylist.Segments]).ToObservableCollection();
        CurrentPlaylistManager.Instance.RecalculatePlaylistTimingsAndIndices(ref CurrentPlaylistManager.Instance.ChosenPlaylist!.Segments);
        CurrentPlaylistListView.ItemsSource = CurrentPlaylistManager.Instance.ChosenPlaylist.Segments;
    }

    private async void ImportPlaylistImageButtonClicked(object sender, EventArgs e)
    {
        Playlist playlist = CurrentPlaylistManager.Instance.ChosenPlaylist!;
        InputPromptResult result = await General.ShowInputAreaPrompt("Import songs: ", playlist.Name, maxLength: 15000);

        if (result.IsCanceled || string.IsNullOrWhiteSpace(result.Text)) { return; }

        // Split user input into lines
        string[] lines = result.Text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        List<SongPart> foundSongs = [];
        List<ImportResult> notFound = [];

        // Assuming you have a master list of songs
        List<SongPart> allSongs = AppState.SongParts;

        // Regex to strip timestamps like "0:32", "1:05:10", etc.
        Regex timestampRegex = new(@"^\s*\d{1,2}:\d{2}(?::\d{2})?\s*", RegexOptions.Compiled);

        bool? artistFirst = null; // Will detect whether the format is Artist-Title or Title-Artist

        foreach (string rawLine in lines)
        {
            string line = timestampRegex.Replace(rawLine, "").Trim();
            if (string.IsNullOrWhiteSpace(line))
                continue;

            string[] parts = line.Split(new[] { " - " }, 2, StringSplitOptions.TrimEntries);
            if (parts.Length < 2)
            {
                notFound.Add(new(artist: line, title: ""));
                continue;
            }

            string first = parts[0];
            string second = parts[1];

            // Detect order from the first matchable line
            if (artistFirst is null)
            {
                bool firstLooksLikeArtist = allSongs.Any(s => s.ArtistName.Equals(first, StringComparison.OrdinalIgnoreCase));
                bool secondLooksLikeArtist = allSongs.Any(s => s.ArtistName.Equals(second, StringComparison.OrdinalIgnoreCase));

                if (firstLooksLikeArtist && !secondLooksLikeArtist)
                    artistFirst = true;
                else if (!firstLooksLikeArtist && secondLooksLikeArtist)
                    artistFirst = false;
                else
                    artistFirst = true; // fallback
            }

            string artist = artistFirst.Value ? first : second;
            string title = artistFirst.Value ? second : first;

            SongPart? match = allSongs.FirstOrDefault(s =>
            {
                string normArtist = Normalize(s.ArtistName);
                string normTitle = Normalize(s.Title);
                string normInputArtist = Normalize(artist);
                string normInputTitle = Normalize(title);

                bool artistMatch =
                    normArtist.StartsWith(normInputArtist, StringComparison.OrdinalIgnoreCase) ||
                    normInputArtist.StartsWith(normArtist, StringComparison.OrdinalIgnoreCase);

                bool titleMatch = normTitle.Contains(normInputTitle, StringComparison.OrdinalIgnoreCase);

                return artistMatch && titleMatch;
            });

            if (match != null)
                foundSongs.Add(match);
            else
                notFound.Add(new(artist: artist, title: title));
        }

        // Show results
        if (notFound.Count > 0)
        {
            bool accept = await ParentPage!.DisplayAlert("Songs not found", $"Do you still want to add the found songs to {playlist.Name}?", "Yes", "No");
            if (accept)
            {
                // TODO: Make/user helper method.
                foreach (SongPart segment in foundSongs)
                {
                    if (!playlist.Segments.Contains(segment))
                    {
                        playlist.Segments.Add(segment);
                    }
                }
            }
            var popup = new ImportSegmentResultsPopup(new(notFound)); // Gets disposed on close.
            Application.Current!.MainPage!.ShowPopup(popup);
        }
        else
        {
            // TODO: Make/user helper method.
            foreach (SongPart segment in foundSongs)
            {
                if (!playlist.Segments.Contains(segment))
                {
                    playlist.Segments.Add(segment);
                }
            }
            General.ShowToast($"{foundSongs.Count} songs successfully added to playlist \"{playlist.Name}\"!");
        }
    }
    static string Normalize(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        // Replace punctuation (like -, _, :, /, etc.)
        string replaced = Regex.Replace(input, @"[-_:/\\(){}\[\]’'\"".,!?]+", "");

        // Remove all spaces and convert to lower case.
        return Regex.Replace(replaced, @"\s+", "").Trim().ToLowerInvariant();
    }

    #endregion Playlist

    #region Songparts

    private void CurrentPlaylistListView_ItemTapped(object sender, Syncfusion.Maui.ListView.ItemTappedEventArgs e)
    {
        if (!General.HasInternetConnection()) { return; }

        SongPart songPart = (SongPart)e.DataItem;

        if (!string.IsNullOrWhiteSpace(songPart.AudioURL))
        {
            AppState.PlayMode = PlayModeValue.Playlist;

            CurrentPlaylistManager.Instance.CurrentlyPlayingPlaylist = CurrentPlaylistManager.Instance.ChosenPlaylist!;
            AudioManager.ChangeAndStartSong(songPart);

            PlaySongPart?.Invoke(sender, e);
        }

        CurrentPlaylistListView.SelectedItems?.Clear();
    }

    private void SwipeItemPlaySongPart(object sender, EventArgs e)
    {
        if (!General.HasInternetConnection()) { return; }

        SongPart songPart = (SongPart)((MenuItem)sender).CommandParameter;
        if (!string.IsNullOrWhiteSpace(songPart.AudioURL))
        {
            AppState.CurrentSongPart = songPart;
            PlaySongPart!.Invoke(sender, e);
        }
    }

    // Remove songpart on swipe
    private async void CurrentPlaylistListViewSwipeEnded(object sender, Syncfusion.Maui.ListView.SwipeEndedEventArgs e)
    {
        if (e.Direction == SwipeDirection.Right && e.Offset > 30)
        {
            SongPart songPart = (SongPart)e.DataItem!;

            bool accept = await ParentPage!.DisplayAlert("Confirmation", $"Delete {songPart.Title}?", "Yes", "No");
            if (accept)
            {
                CurrentPlaylistManager.Instance.RemoveSongpartOfCurrentPlaylist(songPart);
                CurrentPlaylistManager.Instance.RecalculatePlaylistTimingsAndIndices(ref CurrentPlaylistManager.Instance.ChosenPlaylist!.Segments);
                RefreshCurrentPlaylist();
            }
        }
    }

    #endregion Songparts
}