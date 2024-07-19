using CommunityToolkit.Maui.Alerts;
using RpdPlayerApp.Architecture;
using RpdPlayerApp.Models;
using RpdPlayerApp.Repositories;
using RpdPlayerApp.Repository;
using RpdPlayerApp.ViewModel;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;

namespace RpdPlayerApp.Views;

public partial class CurrentPlaylistView : ContentView
{
    public event EventHandler PlaySongPart;
    public event EventHandler BackToPlaylists;

    public CurrentPlaylistView()
    {
        InitializeComponent();

        CurrentPlaylistListView.DragDropController.UpdateSource = true;
    }

    public void InitCurrentPlaylist()
    {

        PlaylistNameEntry.Text = PlaylistManager.Instance.CurrentPlaylist.Name;

        if (PlaylistManager.Instance.CurrentPlaylist.SongParts is not null)
        {
            PlaylistManager.Instance.CurrentPlaylist.SongParts.CollectionChanged += CurrentPlaylistCollectionChanged;

            CurrentPlaylistListView.ItemsSource = PlaylistManager.Instance.CurrentPlaylist.SongParts;

            CountLabel.Text = $"Count: {PlaylistManager.Instance.GetCurrentPlaylistSongCount()}";
            BoygroupCountLabel.Text = $"BG: {PlaylistManager.Instance.CurrentPlaylist.SongParts.AsEnumerable().Count(s => s.Artist?.GroupType == GroupType.BG)}";
            GirlgroupCountLabel.Text = $"GG: {PlaylistManager.Instance.CurrentPlaylist.SongParts.AsEnumerable().Count(s => s.Artist?.GroupType == GroupType.GG)}";
        }
    }

    public void ResetCurrentPlaylist()
    {
        if (PlaylistManager.Instance.CurrentPlaylist.SongParts is not null)
        {
            PlaylistManager.Instance.CurrentPlaylist.SongParts.CollectionChanged -= CurrentPlaylistCollectionChanged;

            CurrentPlaylistListView.ItemsSource = null;

            PlaylistNameEntry.Text = string.Empty;
        }
    }

