using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core.Extensions;
using RpdPlayerApp.Architecture;
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

        BackButtonImage.Source = IconManager.BackIcon;
    }

    private void BackButtonClicked(object sender, EventArgs e)
    {
        // To hide soft keyboard programmatically
        PlaylistNameEntry.IsEnabled = false;
        PlaylistNameEntry.IsEnabled = true;
        BackToPlaylists!.Invoke(sender, e);
    }

    #region Playlist
    internal void PlayPlaylistButtonClicked(object? sender, EventArgs e)
    {
       if (!PlaylistManager.Instance.CurrentPlaylist.SongParts.Any()) { return; }
        
        // Change mode to playlist
        MainViewModel.PlayMode = PlayMode.Playlist;
        
        // Clear playlist queue and fill playlist queue
        MainViewModel.PlaylistQueue.Clear();

        List<SongPart> songParts = [.. PlaylistManager.Instance.CurrentPlaylist.SongParts];
        foreach (var songPart in songParts)
        {
            MainViewModel.PlaylistQueue.Enqueue(songPart);
        }

        MainViewModel.CurrentSongPart = MainViewModel.PlaylistQueue.Dequeue();

        PlaySongPart!.Invoke(sender, e);
    }
    internal void SavePlaylistButtonClicked(object? sender, EventArgs e)
    {
        try
        {
            // Create file on system
            var fullPath = Path.Combine(MainViewModel.Path, $"{PlaylistNameEntry.Text}.txt");

            StringBuilder sb = new();
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


        if (MainViewModel.UsingCloudMode && HelperClass.HasInternetConnection())
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
    internal void ToggleCloudModePressed(object? sender, EventArgs e)
    {
        MainViewModel.UsingCloudMode = !MainViewModel.UsingCloudMode;

        string toastText = MainViewModel.UsingCloudMode ? $"Playlist will save online" : $"Playlist will save locally";
        Toast.Make(toastText, CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();

        ParentPage?.SetupLibraryToolbar();
    }
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

    internal void ClearPlaylistButtonClicked(object? sender, EventArgs e)
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


    private void ShufflePlaylistButton_Clicked(object sender, EventArgs e)
    {
        PlaylistManager.Instance.CurrentPlaylist.SongParts = HelperClass.RandomizePlaylist([.. PlaylistManager.Instance.CurrentPlaylist.SongParts]).ToObservableCollection();
        CurrentPlaylistListView.ItemsSource = PlaylistManager.Instance.CurrentPlaylist.SongParts;
    }

    private void MixedShufflePlaylistButton_Clicked(object sender, EventArgs e)
    {
        PlaylistManager.Instance.CurrentPlaylist.SongParts = HelperClass.RandomizeAndAlternatePlaylist([.. PlaylistManager.Instance.CurrentPlaylist.SongParts]).ToObservableCollection();
        CurrentPlaylistListView.ItemsSource = PlaylistManager.Instance.CurrentPlaylist.SongParts;
    }

    #endregion

    #region Songparts
    private void CurrentPlaylistListView_ItemTapped(object sender, Syncfusion.Maui.ListView.ItemTappedEventArgs e)
    {
        if (!HelperClass.HasInternetConnection())
            return;

        SongPart songPart = (SongPart)e.DataItem;

        if (!string.IsNullOrWhiteSpace(songPart.AudioURL))
        {
            MainViewModel.PlayMode = PlayMode.Playlist;

            MainViewModel.PlaylistQueue.Clear();

            List<SongPart> songParts = [.. PlaylistManager.Instance.CurrentPlaylist.SongParts];
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
        if (!HelperClass.HasInternetConnection())
            return;

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
            PlaylistManager.Instance.RemoveSongpartOfCurrentPlaylist(songPart);

            RefreshCurrentPlaylist();
        }
    }

    #endregion

}