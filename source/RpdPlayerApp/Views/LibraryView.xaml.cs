using CommunityToolkit.Maui.Core.Extensions;
using RpdPlayerApp.Architecture;
using RpdPlayerApp.Enums;
using RpdPlayerApp.Managers;
using RpdPlayerApp.Models;
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
        Loaded += OnLoad;
        InitializeLocalCloudModeSegmentedControl();
    }

    private void OnLoad(object? sender, EventArgs e)
    {
        DeleteInvalidPlaylists();
        LoadLocalPlaylists();
    }

    private void InitializeLocalCloudModeSegmentedControl()
    {
        CloudModeSegmentedControl.ItemsSource = new[] { "Local", "Cloud" };
        CloudModeSegmentedControl.SelectedIndex = 0;
        CloudModeSegmentedControl.SelectionChanged += LocalCloudModeSegmentedControlSelectionChanged;
    }

    private void LocalCloudModeSegmentedControlSelectionChanged(object? sender, Syncfusion.Maui.Buttons.SelectionChangedEventArgs e)
    {
        if (e.NewIndex == 0) { LoadLocalPlaylists(); }
        else { LoadCloudPlaylists(); }
    }

    internal void FocusNewPlaylistEntry() => PlaylistNameEntry.Focus();

    internal void LoadLocalPlaylists()
    {
        List<Playlist> playlists = [];

        string[] files = Directory.GetFiles(FileManager.GetPlaylistsPath(), "*.txt");

        if (files.Length > 0)
        {
            foreach (var file in files)
            {
                if (file.Contains("SONGPARTS.txt")) { continue; }

                int lines = File.ReadAllLines(file).Length;

                string fileName = Path.GetFileNameWithoutExtension(file);

                Playlist playlist = new(creationDate: DateTime.Today, name: Path.GetFileNameWithoutExtension(fileName), path: file, count: lines)
                {
                    SongParts = []
                };

                string? result = General.ReadTextFile(file);

                // Convert text to songParts.
                var pattern = @"\{(.*?)\}";
                var matches = Regex.Matches(result, pattern);

                for (int i = 0; i < matches.Count / Constants.SongPartPropertyAmount; i++)
                {
                    try
                    {
                        int n = Constants.SongPartPropertyAmount * i; // songpart number

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
                    catch (Exception ex)
                    {
                        //SentrySdk.CaptureMessage($"ERROR: {typeof(LibraryView).Name}, songpart {i + 1}, {ex.Message}");
                        General.ShowToast($"ERROR: LoadLocalPlaylists songpart {i + 1}. {ex.Message}");
                    }
                }

                playlist.SetLength();
                playlists.Add(playlist);
            }

            AppState.Playlists = playlists.ToObservableCollection();
        }

        PlaylistsListView.ItemsSource = AppState.Playlists;
    }

    internal void LoadCloudPlaylists()
    {
        List<Playlist> playlists = [];

        PlaylistsListView.ItemsSource = AppState.Playlists;
    }

    private void PlaylistsListViewItemTapped(object sender, Syncfusion.Maui.ListView.ItemTappedEventArgs e)
    {
        Playlist playlist = (Playlist)e.DataItem;
        CurrentPlaylistManager.Instance.CurrentPlaylist = playlist;
        ShowPlaylist?.Invoke(sender, e);
    }

    internal void NewPlaylistButtonClicked(object? sender, EventArgs e)
    {
        if (PlaylistNameEntry.Text.IsNullOrEmpty())
        {
            General.ShowToast($"Please fill in a name");
            return;
        }

        try
        {
            // Create file on system.
            Task<string> result = FileManager.SavePlaylistJsonToFileAsync(PlaylistNameEntry.Text, string.Empty);

            Playlist playlist = new(creationDate: DateTime.Today, name: PlaylistNameEntry.Text, path: result.Result)
            {
                SongParts = []
            };

            AppState.Playlists.Add(playlist);
        }
        catch (Exception ex)
        {
            General.ShowToast(ex.Message);
        }
    }

    private void CopyPlaylistButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            // Create file on system
            //StringBuilder sb = new StringBuilder();
            //foreach (SongPart songPart in PlaylistManager.Instance.CurrentPlaylist.SongParts)
            //{
            //    sb.AppendLine($"{{{songPart.ArtistName}}}{{{songPart.AlbumTitle}}}{{{songPart.Title}}}{{{songPart.PartNameShort}}}{{{songPart.PartNameNumber}}}{{{songPart.AudioURL}}}");
            //}

            File.WriteAllText($"{PlaylistNameEntry.Text} - copy.txt", string.Empty);

            General.ShowToast($"{PlaylistNameEntry.Text} - copy created!");
        }
        catch (Exception ex)
        {
            General.ShowToast(ex.Message);
        }
    }

    private void PlayPlaylistButton_Clicked(object sender, EventArgs e)
    {
        AppState.CurrentSongPart = CurrentPlaylistManager.Instance.CurrentPlaylist.SongParts[0];

        // Change mode to playlist
        AppState.PlayMode = PlayModeValue.Playlist;
        PlayPlaylist?.Invoke(sender, e);
    }

    private void SwipeItemRemoveSongPart(object sender, EventArgs e)
    {
        SongPart songPart = (SongPart)((MenuItem)sender).CommandParameter;
        CurrentPlaylistManager.Instance.RemoveSongpartOfCurrentPlaylist(songPart);
    }

    // Remove/delete playlist
    private async void PlaylistsListView_SwipeEnded(object sender, Syncfusion.Maui.ListView.SwipeEndedEventArgs e)
    {
        if (e.DataItem is null) { return; }

        if (e.Direction == SwipeDirection.Right && e.Offset > 30)
        {
            Playlist playlist = (Playlist)e.DataItem;

            bool accept = await ParentPage!.DisplayAlert("Confirmation", $"Delete {playlist.Name}?", "Yes", "No");
            if (accept)
            {
                File.Delete(playlist.LocalPath);
                AppState.Playlists.Remove(playlist);
            }
        }
    }

    private static void DeleteInvalidPlaylists()
    {
        string[] files = Directory.GetFiles(FileManager.GetPlaylistsPath(), "*.txt");
        foreach (string file in files)
        {
            if (file.Contains("SONGPARTS.txt")) { continue; }

            foreach (var line in File.ReadLines(file))
            {
                var pattern = @"\{(.*?)\}";
                var matches = Regex.Matches(line, pattern);
                if (matches.Count < Constants.SongPartPropertyAmount)
                {
                    File.Delete(file);
                    General.ShowToast("Found invalid or outdated playlists! They have been removed.");
                    break;
                }
            }
        }
    }
}