    private void CurrentPlaylistCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        CountLabel.Text = $"Count: {PlaylistManager.Instance.GetCurrentPlaylistSongCount()}";
        BoygroupCountLabel.Text = $"BG: {PlaylistManager.Instance.CurrentPlaylist.SongParts.AsEnumerable().Count(s => s.Artist?.GroupType == GroupType.BG)}";
        GirlgroupCountLabel.Text = $"GG: {PlaylistManager.Instance.CurrentPlaylist.SongParts.AsEnumerable().Count(s => s.Artist?.GroupType == GroupType.GG)}";
    }

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

    private void BackButton_Clicked(object sender, EventArgs e)
    {
        BackToPlaylists.Invoke(sender, e);
    }

    private void ClearButton_Clicked(object sender, EventArgs e)
    {
        PlaylistManager.Instance.ClearCurrentPlaylist();
    }

    private void PlayPlaylistButton_Clicked(object sender, EventArgs e)
    {
        PlaylistManager.Instance.CurrentSongPartIndex = 0;
        int index = PlaylistManager.Instance.CurrentSongPartIndex;
        MainViewModel.CurrentSongPart = PlaylistManager.Instance.CurrentPlaylist.SongParts[index];

        // Change mode to playlist
        MainViewModel.PlayMode = PlayMode.Playlist;
        PlaySongPart.Invoke(sender, e);
    }

    private void SavePlaylistButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            // Create file on system
            var path = FileSystem.Current.AppDataDirectory;
            var fullPath = Path.Combine(path, $"{PlaylistNameEntry.Text}.txt");

            StringBuilder sb = new StringBuilder();
            foreach (SongPart songPart in PlaylistManager.Instance.CurrentPlaylist.SongParts)
            {
                sb.AppendLine($"{{{songPart.ArtistName}}}{{{songPart.AlbumTitle}}}{{{songPart.Title}}}{{{songPart.PartNameShort}}}{{{songPart.PartNameNumber}}}{{{songPart.AudioURL}}}");
            }

            File.WriteAllText(fullPath, sb.ToString());
        }
        catch (Exception ex)
        {
            Toast.Make(ex.Message, CommunityToolkit.Maui.Core.ToastDuration.Short);
        }
        

        if (ViaCloudCheckBox.IsChecked && HelperClass.HasInternetConnection())
        {
            try
            {
                DropboxRepository.SavePlaylist(PlaylistNameEntry.Text);
                Toast.Make("Saved playlist!");
            }
            catch (Exception ex)
            {
                Toast.Make(ex.Message, CommunityToolkit.Maui.Core.ToastDuration.Short);
            }
        }
    }

    private async void LoadPlaylistButton_Clicked(object sender, EventArgs e)
    {
        if (ViaCloudCheckBox.IsChecked && HelperClass.HasInternetConnection())
        {
            try
            {
                //CurrentPlaylistListView.ItemsSource = null;
                PlaylistManager.Instance.ClearCurrentPlaylist();

                var result = await DropboxRepository.LoadPlaylist(PlaylistNameEntry.Text);

                if (result.Contains("Error"))
                {
                    Toast.Make(result);
                    return;
                }

                // Convert text to songParts
                var pattern = @"\{(.*?)\}";
                var matches = Regex.Matches(result, pattern);

                for (int i = 0; i < matches.Count / 6; i++)
                {
                    int n = 6 * i; // songpart number

                    string artistName = matches[n + 0].Groups[1].Value;
                    string albumTitle = matches[n + 1].Groups[1].Value;

                    SongPart songPart = new SongPart(id: i, artistName: artistName, albumTitle: albumTitle, title: matches[n + 2].Groups[1].Value, partNameShort: $"{matches[n + 3].Groups[1].Value}", partNameNumber: matches[n + 4].Groups[1].Value, audioURL: matches[n + 5].Groups[1].Value);
                    songPart.Album = AlbumRepository.MatchAlbum(artistName, albumTitle);

                    songPart.AlbumURL = songPart.Album is not null ? songPart.Album.ImageURL : string.Empty;
                    PlaylistManager.Instance.AddSongPartToCurrentPlaylist(songPart);
                }

                //CurrentPlaylistListView.ItemsSource = PlaylistManager.Instance.CurrentPlaylist;
            }
            catch (Exception ex)
            {
                Toast.Make(ex.Message, CommunityToolkit.Maui.Core.ToastDuration.Short);
            }
        }
        else
        {
            var path = FileSystem.Current.AppDataDirectory;
            var fullPath = Path.Combine(path, $"{PlaylistNameEntry.Text}.txt");

            var result = HelperClass.ReadTextFile(fullPath);

            if (result.Equals("File not found.") || result.StartsWith("An error occurred"))
            {
                Toast.Make(result);
                return;
            } 

            //CurrentPlaylistListView.ItemsSource = null;
            PlaylistManager.Instance.ClearCurrentPlaylist();

            // Convert text to songParts
            var pattern = @"\{(.*?)\}";
            var matches = Regex.Matches(result, pattern);

            for (int i = 0; i < matches.Count / 6; i++)
            {
                int n = 6 * i; // songpart number

                string artistName = matches[n + 0].Groups[1].Value;
                string albumTitle = matches[n + 1].Groups[1].Value;

                SongPart songPart = new SongPart(id: i, artistName: artistName, albumTitle: albumTitle, title: matches[n + 2].Groups[1].Value, partNameShort: $"{matches[n + 3].Groups[1].Value}", partNameNumber: matches[n + 4].Groups[1].Value, audioURL: matches[n + 5].Groups[1].Value);
                songPart.Album = AlbumRepository.MatchAlbum(artistName, albumTitle);

                songPart.AlbumURL = songPart.Album is not null ? songPart.Album.ImageURL : string.Empty;
                PlaylistManager.Instance.AddSongPartToCurrentPlaylist(songPart);
            }

            //CurrentPlaylistListView.ItemsSource = PlaylistManager.Instance.CurrentPlaylist;
        }
    }

    private void ShufflePlaylistButton_Clicked(object sender, EventArgs e)
    {
        PlaylistManager.Instance.CurrentPlaylist.SongParts.Shuffle();
    }

    private void SwipeItemPlaySongPart(object sender, EventArgs e)
    {
        if (!HelperClass.HasInternetConnection())
            return;

        SongPart songPart = (SongPart)((MenuItem)sender).CommandParameter;
        if (songPart.AudioURL != string.Empty)
        {
            MainViewModel.CurrentSongPart = songPart;
            PlaySongPart.Invoke(sender, e);
        }
    }

    private void CurrentPlaylistListView_SwipeEnded(object sender, Syncfusion.Maui.ListView.SwipeEndedEventArgs e)
    {
        if (e.Direction == SwipeDirection.Right && e.Offset > 30)
        {
            SongPart songPart = (SongPart)e.DataItem;
            PlaylistManager.Instance.RemoveSongpartOfCurrentPlaylist(songPart);
            CountLabel.Text = PlaylistManager.Instance.CurrentPlaylist.Count.ToString();
        }
    }
}