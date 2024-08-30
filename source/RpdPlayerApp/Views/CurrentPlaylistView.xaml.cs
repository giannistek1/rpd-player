using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core.Extensions;
using RpdPlayerApp.Architecture;
using RpdPlayerApp.Models;
using RpdPlayerApp.Repositories;
using RpdPlayerApp.ViewModel;
using System.Text;

namespace RpdPlayerApp.Views;

public partial class CurrentPlaylistView : ContentView
{
    public event EventHandler? PlaySongPart;
    public event EventHandler? BackToPlaylists;

    public CurrentPlaylistView()
    {
        InitializeComponent();

        CurrentPlaylistListView.DragDropController!.UpdateSource = true;
    }

    private void BackButton_Clicked(object sender, EventArgs e)
    {
        BackToPlaylists!.Invoke(sender, e);
    }

    #region Playlist
    public void RefreshCurrentPlaylist()
    {
        PlaylistNameEntry.Text = PlaylistManager.Instance.CurrentPlaylist.Name;

        if (PlaylistManager.Instance.CurrentPlaylist.SongParts is not null)
        {
            CurrentPlaylistListView.ItemsSource = PlaylistManager.Instance.CurrentPlaylist.SongParts;

            PlaylistManager.Instance.CurrentPlaylist.SetLength();
            LengthLabel.Text = String.Format("{0:hh\\:mm\\:ss}", PlaylistManager.Instance.CurrentPlaylist.Length);
            CountLabel.Text = $"Tot: {PlaylistManager.Instance.CurrentPlaylist.SongParts.Count}";
            BoygroupCountLabel.Text = $"BG: {PlaylistManager.Instance.CurrentPlaylist.SongParts.AsEnumerable().Count(s => s.Artist?.GroupType == GroupType.BG)}";
            GirlgroupCountLabel.Text = $"GG: {PlaylistManager.Instance.CurrentPlaylist.SongParts.AsEnumerable().Count(s => s.Artist?.GroupType == GroupType.GG)}";
        }
    }

    private void SavePlaylistButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            // Create file on system
            var fullPath = Path.Combine(MainViewModel.Path, $"{PlaylistNameEntry.Text}.txt");

            StringBuilder sb = new StringBuilder();
            foreach (SongPart songPart in PlaylistManager.Instance.CurrentPlaylist.SongParts)
            {
                sb.AppendLine($"{{{songPart.ArtistName}}}{{{songPart.AlbumTitle}}}{{{songPart.Title}}}{{{songPart.PartNameShort}}}{{{songPart.PartNameNumber}}}{{{songPart.ClipLength}}}{{{songPart.AudioURL}}}");
            }

            File.WriteAllText(fullPath, sb.ToString());

            Toast.Make($"{PlaylistNameEntry.Text} saved locally!", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        }
        catch (Exception ex)
        {
            Toast.Make(ex.Message, CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        }


        if (ViaCloudCheckBox.IsChecked && HelperClass.HasInternetConnection())
        {
            try
            {
                DropboxRepository.SavePlaylist(PlaylistNameEntry.Text);
                Toast.Make($"{PlaylistNameEntry.Text} saved locally and online!", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
            }
            catch (Exception ex)
            {
                Toast.Make(ex.Message, CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
            }
        }
    }

    private void ClearPlaylistButton_Clicked(object sender, EventArgs e)
    {
        PlaylistManager.Instance.ClearCurrentPlaylist();
        RefreshCurrentPlaylist();
    }

    public void ResetCurrentPlaylist()
    {
        if (PlaylistManager.Instance.CurrentPlaylist.SongParts is not null)
        {
            CurrentPlaylistListView.ItemsSource = null;

            PlaylistNameEntry.Text = string.Empty;
        }
    }

    private void PlayPlaylistButton_Clicked(object sender, EventArgs e)
    {
        PlaylistManager.Instance.CurrentSongPartIndex = 0;
        int index = PlaylistManager.Instance.CurrentSongPartIndex;
        MainViewModel.CurrentSongPart = PlaylistManager.Instance.CurrentPlaylist.SongParts[index];

        // Change mode to playlist
        MainViewModel.PlayMode = PlayMode.Playlist;
        PlaySongPart!.Invoke(sender, e);
    }

    private void ShufflePlaylistButton_Clicked(object sender, EventArgs e)
    {
        PlaylistManager.Instance.CurrentPlaylist.SongParts = HelperClass.RandomizePlaylist(PlaylistManager.Instance.CurrentPlaylist.SongParts.ToList()).ToObservableCollection();
        CurrentPlaylistListView.ItemsSource = PlaylistManager.Instance.CurrentPlaylist.SongParts;
    }

    private void MixedShufflePlaylistButton_Clicked(object sender, EventArgs e)
    {
        PlaylistManager.Instance.CurrentPlaylist.SongParts = HelperClass.RandomizeAndAlternatePlaylist(PlaylistManager.Instance.CurrentPlaylist.SongParts.ToList()).ToObservableCollection();
        CurrentPlaylistListView.ItemsSource = PlaylistManager.Instance.CurrentPlaylist.SongParts;
    }

    #endregion

    #region Songparts
    private void CurrentPlaylistListView_ItemTapped(object sender, Syncfusion.Maui.ListView.ItemTappedEventArgs e)
    {
        if (!HelperClass.HasInternetConnection())
            return;

        SongPart songPart = (SongPart)e.DataItem;

        if (songPart.AudioURL != string.Empty)
        {
            // Mode to queue/single song
            MainViewModel.PlayMode = PlayMode.Queue;

            MainViewModel.CurrentSongPart = songPart;
            PlaySongPart?.Invoke(sender, e);
        }

        CurrentPlaylistListView.SelectedItems?.Clear();
    }

    private void SwipeItemPlaySongPart(object sender, EventArgs e)
    {
        if (!HelperClass.HasInternetConnection())
            return;

        SongPart songPart = (SongPart)((MenuItem)sender).CommandParameter;
        if (songPart.AudioURL != string.Empty)
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
            PlaylistManager.Instance.RemoveSongpartOfCurrentPlaylist(songPart);

            RefreshCurrentPlaylist();
        }
    }

    #endregion
}