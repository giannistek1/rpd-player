using CommunityToolkit.Maui.Core.Extensions;
using RpdPlayerApp.Architecture;
using RpdPlayerApp.Enums;
using RpdPlayerApp.Managers;
using RpdPlayerApp.Models;
using RpdPlayerApp.Repositories;
using RpdPlayerApp.Services;
using RpdPlayerApp.ViewModels;
using System.Globalization;
using System.Text.RegularExpressions;

namespace RpdPlayerApp.Views;

public partial class LibraryView : ContentView
{
    internal MainPage? ParentPage { get; set; }

    private readonly LibraryViewModel _viewModel = new();

    public LibraryView()
    {
        InitializeComponent();
        BindingContext = _viewModel;

        Loaded += OnLoad;
        InitializePlaylistModeSegmentedControl();

        PullToRefresh.Refreshing += PullToRefreshRefreshing;
    }

    private void OnLoad(object? sender, EventArgs e)
    {
        DeleteInvalidPlaylists();
    }

    private void InitializePlaylistModeSegmentedControl()
    {
        PlaylistModeSegmentedControl.ItemsSource = new[] { "Local", "Cloud", "Public" };
        PlaylistModeSegmentedControl.SelectedIndex = 0;
        PlaylistModeSegmentedControl.SelectionChanged += PlaylistModeSegmentedControlSelectionChanged;
    }

    private async void PlaylistModeSegmentedControlSelectionChanged(object? sender, Syncfusion.Maui.Buttons.SelectionChangedEventArgs e)
    {
        if (e.NewIndex == 0) { PlaylistsManager.PlaylistMode = PlaylistModeValue.Local; }
        else if (e.NewIndex == 1) { PlaylistsManager.PlaylistMode = PlaylistModeValue.Cloud; }
        else { PlaylistsManager.PlaylistMode = PlaylistModeValue.Public; }

        await LoadPlaylists();
    }

    internal async void RefreshPlaylistsButtonClicked(object? sender, EventArgs e) => await LoadPlaylists(isDirty: true);

    internal async void NewPlaylistButtonClicked(object? sender, EventArgs e)
    {
        InputPromptResult result = await General.ShowInputPromptAsync("Create empty playlist", "Name", maxLength: 26);
        string playlistName = result.Text;

        if (result.IsCanceled) { return; }
        else if (string.IsNullOrWhiteSpace(playlistName))
        {
            General.ShowToast($"Please fill in a name");
            return;
        }

        try
        {
            if (PlaylistsManager.PlaylistMode == PlaylistModeValue.Local)
            {
                await CreateLocalPlaylist(playlistName);
            }
            else if (PlaylistsManager.PlaylistMode == PlaylistModeValue.Cloud)
            {
                await CreateCloudPlaylist(playlistName);
            }
        }
        catch (Exception ex)
        {
            General.ShowToast(ex.Message);
        }
    }

    private async Task CreateLocalPlaylist(string playlistName) => await _viewModel.CreateLocalPlaylist(playlistName);

    private async Task CreateCloudPlaylist(string playlistName) => await _viewModel.CreateEmptyCloudPlaylist(playlistName);

    internal async Task LoadPlaylists(bool isDirty = false)
    {
        PlaylistsListView.ItemsSource = null;

        isDirty = CacheState.IsDirty || isDirty;

        if (PlaylistsManager.PlaylistMode == PlaylistModeValue.Local) { LoadLocalPlaylists(isDirty: isDirty); }
        else if (PlaylistsManager.PlaylistMode == PlaylistModeValue.Cloud) { await LoadCloudPlaylists(isDirty: isDirty); }
        else { await LoadPublicPlaylists(isDirty: isDirty); }

        CacheState.IsDirty = false;
    }

