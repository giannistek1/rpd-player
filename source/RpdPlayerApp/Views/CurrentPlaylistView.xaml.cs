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

    private void BackButtonClicked(object sender, EventArgs e)
    {
        // To hide soft keyboard programmatically.
        PlaylistNameEntry.IsEnabled = false;
        PlaylistNameEntry.IsEnabled = true;
        BackToPlaylists!.Invoke(sender, e);
    }

    #region Playlist

    internal void PlayPlaylistButtonClicked(object? sender, EventArgs e)
    {
        if (!CurrentPlaylistManager.Instance.ChosenPlaylist.SongParts.Any()) { return; }

        // Change mode to playlist
        AppState.PlayMode = PlayModeValue.Playlist;

        CurrentPlaylistManager.Instance.CurrentlyPlayingPlaylist = CurrentPlaylistManager.Instance.ChosenPlaylist;
        AudioManager.ChangeAndStartSong(CurrentPlaylistManager.Instance.ChosenPlaylist.SongParts[0]);

        //PlaySongPart!.Invoke(sender, e);
    }

    internal async void SavePlaylistButtonClicked(object? sender, EventArgs e)
    {
        try
        {
            StringBuilder sb = new();

            var playlist = CurrentPlaylistManager.Instance.ChosenPlaylist;

            // Header should contain: Creation date, last modified date, user, count, length
            sb.AppendLine($"HDR:[{playlist.CreationDate}][{playlist.LastModifiedDate}][{AppState.Username}][{playlist.Count}][{playlist.Length}][{playlist.CountdownMode}]");

            foreach (SongPart songPart in playlist.SongParts)
            {
                sb.AppendLine($"{{{songPart.ArtistName}}}{{{songPart.AlbumTitle}}}{{{songPart.Title}}}{{{songPart.PartNameShort}}}{{{songPart.PartNameNumber}}}{{{songPart.ClipLength}}}{{{songPart.AudioURL}}}");
            }

            await FileManager.SavePlaylistStringToTextFileAsync($"{PlaylistNameEntry.Text}", sb.ToString());

            General.ShowToast($"{PlaylistNameEntry.Text} saved locally!");
        }
        catch (Exception ex)
        {
            General.ShowToast(ex.Message);
        }

        // TODO: Is dit logisch hier?
        if (AppState.UsingCloudMode && General.HasInternetConnection())
        {
            try
            {
                DropboxRepository.SavePlaylist(PlaylistNameEntry.Text);
                General.ShowToast($"{PlaylistNameEntry.Text} saved locally and online!");
            }
            catch (Exception ex)
            {
                General.ShowToast(ex.Message);
            }
        }
    }

    internal void ToggleCloudModePressed(object? sender, EventArgs e)
    {
        AppState.UsingCloudMode = !AppState.UsingCloudMode;

        string toastText = AppState.UsingCloudMode ? "Playlist will save online" : "Playlist will save locally";
        General.ShowToast(toastText);

        ParentPage?.SetupLibraryOrCurrentPlaylistToolbar();
    }

    public void RefreshCurrentPlaylist()
    {
        PlaylistNameEntry.Text = CurrentPlaylistManager.Instance.ChosenPlaylist.Name;

        if (CurrentPlaylistManager.Instance.ChosenPlaylist.SongParts is not null)
        {
            CurrentPlaylistListView.ItemsSource = CurrentPlaylistManager.Instance.ChosenPlaylist.SongParts;

            CurrentPlaylistManager.Instance.ChosenPlaylist.SetLength();
            LengthLabel.Text = String.Format("{0:hh\\:mm\\:ss}", CurrentPlaylistManager.Instance.ChosenPlaylist.Length);
            CountLabel.Text = $"Tot: {CurrentPlaylistManager.Instance.ChosenPlaylist.SongParts.Count}";
            BoygroupCountLabel.Text = $"BG: {CurrentPlaylistManager.Instance.ChosenPlaylist.SongParts.AsEnumerable().Count(s => s.Artist?.GroupType == GroupType.BG)}";
            GirlgroupCountLabel.Text = $"GG: {CurrentPlaylistManager.Instance.ChosenPlaylist.SongParts.AsEnumerable().Count(s => s.Artist?.GroupType == GroupType.GG)}";
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