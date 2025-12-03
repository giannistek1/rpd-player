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

// TODO: Rename to EditPlaylistView
public partial class CurrentPlaylistView : ContentView
{
    public event EventHandler? PlaySongPart;

    public event EventHandler? BackToPlaylists;


    internal Slider ProgressSlider;
    internal TimeSpan Progress;
    internal MainPage? ParentPage { get; set; }

    internal readonly CurrentPlaylistViewModel _viewModel = new();

    private bool _isInitialized = false;

    public CurrentPlaylistView()
    {
        InitializeComponent();
        BindingContext = _viewModel;
        ProgressSlider = PlaylistProgressSlider;
        CurrentPlaylistListView.DragDropController!.UpdateSource = true;

        // TODO: Via command in viewModel.
        BackImageButton.Clicked += BackImageButtonClicked;
        EditPlaylistnameImageButton.Clicked += EditPlaylistnameImageButtonClicked;
        ClearPlaylistImageButton.Clicked += ClearPlaylistImageButtonClicked;
        ShufflePlaylistImageButton.Clicked += ShufflePlaylistImageButtonClicked;
        MixedShufflePlaylistImageButton.Clicked += MixedShufflePlaylistImageButtonClicked;
        ImportPlaylistImageButton.Clicked += ImportPlaylistImageButtonClicked;
    }

    internal void InitializeView()
    {
        // Refresh UI colors?

        if (_isInitialized) { return; }
        CurrentPlaylistManager.Instance.ChosenPlaylist!.Segments.CollectionChanged += CollectionChanged;

        _isInitialized = true;
    }

