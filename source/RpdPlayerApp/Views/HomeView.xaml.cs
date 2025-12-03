using CommunityToolkit.Maui.Views;
using RpdPlayerApp.Architecture;
using RpdPlayerApp.Managers;
using RpdPlayerApp.Models;
using RpdPlayerApp.Repositories;
using RpdPlayerApp.Services;
using RpdPlayerApp.ViewModels;

namespace RpdPlayerApp.Views;

public partial class HomeView : ContentView
{
    //internal event EventHandler? PlaySongPart;
    internal event EventHandler? InitSongParts;
    internal event EventHandler? ShowNewsPopup;

    private readonly SettingsPage _settingsPage = new();
    private readonly FeedbackPage _feedbackPage = new();
    internal MainPage? ParentPage { get; set; }
    internal RpdSettings? RpdSettings { get; set; } = new();

    internal HomeViewModel _viewModel = new();

    private bool _isInitialized = false;

    public HomeView()
    {
        InitializeComponent();

        BindingContext = _viewModel;

        // TODO: Viewmodel commands.
        NewsImageButton.Clicked += NewsImageButtonClicked;
        StartRpdImageButton.Clicked += StartRpdButtonClicked;
        GeneratePlaylistImageButton.Clicked += GeneratePlaylistButtonClicked;
        CategoriesImageButton.Clicked += CategoriesButtonClicked;
    }
    internal async Task Init()
    {
        if (_isInitialized) { return; }

        // For debugging.
        int progress = 0;
        try
        {
            _ = HandleSongPartsDifference();
            progress++;

            SetVersionLabel();
            progress++;
#if RELEASE 
            UniqueSongCountImage.IsVisible = false;
            UniqueSongCountLabel.IsVisible = false;
#endif

            await FileManager.SaveSongPartsAsync([.. SongPartRepository.SongParts]);
            await FileManager.SaveArtistsAsync([.. ArtistRepository.Artists]);
            await FileManager.SaveAlbumsAsync([.. AlbumRepository.Albums]);
            await FileManager.SaveVideosAsync([.. VideoRepository.Videos]);
            progress++;

            bool validConnection = General.HasInternetConnection() && !Constants.APIKEY.IsNullOrWhiteSpace() && !Constants.BASE_URL.IsNullOrWhiteSpace();

            IsOnlineImage.Source = validConnection ? IconManager.OnlineIcon : IconManager.OfflineIcon;
            IsOnlineLabel.Text = General.HasInternetConnection() ? "Online" : "Offline";

            _isInitialized = true;
        }
        catch (Exception ex)
        {
            DebugService.Instance.Debug($"HomeView - {progress}: {ex.Message}");
        }
    }

    private async Task HandleSongPartsDifference()
    {
        if (SongPartRepository.SongParts is null || SongPartRepository.SongParts.Count == 0) { return; }

        var oldSongList = await FileManager.LoadSongPartsAsync();

        if (oldSongList is not null)
        {
            var newSongList = SongPartRepository.SongParts.ToList();
            var differentNewSongs = FindDifferentNewSongs(newSongList, oldSongList);
            UpdateNewsBadge(differentNewSongs);
        }
    }

    private List<SongPart> FindDifferentNewSongs(List<SongPart> newSongList, List<SongPart> oldSongList)
    {
#if RELEASE
        // Add new songs based on audioUrl.
        var differentNewSongs = newSongList.Where(item1 => !oldSongList.Any(item2 => item1.AudioUrl == item2.AudioUrl)).ToList();
        foreach (var newSongPart in newSongList)
        {
            if (!newSongPart.HasVideo) continue;

            var oldSong = oldSongList.FirstOrDefault(s => s?.AudioUrl == newSongPart.AudioUrl);
            if (oldSong is not null && oldSong.HasVideo != newSongPart.HasVideo)
            {
                newSongPart.NewVideoAvailable = true;
            }
        }

        // Add new songs based on video availability.
        var videoDiffs = newSongList
            .Where(item1 => !oldSongList.Any(item2 => item1.HasVideo == item2.HasVideo))
            .Except(differentNewSongs)
            .ToList();

        differentNewSongs.AddRange(videoDiffs);
#else
        var differentNewSongs = newSongList.Where(s => s.ArtistName!.Equals("ATEEZ", StringComparison.OrdinalIgnoreCase)).ToList();
#endif
        return differentNewSongs;
    }

    internal void UpdateNewsBadge(List<SongPart> differentNewSongs)
    {
        if (NewsManager.IsTestMode)
        {
            // Simulate ATEEZ as new songs.
            differentNewSongs = SongPartRepository.SongParts.Where(s => s.ArtistName!.Equals("ATEEZ", StringComparison.OrdinalIgnoreCase)).ToList();
            // Randomize HasNewVideo boolean.
            foreach (var item in differentNewSongs)
            {
                item.NewVideoAvailable = Convert.ToBoolean(General.Rng.Next(2));
            }
            NewsManager.IsTestMode = false;
        }

        if (differentNewSongs.Count > 0)
        {
            NewsManager.SongPartsDifference = differentNewSongs;

            NewsBadgeView.BadgeText = differentNewSongs.Count.ToString();
            NewsBadgeView.IsVisible = true; // Make clickable
        }
        else
        {
            NewsBadgeView.IsVisible = false;
        }
    }

    private void SetVersionLabel()
    {
#if DEBUG
        string releaseMode = "D"; // Debug
#else
        string releaseMode = "P"; // Production
#endif
        VersionLabel.Text = $"v{AppInfo.Current.VersionString}-{AppInfo.Current.BuildString}{releaseMode}";
    }

    internal async void FeedbackButtonPressed(object? sender, EventArgs e)
    {
        if (Navigation.NavigationStack.Count < 2)
        {
            await Navigation.PushAsync(_feedbackPage, true);
        }
    }

    internal async void SettingsButtonPressed(object? sender, EventArgs e)
    {
        if (Navigation.NavigationStack.Count < 2)
        {
            await Navigation.PushAsync(_settingsPage, true);
        }
    }

    private void NewsImageButtonClicked(object? sender, EventArgs e) => ShowNewsPopup?.Invoke(sender, e);
    private async void StartRpdButtonClicked(object? sender, EventArgs e)
    {
        RpdSettings.UsingGeneratePlaylist = false;
        await ParentPage!.ShowRpdPlaylistView();
    }
    private async void GeneratePlaylistButtonClicked(object? sender, EventArgs e)
    {
        RpdSettings.UsingGeneratePlaylist = true;
        await ParentPage!.ShowRpdPlaylistView();
    }
    private void CategoriesButtonClicked(object? sender, EventArgs e) => ParentPage!.ShowHomeCategories(sender, e);
}