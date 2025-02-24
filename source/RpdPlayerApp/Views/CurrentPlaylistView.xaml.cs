using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core.Extensions;
using RpdPlayerApp.Architecture;
using RpdPlayerApp.Enums;
using RpdPlayerApp.Managers;
using RpdPlayerApp.Models;
using RpdPlayerApp.Repositories;
using RpdPlayerApp.ViewModels;
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
        MainViewModel.PlayMode = PlayMode.Playlist;

        // Clear playlist queue and fill playlist queue
        MainViewModel.PlaylistQueue.Clear();

        List<SongPart> songParts = [.. CurrentPlaylistManager.Instance.CurrentPlaylist.SongParts];
        foreach (var songPart in songParts)
        {
            MainViewModel.PlaylistQueue.Enqueue(songPart);
        }

        MainViewModel.CurrentSongPart = MainViewModel.PlaylistQueue.Dequeue();

        PlaySongPart!.Invoke(sender, e);
    }

    internal async void SavePlaylistButtonClicked(object? sender, EventArgs e)
    {
        try
        {
            // Create file on system
            StringBuilder sb = new();
            foreach (SongPart songPart in CurrentPlaylistManager.Instance.CurrentPlaylist.SongParts)
            {
                sb.AppendLine($"{{{songPart.ArtistName}}}{{{songPart.AlbumTitle}}}{{{songPart.Title}}}{{{songPart.PartNameShort}}}{{{songPart.PartNameNumber}}}{{{songPart.ClipLength}}}{{{songPart.AudioURL}}}");
            }

            await FileManager.SavePlaylistJsonToFileAsync($"{PlaylistNameEntry.Text}.txt", sb.ToString());

            HelperClass.ShowToast($"{PlaylistNameEntry.Text} saved locally!");
        }
        catch (Exception ex)
        {
            HelperClass.ShowToast(ex.Message);
        }

        // TODO: Is dit logisch hier?
        if (MainViewModel.UsingCloudMode && HelperClass.HasInternetConnection())
        {
            try
            {
                DropboxRepository.SavePlaylist(PlaylistNameEntry.Text);
                HelperClass.ShowToast($"{PlaylistNameEntry.Text} saved locally and online!");
            }
            catch (Exception ex)
            {
                HelperClass.ShowToast(ex.Message);
            }
        }
    }

    internal void ToggleCloudModePressed(object? sender, EventArgs e)
    {
        MainViewModel.UsingCloudMode = !MainViewModel.UsingCloudMode;

        string toastText = MainViewModel.UsingCloudMode ? "Playlist will save online" : "Playlist will save locally";
        HelperClass.ShowToast(toastText);

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

    internal void ClearPlaylistButtonClicked(object? sender, EventArgs e)
    {
        CurrentPlaylistManager.Instance.ClearCurrentPlaylist();
        RefreshCurrentPlaylist();
    }

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
        CurrentPlaylistManager.Instance.CurrentPlaylist.SongParts = HelperClass.RandomizePlaylist([.. CurrentPlaylistManager.Instance.CurrentPlaylist.SongParts]).ToObservableCollection();
        CurrentPlaylistListView.ItemsSource = CurrentPlaylistManager.Instance.CurrentPlaylist.SongParts;
    }

    private void MixedShufflePlaylistButtonImageButton_Clicked(object sender, EventArgs e)
    {
        CurrentPlaylistManager.Instance.CurrentPlaylist.SongParts = HelperClass.RandomizeAndAlternatePlaylist([.. CurrentPlaylistManager.Instance.CurrentPlaylist.SongParts]).ToObservableCollection();
        CurrentPlaylistListView.ItemsSource = CurrentPlaylistManager.Instance.CurrentPlaylist.SongParts;
    }

    #endregion Playlist

    #region Songparts

    private void CurrentPlaylistListView_ItemTapped(object sender, Syncfusion.Maui.ListView.ItemTappedEventArgs e)
    {
        if (!HelperClass.HasInternetConnection()) { return; }

        SongPart songPart = (SongPart)e.DataItem;

        if (!string.IsNullOrWhiteSpace(songPart.AudioURL))
        {
            MainViewModel.PlayMode = PlayMode.Playlist;

            MainViewModel.PlaylistQueue.Clear();

            List<SongPart> songParts = [.. CurrentPlaylistManager.Instance.CurrentPlaylist.SongParts];
            int index = songParts.FindIndex(s => s.Id == songPart.Id);

            while (index < songParts.Count)
            {
                MainViewModel.PlaylistQueue.Enqueue(songParts[index]);
                index++;
            }

            MainViewModel.CurrentSongPart = MainViewModel.PlaylistQueue.Dequeue();

            PlaySongPart?.Invoke(sender, e);
        }

        CurrentPlaylistListView.SelectedItems?.Clear();
    }

    private void SwipeItemPlaySongPart(object sender, EventArgs e)
    {
        if (!HelperClass.HasInternetConnection()) { return; }

        SongPart songPart = (SongPart)((MenuItem)sender).CommandParameter;
        if (!string.IsNullOrWhiteSpace(songPart.AudioURL))
        {
            MainViewModel.CurrentSongPart = songPart;
            PlaySongPart!.Invoke(sender, e);
        }
    }

    // Remove songpart on swipe
    private void CurrentPlaylistListViewSwipeEnded(object sender, Syncfusion.Maui.ListView.SwipeEndedEventArgs e)
    {
        if (e.Direction == SwipeDirection.Right && e.Offset > 30)
        {
            SongPart songPart = (SongPart)e.DataItem!;
            CurrentPlaylistManager.Instance.RemoveSongpartOfCurrentPlaylist(songPart);

            RefreshCurrentPlaylist();
        }
    }

    #endregion Songparts
}