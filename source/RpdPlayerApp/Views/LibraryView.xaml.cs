using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core.Extensions;
using RpdPlayerApp.Architecture;
using RpdPlayerApp.Models;
using RpdPlayerApp.Repositories;
using RpdPlayerApp.Repository;
using RpdPlayerApp.ViewModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;

namespace RpdPlayerApp.Views;

public partial class LibraryView : ContentView
{
    public event EventHandler PlayPlaylist;
    public event EventHandler ShowPlaylist;

    public LibraryView()
    {
        InitializeComponent();

        var path = FileSystem.Current.AppDataDirectory;
        
        List<Playlist> playlists = [];
        string[] files = Directory.GetFiles(path, "*.txt");

        if (files.Length > 0)
        {
            foreach (var file in files)
            {
                int lines = File.ReadAllLines(file).Count();

                Playlist playlist = new Playlist(name: Path.GetFileNameWithoutExtension(file), path: file, count: lines);
                playlist.SongParts = new ObservableCollection<SongPart>();

                string? result = HelperClass.ReadTextFile(file);

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
                    songPart.Artist = ArtistRepository.MatchArtist(artistName);

                    songPart.AlbumURL = songPart.Album is not null ? songPart.Album.ImageURL : string.Empty;
                    playlist.SongParts.Add(songPart);
                }

                playlists.Add(playlist);
            }

            MainViewModel.Playlists = playlists.ToObservableCollection();
        }

        PlaylistsListView.ItemsSource = MainViewModel.Playlists;
    }

    private void CurrentPlaylistListView_ItemTapped(object sender, Syncfusion.Maui.ListView.ItemTappedEventArgs e)
    {
        Playlist playlist = (Playlist)e.DataItem;
        PlaylistManager.Instance.CurrentPlaylist = playlist;
        ShowPlaylist.Invoke(sender, e);
    }

    private void NewPlaylistButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            // Create file on system
            var path = FileSystem.Current.AppDataDirectory;
            var fullPath = Path.Combine(path, $"{PlaylistNameEntry.Text}.txt");

            File.WriteAllText(fullPath, string.Empty);

            Toast.Make($"{PlaylistNameEntry.Text} created!", CommunityToolkit.Maui.Core.ToastDuration.Short);

            Playlist playlist = new Playlist(name: PlaylistNameEntry.Text, fullPath);
            playlist.SongParts = new ObservableCollection<SongPart>();

            MainViewModel.Playlists.Add(playlist);
        }
        catch (Exception ex)
        {
            Toast.Make(ex.Message, CommunityToolkit.Maui.Core.ToastDuration.Short);
        }
    }

    private void CopyPlaylistButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            // Create file on system
            var path = FileSystem.Current.AppDataDirectory;
            var fullPath = Path.Combine(path, $"{PlaylistNameEntry.Text} - copy.txt");

            //StringBuilder sb = new StringBuilder();
            //foreach (SongPart songPart in PlaylistManager.Instance.CurrentPlaylist.SongParts)
            //{
            //    sb.AppendLine($"{{{songPart.ArtistName}}}{{{songPart.AlbumTitle}}}{{{songPart.Title}}}{{{songPart.PartNameShort}}}{{{songPart.PartNameNumber}}}{{{songPart.AudioURL}}}");
            //}

            File.WriteAllText(fullPath, string.Empty);

            Toast.Make($"{PlaylistNameEntry.Text} - copy created!", CommunityToolkit.Maui.Core.ToastDuration.Short);
        }
        catch (Exception ex)
        {
            Toast.Make(ex.Message, CommunityToolkit.Maui.Core.ToastDuration.Short);
        }
    }

    private void PlayPlaylistButton_Clicked(object sender, EventArgs e)
    {
        PlaylistManager.Instance.CurrentSongPartIndex = 0;
        int index = PlaylistManager.Instance.CurrentSongPartIndex;
        MainViewModel.CurrentSongPart = PlaylistManager.Instance.CurrentPlaylist.SongParts[index];

        // Change mode to playlist
        MainViewModel.IsPlayingPlaylist = true;
        PlayPlaylist.Invoke(sender, e);
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

    private void SwipeItemRemoveSongPart(object sender, EventArgs e)
    {
        SongPart songPart = (SongPart)((MenuItem)sender).CommandParameter;
        PlaylistManager.Instance.RemoveSongpartOfCurrentPlaylist(songPart);
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
            PlayPlaylist.Invoke(sender, e);
        }
    }

    // Remove/delete playlist
    private void CurrentPlaylistListView_SwipeEnded(object sender, Syncfusion.Maui.ListView.SwipeEndedEventArgs e)
    {
        if (e.Direction == SwipeDirection.Right && e.Offset > 30)
        {
            Playlist playlist = (Playlist)e.DataItem;

            File.Delete(playlist.LocalPath);

            MainViewModel.Playlists.Remove(playlist);
        }
    }
}