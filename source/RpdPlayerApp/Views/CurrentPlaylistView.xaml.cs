using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core.Extensions;
using RpdPlayerApp.Architecture;
using RpdPlayerApp.Enums;
using RpdPlayerApp.Managers;
using RpdPlayerApp.Models;
using RpdPlayerApp.Repositories;
using System.Text;

namespace RpdPlayerApp.Views;

public partial class CurrentPlaylistView : ContentView
{
    public event EventHandler? PlaySongPart;

    public event EventHandler? BackToPlaylists;

    internal MainPage? ParentPage { get; set; }

    PlaylistRepository _playlistRepository = new();

    public CurrentPlaylistView()
    {
        InitializeComponent();
        CurrentPlaylistListView.DragDropController!.UpdateSource = true;
    }

    internal void InitializeView()
    {
        BackButtonImageButton.BackgroundColor = (Color)Application.Current!.Resources["BackgroundColor"];
        BackButtonImageButton.Source = IconManager.BackIcon;
    }

    private async void BackButtonClicked(object sender, EventArgs e)
    {
        // To hide soft keyboard programmatically.
        PlaylistNameEntry.IsEnabled = false;
        PlaylistNameEntry.IsEnabled = true;

        // Reload cache.
        Playlist playlist = CurrentPlaylistManager.Instance.ChosenPlaylist;

        if (playlist!.IsCloudPlaylist && playlist.Owner.Equals(AppState.Username))
        {
            CacheState.CloudPlaylists = null;
            await _playlistRepository.SaveCloudPlaylist(id: playlist.Id,
                                                        creationDate: playlist.CreationDate,
                                                        name: playlist.Name,
                                                        playlist.LengthInSeconds,
                                                        playlist.Count,
                                                        playlist.SongParts.ToList(),
                                                        isPublic: playlist.IsPublic);
        }

        CacheState.LocalPlaylists = null;
        await PlaylistsManager.SavePlaylistLocally(playlist, PlaylistNameEntry.Text);

        BackToPlaylists!.Invoke(sender, e);
    }

    #region Playlist

    internal void PlayPlaylistButtonClicked(object? sender, EventArgs e)
    {
        if (!CurrentPlaylistManager.Instance.ChosenPlaylist!.SongParts.Any()) { return; }

        // Change mode to playlist
        AppState.PlayMode = PlayModeValue.Playlist;

        CurrentPlaylistManager.Instance.CurrentlyPlayingPlaylist = CurrentPlaylistManager.Instance.ChosenPlaylist;
        AudioManager.ChangeAndStartSong(CurrentPlaylistManager.Instance.ChosenPlaylist.SongParts[0]);

        //PlaySongPart!.Invoke(sender, e);
    }

    internal async void SavePlaylistButtonClicked(object? sender, EventArgs e) => await PlaylistsManager.SavePlaylistLocally(CurrentPlaylistManager.Instance.ChosenPlaylist!, PlaylistNameEntry.Text);


    internal void ToggleCloudModePressed(object? sender, EventArgs e)
    {
        AppState.UsingCloudMode = !AppState.UsingCloudMode;

        string toastText = AppState.UsingCloudMode ? "Playlist will save online" : "Playlist will save locally";
        General.ShowToast(toastText);

        ParentPage?.SetupLibraryOrCurrentPlaylistToolbar();
    }

    public void RefreshCurrentPlaylist()
    {
        var playlist = CurrentPlaylistManager.Instance.ChosenPlaylist;

        PlaylistNameEntry.Text = playlist!.Name;

        if (playlist.SongParts is not null)
        {
            CurrentPlaylistListView.ItemsSource = playlist.SongParts;

            playlist.SetLength();
            LengthLabel.Text = String.Format("{0:hh\\:mm\\:ss}", playlist.Length);
            CountLabel.Text = $"Tot: {playlist.SongParts.Count}";
            BoygroupCountLabel.Text = $"BG: {playlist.SongParts.AsEnumerable().Count(s => s.Artist?.GroupType == GroupType.BG)}";
            GirlgroupCountLabel.Text = $"GG: {playlist.SongParts.AsEnumerable().Count(s => s.Artist?.GroupType == GroupType.GG)}";
        }
    }

    internal async void ClearPlaylistButtonClicked(object? sender, EventArgs e)
    {
        bool accept = await ParentPage!.DisplayAlert("Confirmation", $"Clear {CurrentPlaylistManager.Instance.ChosenPlaylist.Name}?", "Yes", "No");
        if (accept)
        {
            CurrentPlaylistManager.Instance.ClearCurrentPlaylist();
            RefreshCurrentPlaylist();
        }
    }

    /// <summary> Sets ChosenPlaylist to null. </summary>
    public void ResetCurrentPlaylist()
    {
        if (CurrentPlaylistManager.Instance.ChosenPlaylist.SongParts is not null)
        {
            CurrentPlaylistListView.ItemsSource = null;

            PlaylistNameEntry.Text = string.Empty;
        }
    }

    private void ShufflePlaylistButtonImageButton_Clicked(object sender, EventArgs e)
    {
        CurrentPlaylistManager.Instance.ChosenPlaylist.SongParts = General.RandomizePlaylist([.. CurrentPlaylistManager.Instance.ChosenPlaylist.SongParts]).ToObservableCollection();
        CurrentPlaylistListView.ItemsSource = CurrentPlaylistManager.Instance.ChosenPlaylist.SongParts;
    }

    private void MixedShufflePlaylistButtonImageButton_Clicked(object sender, EventArgs e)
    {
        CurrentPlaylistManager.Instance.ChosenPlaylist.SongParts = General.RandomizeAndAlternatePlaylist([.. CurrentPlaylistManager.Instance.ChosenPlaylist.SongParts]).ToObservableCollection();
        CurrentPlaylistListView.ItemsSource = CurrentPlaylistManager.Instance.ChosenPlaylist.SongParts;
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

            CurrentPlaylistManager.Instance.CurrentlyPlayingPlaylist = CurrentPlaylistManager.Instance.ChosenPlaylist;
            AudioManager.ChangeAndStartSong(songPart);

            //PlaySongPart?.Invoke(sender, e);
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
                RefreshCurrentPlaylist();
            }
        }
    }

    #endregion Songparts
}