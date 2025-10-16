using CommunityToolkit.Maui.Core.Extensions;
using Newtonsoft.Json;
using RpdPlayerApp.Architecture;
using RpdPlayerApp.DTO;
using RpdPlayerApp.Enums;
using RpdPlayerApp.Managers;
using RpdPlayerApp.Models;
using RpdPlayerApp.Repositories;
using System.Globalization;
using System.Text.RegularExpressions;

namespace RpdPlayerApp.Views;

public partial class LibraryView : ContentView
{
    internal MainPage? ParentPage { get; set; }

    public event EventHandler? PlayPlaylist;

    public event EventHandler? ShowPlaylist;

    PlaylistRepository _playlistRepository = new();

    internal int PlaylistMode { get; set; } = 0;

    public LibraryView()
    {
        InitializeComponent();
        Loaded += OnLoad;
        InitializeLocalCloudModeSegmentedControl();
    }

    private void OnLoad(object? sender, EventArgs e)
    {
        DeleteInvalidPlaylists();
    }

    private void InitializeLocalCloudModeSegmentedControl()
    {
        CloudModeSegmentedControl.ItemsSource = new[] { "Local", "Cloud", "Public" };
        CloudModeSegmentedControl.SelectedIndex = 0;
        CloudModeSegmentedControl.SelectionChanged += LocalCloudModeSegmentedControlSelectionChanged;
    }

    private void LocalCloudModeSegmentedControlSelectionChanged(object? sender, Syncfusion.Maui.Buttons.SelectionChangedEventArgs e)
    {
        if (e.NewIndex == 0) { PlaylistMode = 0; LoadLocalPlaylists(); }
        else if (e.NewIndex == 1) { PlaylistMode = 1; LoadCloudPlaylists(); }
        else { PlaylistMode = 2; LoadPublicPlaylists(); }
    }

    internal void LoadPlaylists()
    {
        if (PlaylistMode == 0) { LoadLocalPlaylists(); }
        else if (PlaylistMode == 1) { LoadCloudPlaylists(); }
        else { LoadPublicPlaylists(); }
    }

    internal void FocusNewPlaylistEntry() => PlaylistNameEntry.Focus();

