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
        if (!CurrentPlaylistManager.Instance.CurrentPlaylist.SongParts.Any()) { return; }

        // Change mode to playlist
        AppState.PlayMode = PlayModeValue.Playlist;

        // Clear playlist queue and fill playlist queue
        AppState.PlaylistQueue.Clear();

        List<SongPart> songParts = [.. CurrentPlaylistManager.Instance.CurrentPlaylist.SongParts];
        foreach (var songPart in songParts)
        {
            AppState.PlaylistQueue.Enqueue(songPart);
        }

        AppState.CurrentSongPart = AppState.PlaylistQueue.Dequeue();

        PlaySongPart!.Invoke(sender, e);
    }

    internal async void SavePlaylistButtonClicked(object? sender, EventArgs e)
    {
        try
        {
            StringBuilder sb = new();

            var playlist = CurrentPlaylistManager.Instance.CurrentPlaylist;

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
        PlaylistNameEntry.Text = CurrentPlaylistManager.Instance.CurrentPlaylist.Name;

        if (CurrentPlaylistManager.Instance.CurrentPlaylist.SongParts is not null)
        {
            CurrentPlaylistListView.ItemsSource = CurrentPlaylistManager.Instance.CurrentPlaylist.SongParts;

            CurrentPlaylistManager.Instance.CurrentPlaylist.SetLength();
            LengthLabel.Text = String.Format("{0:hh\\:mm\\:ss}", CurrentPlaylistManager.Instance.CurrentPlaylist.Length);
            CountLabel.Text = $"Tot: {CurrentPlaylistManager.Instance.CurrentPlaylist.SongParts.Count}";
            BoygroupCountLabel.Text = $"BG: {CurrentPlaylistManager.Instance.CurrentPlaylist.SongParts.AsEnumerable().Count(s => s.Artist?.GroupType == GroupType.BG)}";
            GirlgroupCountLabel.Text = $"GG: {CurrentPlaylistManager.Instance.CurrentPlaylist.SongParts.AsEnumerable().Count(s => s.Artist?.GroupType == GroupType.GG)}";
        }
    }

    internal async void ClearPlaylistButtonClicked(object? sender, EventArgs e)
    {
        bool accept = await ParentPage!.DisplayAlert("Confirmation", $"Clear {CurrentPlaylistManager.Instance.CurrentPlaylist.Name}?", "Yes", "No");
        if (accept)
        {
            CurrentPlaylistManager.Instance.ClearCurrentPlaylist();
            RefreshCurrentPlaylist();
        }
    }

    /// <summary> Sets CurrentPlaylist to null. </summary>
    public void ResetCurrentPlaylist()
    {
        if (CurrentPlaylistManager.Instance.CurrentPlaylist.SongParts is not null)
        {
            CurrentPlaylistListView.ItemsSource = null;

            PlaylistNameEntry.Text = string.Empty;
        }
    }

    private void ShufflePlaylistButtonImageButton_Clicked(object sender, EventArgs e)
    {
        CurrentPlaylistManager.Instance.CurrentPlaylist.SongParts = General.RandomizePlaylist([.. CurrentPlaylistManager.Instance.CurrentPlaylist.SongParts]).ToObservableCollection();
        CurrentPlaylistListView.ItemsSource = CurrentPlaylistManager.Instance.CurrentPlaylist.SongParts;
    }

    private void MixedShufflePlaylistButtonImageButton_Clicked(object sender, EventArgs e)
    {
        CurrentPlaylistManager.Instance.CurrentPlaylist.SongParts = General.RandomizeAndAlternatePlaylist([.. CurrentPlaylistManager.Instance.CurrentPlaylist.SongParts]).ToObservableCollection();
        CurrentPlaylistListView.ItemsSource = CurrentPlaylistManager.Instance.CurrentPlaylist.SongParts;
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

            AppState.PlaylistQueue.Clear();

            List<SongPart> songParts = [.. CurrentPlaylistManager.Instance.CurrentPlaylist.SongParts];
            int index = songParts.FindIndex(s => s.Id == songPart.Id);

            while (index < songParts.Count)
            {
                AppState.PlaylistQueue.Enqueue(songParts[index]);
                index++;
            }

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