    // Code goes here at start because of SegmentedControlSelectionChanged
    internal void LoadLocalPlaylists(bool isDirty = false)
    {
        if (CacheState.LocalPlaylists.Any() && !isDirty)
        {
            PlaylistsListView.ItemsSource = CacheState.LocalPlaylists;
            return;
        }

        string[] files = Directory.GetFiles(FileManager.GetPlaylistsPath(), "*.txt");

        if (files.Length <= 0)
        {
            PlaylistsListView.ItemsSource = CacheState.LocalPlaylists;
            return;
        }

        CacheState.LocalPlaylists.Clear();

        try
        {
            foreach (var file in files)
            {
                if (file.Contains("SONGPARTS.txt")) { continue; } // For offline mode / news

                string fileName = Path.GetFileNameWithoutExtension(file);

                string? result = General.ReadTextFile(file);

                int containsHeader = result.Contains("HDR:") ? 1 : 0;

                // Header should contain: Creation date, last modified date, user, count, length
                var headerPattern = @"\[(.*?)\]";
                string line1 = File.ReadLines(file).First();
                var headerMatches = Regex.Matches(line1, headerPattern);

                string user = string.Empty;

                DateTime creationDate = DateTime.Today;
                DateTime modifiedDate = DateTime.Today;
                if (containsHeader == 1)
                {
                    creationDate = DateTime.ParseExact(headerMatches[0].Groups[1].Value, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    modifiedDate = DateTime.ParseExact(headerMatches[1].Groups[1].Value, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    user = headerMatches[2].Groups[1].Value;
                }

                Playlist playlist = new(creationDate: creationDate, lastModifiedDate: modifiedDate, name: Path.GetFileNameWithoutExtension(fileName), path: file, owner: user);

                // Convert text to songParts.
                var pattern = @"\{(.*?)\}";
                var matches = Regex.Matches(result, pattern);

                TimeSpan startTime = TimeSpan.Zero;

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
                            audioUrl: matches[n + 6].Groups[1].Value,
                            videoUrl: videoURL,
                            playlistStartTime: startTime
                        );

                        songPart.AlbumUrl = songPart.Album is not null ? songPart.Album.ImageUrl : string.Empty;
                        playlist.Segments.Add(songPart);
                        startTime += songPart.ClipLengthAsTimeSpan;
                    }
                    catch (Exception ex)
                    {
                        DebugService.Instance.Error($"{typeof(LibraryView).Name} local: songpart {i + 1}, {ex.Message}");
                        General.ShowToast($"ERROR: LoadLocalPlaylists songpart {i + 1}. {ex.Message}");
                    }
                }

                CacheState.LocalPlaylists.Add(playlist);
            }
        }
        catch (Exception ex)
        {
            DebugService.Instance.Debug($"ERROR: {typeof(LibraryView).Name} - Local: {ex.Message}");
        }

