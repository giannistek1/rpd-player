using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core.Extensions;
using RpdPlayerApp.Architecture;
using RpdPlayerApp.Enums;
using RpdPlayerApp.Models;
using RpdPlayerApp.ViewModels;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace RpdPlayerApp.Views;

public partial class LibraryView : ContentView
{
    internal MainPage? ParentPage { get; set; }

    public event EventHandler? PlayPlaylist;
    public event EventHandler? ShowPlaylist;

    public LibraryView()
    {
        InitializeComponent();

        CheckValidPlaylists();

        LoadPlaylists();
    }

    internal void FocusNewPlaylistEntry() => PlaylistNameEntry.Focus();

    internal void LoadPlaylists()
    {
        List<Playlist> playlists = [];
        string[] files = Directory.GetFiles(MainViewModel.Path, "*.txt");

        if (files.Length > 0)
        {
            foreach (var file in files)
            {
                int lines = File.ReadAllLines(file).Length;

                Playlist playlist = new(name: Path.GetFileNameWithoutExtension(file), path: file, count: lines)
                {
                    SongParts = []
                };

                string? result = HelperClass.ReadTextFile(file);

                // Convert text to songParts
                var pattern = @"\{(.*?)\}";
                var matches = Regex.Matches(result, pattern);

                for (int i = 0; i < matches.Count / MainViewModel.SongPartPropertyAmount; i++)
                {
                    int n = MainViewModel.SongPartPropertyAmount * i; // songpart number

                    string artistName = matches[n + 0].Groups[1].Value;
                    string albumTitle = matches[n + 1].Groups[1].Value;
                    string videoURL = matches[n + 6].Groups[1].Value.Replace(".mp3", ".mp4").Replace("rpd-audio", "rpd-videos");

                    SongPart songPart = new(id: i,
                        artistName: artistName,
                        albumTitle: albumTitle,
                        title: matches[n + 2].Groups[1].Value,
                        partNameShort: $"{matches[n + 3].Groups[1].Value}",
                        partNameNumber: matches[n + 4].Groups[1].Value,
                        clipLength: Convert.ToDouble(matches[n + 5].Groups[1].Value),
                        audioURL: matches[n + 6].Groups[1].Value,
                        videoURL: videoURL
                    );

                    songPart.AlbumURL = songPart.Album is not null ? songPart.Album.ImageURL : string.Empty;
                    playlist.SongParts.Add(songPart);
                }

                playlist.SetLength();
                playlists.Add(playlist);
            }

            MainViewModel.Playlists = playlists.ToObservableCollection();
        }

        PlaylistsListView.ItemsSource = MainViewModel.Playlists;
    }

    private void PlaylistsListViewItemTapped(object sender, Syncfusion.Maui.ListView.ItemTappedEventArgs e)
    {
        Playlist playlist = (Playlist)e.DataItem;
        PlaylistManager.Instance.CurrentPlaylist = playlist;
        ShowPlaylist?.Invoke(sender, e);
    }

    internal void NewPlaylistButtonClicked(object? sender, EventArgs e)
    {
        if (PlaylistNameEntry.Text.IsNullOrBlank())
        {
            Toast.Make($"Please fill in a name", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
            return;
        }
        
        try
        {
            // Create file on system
            var fullPath = Path.Combine(MainViewModel.Path, $"{PlaylistNameEntry.Text}.txt");

            File.WriteAllText(fullPath, string.Empty);

            Toast.Make($"{PlaylistNameEntry.Text} created!", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();

            Playlist playlist = new(name: PlaylistNameEntry.Text, fullPath)
            {
                SongParts = []
            };

            MainViewModel.Playlists.Add(playlist);
        }
        catch (Exception ex)
        {
            Toast.Make(ex.Message, CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        }
    }

    private void CopyPlaylistButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            // Create file on system
            var fullPath = Path.Combine(MainViewModel.Path, $"{PlaylistNameEntry.Text} - copy.txt");

            //StringBuilder sb = new StringBuilder();
            //foreach (SongPart songPart in PlaylistManager.Instance.CurrentPlaylist.SongParts)
            //{
            //    sb.AppendLine($"{{{songPart.ArtistName}}}{{{songPart.AlbumTitle}}}{{{songPart.Title}}}{{{songPart.PartNameShort}}}{{{songPart.PartNameNumber}}}{{{songPart.AudioURL}}}");
            //}

            File.WriteAllText(fullPath, string.Empty);

            Toast.Make($"{PlaylistNameEntry.Text} - copy created!", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        }
        catch (Exception ex)
        {
            Toast.Make(ex.Message, CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        }
    }

    private void PlayPlaylistButton_Clicked(object sender, EventArgs e)
    {
        MainViewModel.CurrentSongPart = PlaylistManager.Instance.CurrentPlaylist.SongParts[0];

        // Change mode to playlist
        MainViewModel.PlayMode = PlayMode.Playlist;
        PlayPlaylist?.Invoke(sender, e);
    }

    private void SwipeItemRemoveSongPart(object sender, EventArgs e)
    {
        SongPart songPart = (SongPart)((MenuItem)sender).CommandParameter;
        PlaylistManager.Instance.RemoveSongpartOfCurrentPlaylist(songPart);
    }

    // Remove/delete playlist
    private void PlaylistsListView_SwipeEnded(object sender, Syncfusion.Maui.ListView.SwipeEndedEventArgs e)
    {
        if (e.DataItem is null) { return; }

        if (e.Direction == SwipeDirection.Right && e.Offset > 30)
        {
            Playlist playlist = (Playlist)e.DataItem;

            File.Delete(playlist.LocalPath);

            MainViewModel.Playlists.Remove(playlist);
        }
    }

    private static void CheckValidPlaylists()
    {
        string[] files = Directory.GetFiles(MainViewModel.Path, "*.txt");
        foreach (string file in files)
        {
            foreach (var line in File.ReadLines(file))
            {
                var pattern = @"\{(.*?)\}";
                var matches = Regex.Matches(line, pattern);
                if (matches.Count < MainViewModel.SongPartPropertyAmount)
                {
                    Toast.Make($"Found invalid or outdated playlists! They have been removed.", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
                    File.Delete(file);
                    break;
                }
            }
        }
    }
}