    // Code goes here at start because of SegmentedControlSelectionChanged
    internal void LoadLocalPlaylists()
    {
        if (CacheState.LocalPlaylists is not null && CacheState.LocalPlaylists.Any())
        {
            PlaylistsListView.ItemsSource = CacheState.LocalPlaylists;
            return;
        }

        List<Playlist> playlists = [];

        string[] files = Directory.GetFiles(FileManager.GetPlaylistsPath(), "*.txt");

        if (files.Length <= 0) { return; }

        try
        {
            foreach (var file in files)
            {
                if (file.Contains("SONGPARTS.txt")) { continue; }

                int lines = File.ReadAllLines(file).Length;

                string fileName = Path.GetFileNameWithoutExtension(file);

                string? result = General.ReadTextFile(file);

                int containsHeader = result.Contains("HDR:") ? 1 : 0;

                // Header should contain: Creation date, last modified date, user, count, length
                var headerPattern = @"\[(.*?)\]";
                string line1 = File.ReadLines(file).First();
                var headerMatches = Regex.Matches(line1, headerPattern);

                DateTime creationDate = DateTime.Today;
                if (containsHeader == 1)
                {
                    creationDate = DateTime.ParseExact(headerMatches[0].Groups[1].Value, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    DateTime modifiedDate = DateTime.ParseExact(headerMatches[1].Groups[1].Value, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    string user = headerMatches[2].Groups[1].Value;
                    int count = Convert.ToInt32(headerMatches[3].Groups[1].Value);
                    TimeSpan length = TimeSpan.Parse(headerMatches[4].Groups[1].Value);
                }

                Playlist playlist = new(creationDate: creationDate, name: Path.GetFileNameWithoutExtension(fileName), path: file, count: lines - containsHeader)
                {
                    SongParts = []
                };

                // Convert text to songParts.
                var pattern = @"\{(.*?)\}";
                var matches = Regex.Matches(result, pattern);

                for (int i = 0; i < matches.Count / Constants.SongPartPropertyAmount; i++)
                {
                    try
                    {
                        int n = Constants.SongPartPropertyAmount * i; // i = songpart number

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
                        DebugService.Instance.AddDebug($"ERROR: {typeof(LibraryView).Name}, songpart {i + 1}, {ex.Message}");
                        General.ShowToast($"ERROR: LoadLocalPlaylists songpart {i + 1}. {ex.Message}");
                    }
                }

                playlist.SetLength();
                playlist.SetCount();
                playlists.Add(playlist);
            }
        }
        catch (Exception ex)
        {
            DebugService.Instance.AddDebug(ex.Message);
        }

        CacheState.LocalPlaylists = playlists.ToObservableCollection();
        PlaylistsListView.ItemsSource = CacheState.LocalPlaylists;
    }

    internal void LoadCloudPlaylists()
    {
        if (CacheState.CloudPlaylists is not null && CacheState.CloudPlaylists.Any())
        {
            PlaylistsListView.ItemsSource = CacheState.CloudPlaylists;
            return;
        }

        List<Playlist> playlists = [];

        CacheState.CloudPlaylists = playlists.ToObservableCollection();
        PlaylistsListView.ItemsSource = CacheState.CloudPlaylists;
    }

    internal async void LoadPublicPlaylists()
    {
        if (CacheState.PublicPlaylists is not null && CacheState.PublicPlaylists.Any())
        {
            PlaylistsListView.ItemsSource = CacheState.PublicPlaylists;
            return;
        }

        List<Playlist> playlists = [];

        var results = await _playlistRepository.GetAllPublicPlaylists();
        foreach (var playlistDto in results)
        {
            Playlist playlist = new(creationDate: playlistDto.CreationDate, name: playlistDto.Title, count: playlistDto.Count)
            {
                SongParts = []
            };

            try
            {
                foreach (var segment in playlistDto.Segments)
                {
                    SongPart songPart = new(id: segment.Id,
                                            artistName: segment.ArtistName,
                                            albumTitle: segment.AlbumName,
                                            title: segment.Title,
                                            partNameShort: segment.SegmentShort,
                                            partNameNumber: segment.SegmentNumber,
                                            clipLength: segment.ClipLength,
                                            audioURL: segment.AudioUrl,
                                            videoURL: segment.AudioUrl.Replace(".mp3", ".mp4").Replace("rpd-audio", "rpd-videos")
                                        );

                    songPart.AlbumURL = songPart.Album is not null ? songPart.Album.ImageURL : string.Empty;
                    playlist.SongParts.Add(songPart);

                    DebugService.Instance.AddDebug($"{songPart.PartNameFull} - {songPart.PartNameShortWithNumber} added!");
                }
            }
            catch (Exception ex)
            {
                DebugService.Instance.AddDebug(ex.Message);
            }

            playlist.SetLength();
            playlist.SetCount();
            playlists.Add(playlist);
        }

        CacheState.PublicPlaylists = playlists.ToObservableCollection();
        PlaylistsListView.ItemsSource = CacheState.PublicPlaylists;
    }

    private void PlaylistsListViewItemTapped(object sender, Syncfusion.Maui.ListView.ItemTappedEventArgs e)
    {
        Playlist playlist = (Playlist)e.DataItem;
        CurrentPlaylistManager.Instance.ChosenPlaylist = playlist;
        ShowPlaylist?.Invoke(sender, e);
    }

    internal async void NewPlaylistButtonClicked(object? sender, EventArgs e)
    {
        if (PlaylistNameEntry.Text.IsNullOrWhiteSpace())
        {
            General.ShowToast($"Please fill in a name");
            return;
        }

        try
        {
            // HDR: Creation date | Modified date | Owner | Count | Length | Countdown mode
            string playlistHeader = $"HDR:[{DateTime.Today}][{DateTime.Today}][{AppState.Username}][0][{TimeSpan.Zero}][0]";

            string result = await FileManager.SavePlaylistStringToTextFileAsync(PlaylistNameEntry.Text, playlistHeader);
            Playlist playlist = new(creationDate: DateTime.Today, name: PlaylistNameEntry.Text, path: result)
            {
                SongParts = []
            };

            if (CacheState.LocalPlaylists is not null)
            {
                CacheState.LocalPlaylists.Add(playlist);
            }
            PlaylistNameEntry.Text = string.Empty;
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

    // TODO: todo
    private void PlayPlaylistButton_Clicked(object sender, EventArgs e)
    {
        //AppState.CurrentSongPart = CurrentPlaylistManager.Instance.ChosenPlaylist.SongParts[0];

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
                if (PlaylistMode == 0)
                {
                    CacheState.LocalPlaylists?.Remove(playlist);
                }
                else if (PlaylistMode == 1)
                {
                    CacheState.CloudPlaylists?.Remove(playlist);
                }
                else
                {
                    CacheState.PublicPlaylists?.Remove(playlist);
                }
            }
        }
    }

    private static void DeleteInvalidPlaylists()
    {
        string[] files = Directory.GetFiles(FileManager.GetPlaylistsPath(), "*.txt");
        foreach (string file in files)
        {
            if (file.Contains("SONGPARTS.txt")) { continue; } // For offline mode?

            var lines = File.ReadLines(file);

            // Skip first line if it starts with "HDR:"
            if (lines.FirstOrDefault()?.StartsWith("HDR:", StringComparison.OrdinalIgnoreCase) == true) { lines = lines.Skip(1); }

            foreach (var line in lines)
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