        PlaylistsListView.ItemsSource = CacheState.LocalPlaylists;
    }

    private async void PullToRefreshRefreshing(object? sender, EventArgs e)
    {
        PullToRefresh.IsRefreshing = true;

        await LoadPlaylists(isDirty: true);

        PullToRefresh.IsRefreshing = false;
    }

    internal async Task LoadCloudPlaylists(bool isDirty = false)
    {
        if (!General.HasInternetConnection()) { return; }

        if (CacheState.CloudPlaylists.Any() && !isDirty)
        {
            PlaylistsListView.ItemsSource = CacheState.CloudPlaylists;
            return;
        }

        CacheState.CloudPlaylists.Clear();

        var cloudPlaylists = await PlaylistRepository.GetCloudPlaylists();
        foreach (var playlistDto in cloudPlaylists)
        {
            Playlist playlist = new(creationDate: playlistDto.CreationDate, lastModifiedDate: playlistDto.LastModifiedDate, name: playlistDto.Name, owner: AppState.Username)
            {
                Id = playlistDto.Id,
                IsCloudPlaylist = true,
                IsPublic = playlistDto.IsPublic
            };

            TimeSpan startTime = TimeSpan.Zero;

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
                                            audioUrl: segment.AudioUrl,
                                            videoUrl: segment.AudioUrl.Replace(".mp3", ".mp4").Replace("rpd-audio", "rpd-videos"),
                                            playlistStartTime: startTime
                                        );
                    songPart.AlbumUrl = songPart.Album is not null ? songPart.Album.ImageUrl : string.Empty;
                    playlist.Segments.Add(songPart);

                    startTime += songPart.ClipLengthAsTimeSpan;
                }
            }
            catch (Exception ex)
            {
                DebugService.Instance.Debug($"LibraryView - Cloud: {ex.Message}");
            }

            CacheState.CloudPlaylists!.Add(playlist);
        }

        PlaylistsListView.ItemsSource = CacheState.CloudPlaylists;
    }

    internal async Task LoadPublicPlaylists(bool isDirty = false)
    {
        if (!General.HasInternetConnection()) { return; }

        if (CacheState.PublicPlaylists.Any() && !isDirty)
        {
            PlaylistsListView.ItemsSource = CacheState.PublicPlaylists;
            return;
        }

        CacheState.PublicPlaylists.Clear();

        var results = await PlaylistRepository.GetAllPublicPlaylists();
        foreach (var playlistDto in results)
        {
            DebugService.Instance.Debug($"LibraryView: Pub: {playlistDto.CreationDate} | {playlistDto.LastModifiedDate}");

            Playlist playlist = new(creationDate: playlistDto.CreationDate, lastModifiedDate: playlistDto.LastModifiedDate, name: playlistDto.Name, owner: playlistDto.Owner)
            {
                Id = playlistDto.Id,
                IsCloudPlaylist = true,
                IsPublic = true,
            };

            TimeSpan startTime = TimeSpan.Zero;

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
                                            audioUrl: segment.AudioUrl,
                                            videoUrl: segment.AudioUrl.Replace(".mp3", ".mp4").Replace("rpd-audio", "rpd-videos"),
                                            playlistStartTime: startTime
                                        );

                    songPart.AlbumUrl = songPart.Album is not null ? songPart.Album.ImageUrl : string.Empty;
                    playlist.Segments.Add(songPart);

                    startTime += songPart.ClipLengthAsTimeSpan;
                }
            }
            catch (Exception ex)
            {
                DebugService.Instance.Debug($"LibraryView - Public: {ex.Message}");
            }

            CacheState.PublicPlaylists.Add(playlist);
        }

        PlaylistsListView.ItemsSource = CacheState.PublicPlaylists;
    }

    private void PlaylistsListViewItemTapped(object sender, Syncfusion.Maui.ListView.ItemTappedEventArgs e)
    {
        if (e.DataItem is not Playlist) { return; }

        Playlist playlist = (Playlist)e.DataItem;
        CurrentPlaylistManager.Instance.ChosenPlaylist = playlist;
        if (Shell.Current.CurrentPage is MainPage mainPage)
        {
            mainPage.ShowPlaylistView();
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

            //File.WriteAllText($"{PlaylistNameEntry.Text} - copy.txt", string.Empty);

            //General.ShowToast($"{PlaylistNameEntry.Text} - copy created!");
        }
        catch (Exception ex)
        {
            General.ShowToast(ex.Message);
        }
    }

    // TODO: Remove?
    private void SwipeItemRemoveSongPart(object sender, EventArgs e)
    {
        SongPart songPart = (SongPart)((MenuItem)sender).CommandParameter;
        CurrentPlaylistManager.Instance.RemoveSongpartOfCurrentPlaylist(songPart);
    }

    // Remove/delete playlist
    private async void PlaylistsListViewSwipeEnded(object sender, Syncfusion.Maui.ListView.SwipeEndedEventArgs e)
    {
        if (e.DataItem is null) { return; }

        if (e.Direction == SwipeDirection.Right && e.Offset > 30)
        {
            Playlist playlist = (Playlist)e.DataItem;
            try
            {
                await DeletePlaylist(playlist);
            }
            catch (Exception ex)
            {
                DebugService.Instance.Error(ex.Message);
                General.ShowToast(ex.Message);
            }
        }
    }

    private async Task DeletePlaylist(Playlist playlist)
    {
        bool accept = await ParentPage!.DisplayAlert("Confirmation", $"Delete {playlist.Name}?", "Yes", "No");
        if (accept)
        {
            PlaylistDeletedReturnValue returnValue = await PlaylistsManager.DeletePlaylist(playlist);
            switch (returnValue)
            {
                case PlaylistDeletedReturnValue.DeletedLocally:
                    General.ShowToast("Playlist deleted locally.");
                    break;
                case PlaylistDeletedReturnValue.DeletedFromCloud:
                    General.ShowToast("Playlist deleted from cloud.");
                    break;
                case PlaylistDeletedReturnValue.FailedToDelete:
                    General.ShowToast("Failed to delete playlist.");
                    break;
                case PlaylistDeletedReturnValue.CantDeletePublicPlaylist:
                    General.ShowToast("Can't delete public playlist.");
                    break;
            }
        }
    }

    private static void DeleteInvalidPlaylists()
    {
        string[] files = Directory.GetFiles(FileManager.GetPlaylistsPath(), "*.txt");
        foreach (string file in files)
        {
            if (file.Contains("SONGPARTS.txt")) { continue; } // Nowadays .json For news and offline mode

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