    private void CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        CurrentPlaylistManager.Instance.RecalculatePlaylistTimingsAndIndices(ref CurrentPlaylistManager.Instance.ChosenPlaylist!.Segments);
    }

    internal async void BackImageButtonClicked(object? sender, EventArgs e)
    {
        // To hide soft keyboard programmatically.
        //PlaylistNameEntry.IsEnabled = false;
        //PlaylistNameEntry.IsEnabled = true;

        Playlist playlist = CurrentPlaylistManager.Instance.ChosenPlaylist!;

        if (playlist!.IsCloudPlaylist && playlist.Owner.Equals(AppState.Username))
        {
            await PlaylistRepository.SaveCloudPlaylistAsync(id: playlist.Id,
                                                        creationDate: playlist.CreationDate,
                                                        name: playlist.Name,
                                                        playlist.LengthInSeconds,
                                                        playlist.Count,
                                                        playlist.Segments.ToList(),
                                                        isPublic: playlist.IsPublic);
        }

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

            // TODO: Bindings (computable)
            LengthLabel.Text = String.Format("{0:hh\\:mm\\:ss}", playlist.Length);

            CountLabel.Text = $" of {playlist.Segments.Count}";

            _viewModel.BoygroupCount = playlist.Segments.AsEnumerable().Count(s => s.Artist?.GroupType == GroupType.BG);
            _viewModel.GirlgroupCount = playlist.Segments.AsEnumerable().Count(s => s.Artist?.GroupType == GroupType.GG);
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
            CurrentPlaylistManager.Instance.ChosenPlaylist = null;
        }
    }

    private async void EditPlaylistnameImageButtonClicked(object? sender, EventArgs e)
    {
        Playlist playlist = CurrentPlaylistManager.Instance.ChosenPlaylist!;
        InputPromptResult result = await General.ShowInputPromptAsync("Playlist name: ", playlist.Name);
        if (result.IsCanceled || string.IsNullOrWhiteSpace(result.Text)) { return; }

        playlist.Name = result.Text;
        ParentPage!.Title = playlist.Name;
    }

    private void ShufflePlaylistImageButtonClicked(object? sender, EventArgs e)
    {
        CurrentPlaylistManager.Instance.ChosenPlaylist!.Segments = General.RandomizePlaylist([.. CurrentPlaylistManager.Instance.ChosenPlaylist.Segments]).ToObservableCollection();
        CurrentPlaylistManager.Instance.RecalculatePlaylistTimingsAndIndices(ref CurrentPlaylistManager.Instance.ChosenPlaylist!.Segments);
        CurrentPlaylistListView.ItemsSource = CurrentPlaylistManager.Instance.ChosenPlaylist.Segments;
    }

    private void MixedShufflePlaylistImageButtonClicked(object? sender, EventArgs e)
    {
        CurrentPlaylistManager.Instance.ChosenPlaylist!.Segments = General.RandomizeAndAlternatePlaylist([.. CurrentPlaylistManager.Instance.ChosenPlaylist.Segments]).ToObservableCollection();
        CurrentPlaylistManager.Instance.RecalculatePlaylistTimingsAndIndices(ref CurrentPlaylistManager.Instance.ChosenPlaylist!.Segments);
        CurrentPlaylistListView.ItemsSource = CurrentPlaylistManager.Instance.ChosenPlaylist.Segments;
    }

    private async void ImportPlaylistImageButtonClicked(object? sender, EventArgs e)
    {
        Playlist playlist = CurrentPlaylistManager.Instance.ChosenPlaylist!;
        InputPromptResult result = await General.ShowInputAreaPromptAsync("Paste your song list (max 15000 char):", $"Timestamps get filtered out.{Environment.NewLine}Valid examples below:{Environment.NewLine}{Environment.NewLine}ATEEZ - Answer{Environment.NewLine}Fancy - TWICE{Environment.NewLine}01:15:34 ATEEZ - Ice on my teeth", maxLength: 15000);

        if (result.IsCanceled || string.IsNullOrWhiteSpace(result.Text)) { return; }

        // Clean input (of weird invisible characters)
        string cleanInput = result.Text
            .Replace("\u200B", "")  // zero-width space
            .Replace("\u200C", "")  // zero-width non-joiner
            .Replace("\u200D", "")  // zero-width joiner
            .Replace("\uFEFF", "")  // zero-width no-break space
            .Replace("\u00A0", " "); // non-breaking space

        string[] lines = cleanInput.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        // Regex to strip timestamps like "0:32", "1:05:10", "01:15:34", etc.
        Regex timestampRegex = new(@"^[\s\u00A0\u200B]*\d{1,2}:\d{2}(?::\d{2})?[\s\u00A0\u200B]*", RegexOptions.Compiled);

        List<SongPart> foundSongs = [];
        List<ImportResult> notFound = [];

        List<SongPart> allSongs = AppState.SongParts;

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

            // Detect order from the first matchable line.
            if (artistFirst is null)
            {
                bool firstLooksLikeArtist = allSongs.Any(s => s.ArtistName.Equals(first, StringComparison.OrdinalIgnoreCase));
                bool secondLooksLikeArtist = allSongs.Any(s => s.ArtistName.Equals(second, StringComparison.OrdinalIgnoreCase));

                //DebugService.Instance.Debug($"1: {first} 2:{second}, {firstLooksLikeArtist}, {secondLooksLikeArtist}");

                if (firstLooksLikeArtist && !secondLooksLikeArtist)
                    artistFirst = true;
                else if (!firstLooksLikeArtist && secondLooksLikeArtist)
                    artistFirst = false;
                else
                    artistFirst = true; // fallback

                DebugService.Instance.Info($"artistFirst: {artistFirst}");
            }

            string artist = artistFirst.Value ? first : second;
            string title = artistFirst.Value ? second : first;

            // Normalize user input
            string normInputArtist = Normalize(artist);
            string normInputTitle = Normalize(title);

            // Get all possible matches (don't search too wide, like with regular search)
            var possibleMatches = allSongs
                .Where(s =>
                {
                    string normArtist = Normalize(s.ArtistName);
                    string normTitle = Normalize(s.Title);

                    var altNames = s.Artist.AltNames
                        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .Select(Normalize)
                        .ToList() ?? new List<string>();

                    bool artistMatch =
                        normArtist.Equals(normInputArtist, StringComparison.OrdinalIgnoreCase) ||
                        normArtist.StartsWith(normInputArtist, StringComparison.OrdinalIgnoreCase) ||
                        altNames.Any(a =>
                            a.Equals(normInputArtist, StringComparison.OrdinalIgnoreCase) ||
                            a.StartsWith(normInputArtist, StringComparison.OrdinalIgnoreCase));

                    bool titleMatch = normTitle.Equals(normInputTitle, StringComparison.OrdinalIgnoreCase) ||
                                      normTitle.StartsWith(normInputTitle, StringComparison.OrdinalIgnoreCase) ||
                                      normTitle.EndsWith(normInputTitle, StringComparison.OrdinalIgnoreCase);

                    return artistMatch && titleMatch;
                })
                .Select(s =>
                {
                    string normTitle = Normalize(s.Title);
                    // Higher score = better match
                    int score = 0;

                    // Prefer exact matches
                    if (normTitle.Equals(normInputTitle, StringComparison.OrdinalIgnoreCase)) score += 100;
                    // Partial matches give smaller scores
                    else if (normTitle.Contains(normInputTitle, StringComparison.OrdinalIgnoreCase)) score += 50;
                    // Closer (longer overlap) titles are favored
                    score += Math.Min(normTitle.Length, normInputTitle.Length);

                    return new { Song = s, Score = score };
                })
                .OrderByDescending(x => x.Score)
                .ToList();

            SongPart? match = possibleMatches.FirstOrDefault()?.Song;

            if (match != null)
                foundSongs.Add(match);
            else
                notFound.Add(new(artist: artist, title: title));
        }

        // Show results
        if (notFound.Count > 0)
        {
            bool accept = await ParentPage!.DisplayAlert($"{notFound.Count} songs not found", $"Add the {foundSongs.Count} found songs to {playlist.Name}?", "Yes", "No");
            if (accept)
            {
                PlaylistsManager.TryAddSegmentToPlaylist(playlist, foundSongs);
            }
            ImportSegmentResultsPopup popup = new ImportSegmentResultsPopup(new(notFound)); // Gets disposed on close.
            await Application.Current!.MainPage!.ShowPopupAsync(popup);
        }
        else
        {
            int count = PlaylistsManager.TryAddSegmentToPlaylist(playlist, foundSongs);
            General.ShowToast($"{count} songs successfully added to playlist \"{playlist.Name}\"!");
        }
        CurrentPlaylistManager.Instance.RecalculatePlaylistTimingsAndIndices(ref playlist.Segments);
        RefreshCurrentPlaylist();
    }

    // Tests: "Left & Right" "Left And Right" "Left-and-Right" "Left & Right!" "LEFT AND RIGHT" = leftandright
    /// <summary> Removes all punctuation and whitespace from a string. </summary>
    /// <param name="input"></param>
    /// <returns>string without punctuation and whitespace.</returns>
    static string Normalize(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        // Convert & and similar symbols to "and"
        string normalized = input.Replace("&", "and", StringComparison.OrdinalIgnoreCase);

        // Replace punctuation (like -, _, :, /, etc.)
        normalized = Regex.Replace(normalized, @"[-_:/\\(){}\[\]’'\"".,!?*]+", "");

        // Normalize "and" variants (e.g., double spaces, weird spacing)
        normalized = Regex.Replace(normalized, @"\band\b", "and", RegexOptions.IgnoreCase);

        // Remove all spaces and convert to lower case.
        return Regex.Replace(normalized, @"\s+", "").Trim().ToLowerInvariant();
    }

    #endregion Playlist

    #region Songparts

    private void CurrentPlaylistListView_ItemTapped(object sender, Syncfusion.Maui.ListView.ItemTappedEventArgs e)
    {
        if (!General.HasInternetConnection()) { return; }

        SongPart songPart = (SongPart)e.DataItem;

        if (!string.IsNullOrWhiteSpace(songPart.AudioUrl))
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
        if (!string.IsNullOrWhiteSpace(songPart.AudioUrl))
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