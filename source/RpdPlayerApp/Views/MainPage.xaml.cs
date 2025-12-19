using CommunityToolkit.Maui.Views;
using RpdPlayerApp.Architecture;
using RpdPlayerApp.Enums;
using RpdPlayerApp.Managers;
using RpdPlayerApp.Models;
using RpdPlayerApp.Repositories;
using RpdPlayerApp.Services;
using System.Threading.Tasks;

namespace RpdPlayerApp.Views;

public partial class MainPage
{
    private readonly HomeCategoriesView _homeCategoriesView = new(); // Home
    private readonly CurrentPlaylistView _currentPlaylistView = new(); // Playlists
    private readonly HomeRpdPlaylistView _homeRpdPlaylistView;

    private SortByPopup? _sortByBottomSheet;
    private SongSegmentDetailPopup? _detailBottomSheet;

    private bool _useTabAnimation = false;
    internal RpdSettings? RpdSettings { get; set; }

    public MainPage()
    {
        InitializeComponent();

        RpdSettings = new RpdSettings();
        _homeRpdPlaylistView = new(RpdSettings);

        BindingContext = DebugService.Instance;

        CommonSettings.ActivityTimeStopWatch.Start();

        AudioPlayerControl.CurrentPlaylistViewModel = _currentPlaylistView._viewModel;

        SetupHomeToolbar();

        Loaded += async (s, e) => await OnLoadedAsync();
        Appearing += OnAppearing;
    }

    private async Task OnLoadedAsync()
    {
        await LoadSettingsAsync();

        SetParentPages();
        SetContentViewEvents();
        InitializePageContainers();
        KeepScreenOn();

        // Run heavy data loading on a background thread to avoid blocking the UI thread.
        await Task.Run(async () => await LoadInitialDataInBackground());

        await HomeView.Init();
        HandleAutoStartRpd();

        SearchSongPartsView.Init();
    }

    // Performs initial data loading on a background thread. Any UI-affecting callbacks or logs are marshalled back to the main thread.
    private async Task LoadInitialDataInBackground()
    {
        if (General.HasInternetConnection())
        {
            // These repository calls may be CPU or IO heavy; run them on background thread.
            ArtistRepository.GetArtists();
            AlbumRepository.GetAlbums();
            VideoRepository.GetVideos();
            SongPartRepository.GetSongParts();

            // If RpdSettings initialization touches UI or preferences, run it on main thread to be safe.
            MainThread.BeginInvokeOnMainThread(() => RpdSettings.InitializeCompanies());
        }
        else
        {
            await LoadDataOffline();

            // Marshal logs and any UI callbacks back to the main thread.
            MainThread.BeginInvokeOnMainThread(() =>
            {
                RpdSettings.InitializeCompanies();
                OnInitSongParts(this, EventArgs.Empty);
            });
        }
    }

    private async Task LoadDataOffline()
    {

        var loadedArtists = await FileManager.LoadArtistsAsync();

        ArtistRepository.Artists.Clear();
        foreach (Artist artist in loadedArtists)
        {
            artist.InitPostProperties();
            ArtistRepository.Artists.Add(artist);
        }

        var loadedAlbums = await FileManager.LoadAlbumsAsync();

        AlbumRepository.Albums.Clear();
        foreach (Album album in loadedAlbums)
        {
            album.InitPostProperties();
            AlbumRepository.Albums.Add(album);
        }

        var loadedVideos = await FileManager.LoadVideosAsync();

        VideoRepository.Videos.Clear();
        foreach (Video video in loadedVideos)
        {
            VideoRepository.Videos.Add(video);
        }

        var loadedSongParts = await FileManager.LoadSongPartsAsync();

        SongPartRepository.SongParts.Clear();
        foreach (SongPart part in loadedSongParts)
        {
            part.InitPostProperties();
            SongPartRepository.SongParts.Add(part);
        }
    }

    private void OnAppearing(object? sender, EventArgs e)
    {
        // Set tab animation preference. TODO: Can be more optimized
        if (Preferences.ContainsKey(CommonSettings.USE_TAB_ANIMATION)) { _useTabAnimation = Preferences.Get(key: CommonSettings.USE_TAB_ANIMATION, defaultValue: false); }
        MainContainer.ContentTransitionDuration = _useTabAnimation ? 0.0 : 1.0; // Only reverse works?
        DebugService.Instance.Debug($"MainPage: tab animation: {_useTabAnimation}");

        AudioPlayerControl.UpdateUI();
    }

    private static async Task LoadSettingsAsync()
    {
        AppState.DeviceId = await DeviceIdManager.GetDeviceIdAsync();
        AppState.Username = Preferences.Get(CommonSettings.USERNAME, Constants.DEFAULT_USERNAME);

        if (Preferences.ContainsKey(CommonSettings.MAIN_VOLUME))
        {
            CommonSettings.MainVolume = Preferences.Get(CommonSettings.MAIN_VOLUME, CommonSettings.DEFAULT_MAIN_VOLUME);
        }
        if (Preferences.ContainsKey(CommonSettings.TOTAL_ACTIVITY_TIME))
        {
            CommonSettings.TotalActivityTime = TimeSpan.Parse(Preferences.Get(key: CommonSettings.TOTAL_ACTIVITY_TIME, defaultValue: new TimeSpan(0).ToString()));
        }
        else
        {
            CommonSettings.TotalActivityTime = new TimeSpan(0);
        }
    }

    private void SetParentPages()
    {
        HomeView.ParentPage = this;
        _homeCategoriesView.ParentPage = this;
        _homeRpdPlaylistView.ParentPage = this;
        SearchSongPartsView.ParentPage = this;
        LibraryView.ParentPage = this;
        _currentPlaylistView.ParentPage = this;
    }

    private void SetContentViewEvents()
    {
        //HomeView.PlaySongPart += OnPlaySongPart;
        HomeView.ShowNewsPopup += OnShowNewsPopup;
        HomeView.InitSongParts += OnInitSongParts;

        _homeCategoriesView.IsVisible = false;
        _homeCategoriesView.FilterPressed += OnFilterPressed;

        _homeRpdPlaylistView.IsVisible = false;
        _homeRpdPlaylistView.InitSongParts += OnInitSongParts;
        _homeRpdPlaylistView.CreatePlaylistButtonPressed += OnCreatePlaylistButtonPressed;

        SearchSongPartsView.PlaySongPart += OnPlaySongPart;
        SearchSongPartsView.AddSongPart += OnAddSongPart;
        SearchSongPartsView.EnqueueSongPart += OnEnqueueSongPart;
        SearchSongPartsView.ShowSortBy += OnShowSortBy;

        _currentPlaylistView.IsVisible = false;
        _currentPlaylistView.BackToPlaylists += OnBackToPlaylists;
        _currentPlaylistView.PlaySongPart += OnPlaySongPart;

        AudioPlayerControl.Pause += OnPause;
        AudioPlayerControl.ShowDetails += OnOpenSongPartDetailBottomSheet;
        AudioPlayerControl.UpdateProgress += OnUpdateAudioSlider;
    }

    private void InitializePageContainers()
    {
        AddViewToContainerIfNotExists(LibraryContainer, _currentPlaylistView);
        AddViewToContainerIfNotExists(LibraryContainer, LibraryView);
        AddViewToContainerIfNotExists(HomeContainer, HomeView);
        AddViewToContainerIfNotExists(HomeContainer, _homeRpdPlaylistView);
        AddViewToContainerIfNotExists(HomeContainer, _homeCategoriesView);
    }

    private void AddViewToContainerIfNotExists(Layout container, View view)
    {
        if (!container.Children.Contains(view))
        {
            container.Children.Add(view);
        }
    }

    /// <remarks> iOS needs to run this on the main thread. </remarks>
    private void KeepScreenOn()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            DeviceDisplay.Current.KeepScreenOn = true;
        });
    }

    internal void BackPressed() => BackToHomeView();

    private async void OnFavoriteSongPart(object? sender, EventArgs e)
    {
        if (_currentPlaylistView.IsVisible)
        {
            _currentPlaylistView.RefreshCurrentPlaylist();
            _currentPlaylistView.SavePlaylistButtonClicked(sender, e);
        }
        else
        {
            await LibraryView.LoadPlaylists(isDirty: true);
        }
    }

    #region Toolbar

    private void MainContainerTabItemTapped(object sender, Syncfusion.Maui.TabView.TabItemTappedEventArgs e)
    {
        if (HomeTabItem == e.TabItem)
        {
            SetupHomeToolbar();
            // TODO: isBusy check

            if (_homeCategoriesView.IsVisible)
            {
                _homeCategoriesView.Init();
            }
            else if (_homeRpdPlaylistView.IsVisible)
            {
                _homeRpdPlaylistView.UpdateModeControls();
            }
            else
            {
                _ = HomeView.Init();
            }
        }
        else if (SearchTabItem == e.TabItem)
        {
            SetupSearchToolbar();
            // TODO: isBusy check
            SearchSongPartsView.Init();
        }
        else if (LibraryTabItem == e.TabItem)
        {
            SetupLibraryOrCurrentPlaylistToolbar();
            // Optionally initialize LibraryView or CurrentPlaylistView here
        }
    }

    internal void SetupHomeToolbar()
    {
        if (HomeView.IsVisible) { Title = "Home"; }
        else if (_homeCategoriesView.IsVisible) { Title = "Categories"; }
        else if (_homeRpdPlaylistView.IsVisible) { Title = RpdSettings.UsingGeneratePlaylist ? "Generate playlist" : "Start RPD"; }

        ToolbarItems.Clear();
        AddToolbarItem(IconManager.ToolbarRateReviewIcon, HomeView.FeedbackButtonPressed, 0);
        AddToolbarItem(IconManager.ToolbarSettingsIcon, HomeView.SettingsButtonPressed, 1);
    }

    internal void SetupSearchToolbar(object? sender = null, EventArgs? e = null)
    {
        Title = GetSearchFilterModeText();

        ToolbarItems.Clear();

        AddToolbarItem(IconManager.ToolbarMoreItemsIcon, ShowSecondaryToolbarItems, 3);
    }

    private string GetSearchFilterModeText()
    {
        return AppState.SearchFilterMode switch
        {
            SearchFilterModeValue.All => "All songs",
            SearchFilterModeValue.DanceVideos => "Dance videos",
            SearchFilterModeValue.Male => "Boy(groups)",
            SearchFilterModeValue.Female => "Girl(groups)",
            SearchFilterModeValue.Hybe => "Hybe Labels",
            SearchFilterModeValue.YG => "YG Entertainment",
            SearchFilterModeValue.JYP => "JYP Entertainment",
            SearchFilterModeValue.SM => "SM Entertainment",
            SearchFilterModeValue.Cube => "Cube Entertainment",
            SearchFilterModeValue.FNC => "FNC Entertainment",
            SearchFilterModeValue.Pledis => "Pledis Entertainment",
            SearchFilterModeValue.Starship => "Starship Entertainment",
            SearchFilterModeValue.RBW => "RBW Entertainment",
            SearchFilterModeValue.Woollim => "Woollim Entertainment",
            SearchFilterModeValue.IST => "IST Entertainment",
            SearchFilterModeValue.CJ_ENM_Music => "CJ ENM Music",
            SearchFilterModeValue.Kakao_Entertainment => "Kakao Entertainment",
            SearchFilterModeValue.Firstgen => "1st gen (< 2002)",
            SearchFilterModeValue.Secondgen => "2nd gen (2003-2012)",
            SearchFilterModeValue.Thirdgen => "3rd gen (2012-2017)",
            SearchFilterModeValue.Fourthgen => "4th gen (2018-2022)",
            SearchFilterModeValue.Fifthgen => "5th gen (2023 >)",
            SearchFilterModeValue.Kpop => "Korean pop",
            SearchFilterModeValue.Jpop => "Japanese pop",
            SearchFilterModeValue.EN => "English pop",
            SearchFilterModeValue.Cpop => "Chinese pop",
            SearchFilterModeValue.Tpop => "Thai pop",
            SearchFilterModeValue.Solo => "Solo artists",
            SearchFilterModeValue.Duo => "Duos",
            SearchFilterModeValue.Trio => "Trios",
            SearchFilterModeValue.Quadruplet => "Quadruplets",
            SearchFilterModeValue.Quintet => "Quintets",
            SearchFilterModeValue.Sextet => "Sextets",
            SearchFilterModeValue.Septet => "Septets",
            SearchFilterModeValue.Octet => "Octets",
            SearchFilterModeValue.Nonet => "Nonets",
            SearchFilterModeValue.Group => "Groups (2+ members)",
            SearchFilterModeValue.KpopSoonerThan2012 => "K-pop < 2012",
            SearchFilterModeValue.kpop2012 => "K-pop 2012",
            SearchFilterModeValue.kpop2013 => "K-pop 2013",
            SearchFilterModeValue.kpop2014 => "K-pop 2014",
            SearchFilterModeValue.kpop2015 => "K-pop 2015",
            SearchFilterModeValue.kpop2016 => "K-pop 2016",
            SearchFilterModeValue.kpop2017 => "K-pop 2017",
            SearchFilterModeValue.kpop2018 => "K-pop 2018",
            SearchFilterModeValue.kpop2019 => "K-pop 2019",
            SearchFilterModeValue.kpop2020 => "K-pop 2020",
            SearchFilterModeValue.kpop2021 => "K-pop 2021",
            SearchFilterModeValue.kpop2022 => "K-pop 2022",
            SearchFilterModeValue.kpop2023 => "K-pop 2023",
            SearchFilterModeValue.kpop2024 => "K-pop 2024",
            SearchFilterModeValue.kpop2025 => "K-pop 2025",
            _ => "Unknown"
        };
    }

    /// <remarks> Workaround since secondary items are broken... </remarks>
    /// <param name="sender"> </param>
    /// <param name="e"> </param>
    internal void ShowSecondaryToolbarItems(object? sender = null, EventArgs? e = null)
    {
        Title = string.Empty;
        ToolbarItems.Clear();

        AddToolbarItem(IconManager.ToolbarBackIcon, SetupSearchToolbar, 10, ToolbarItemOrder.Primary);
        AddToolbarItem(IconManager.ToolbarCasinoIcon, SearchSongPartsView.PlayRandomButtonClicked, 11);
        AddToolbarItem(AppState.UsingVideoMode ? IconManager.ToolbarVideoIcon : IconManager.ToolbarVideoOffIcon, SearchSongPartsView.ToggleAudioModeButtonClicked, 12);
        AddToolbarItem(IconManager.ToolbarCollapseAllIcon, SearchSongPartsView.CollapseAllButtonClicked, 20, ToolbarItemOrder.Primary);
        AddToolbarItem(IconManager.ToolbarExpandAllIcon, SearchSongPartsView.ExpandAllButtonClicked, 21, ToolbarItemOrder.Primary);
        AddToolbarItem(IconManager.ToolbarSortIcon, SearchSongPartsView.SortButtonClicked, 30);
    }

    internal void SetupLibraryOrCurrentPlaylistToolbar()
    {
        ToolbarItems.Clear();

        if (LibraryView.IsVisible)
        {
            Title = "Playlists";
            AddToolbarItem(IconManager.ToolbarAddIcon, LibraryView.NewPlaylistButtonClicked, 1);
            AddToolbarItem(IconManager.ToolbarRefreshIcon, LibraryView.RefreshPlaylistsButtonClicked, 1);
        }
        else // CurrentPlaylist is visible.
        {
            Title = CurrentPlaylistManager.Instance.ChosenPlaylist!.Name;
            AddToolbarItem(IconManager.ToolbarPlayIcon, _currentPlaylistView.PlayPlaylistButtonClicked, 1);

            _currentPlaylistView.InitializeView();
        }
    }

    private void AddToolbarItem(ImageSource icon, EventHandler clicked, int priority, ToolbarItemOrder order = ToolbarItemOrder.Default)
    {
        var toolbarItem = new ToolbarItem
        {
            Text = "",
            IconImageSource = icon,
            Order = order,
            Priority = priority
        };
        toolbarItem.Clicked += clicked;
        ToolbarItems.Add(toolbarItem);
    }

    #endregion Toolbar

    internal void OnInitSongParts(object? sender, EventArgs e) => SearchSongPartsView.InitSongParts();

    private void OnPlayToggleSongPart(object? sender, EventArgs e) => AudioPlayerControl.PlayToggleButtonPressed(sender!, e);

    private void OnPreviousSong(object? sender, EventArgs e) => AudioPlayerControl.PlayPreviousSongPart(sender!, e);

    internal void OnNextSong(object? sender, EventArgs e) => AudioPlayerControl.NextButtonPressed(sender!, e);

    private async void OnOpenSongPartDetailBottomSheet(object? sender, EventArgs e)
    {
        if (AppState.CurrentSongPart is null || string.IsNullOrWhiteSpace(AppState.CurrentSongPart.Title)) { return; }
        if (_detailBottomSheet is not null) { return; }

        SongSegmentDetailPopup popup = new(AppState.CurrentSongPart, this);

        popup.AudioPlayerControl = AudioPlayerControl;
        popup.PlayToggleSongPart += OnPlayToggleSongPart;
        popup.PreviousSongPart += OnPreviousSong;
        popup.NextSongPart += OnNextSong;
        popup.FavoriteSongPart += OnFavoriteSongPart;
        popup.isShown = true;

        _detailBottomSheet = popup;
        await this.ShowPopupAsync(popup);

        _detailBottomSheet = null;
    }

    /// <summary> Updates whatever when there is a new song enqueued. </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnEnqueueSongPart(object? sender, EventArgs e) { }

    private void OnFilterPressed(object? sender, EventArgs e)
    {
        SearchSongPartsView.SetFilterMode();
        SearchSongPartsView.RefreshSort();
        MainContainer.SelectedIndex = 1;
    }

    private void OnCreatePlaylistButtonPressed(object? sender, EventArgs e)
    {
        MainContainer.SelectedIndex = 2;
        ShowPlaylistView();
    }

    private void OnAddSongPart(object? sender, EventArgs e) => _currentPlaylistView.RefreshCurrentPlaylist();

    internal void ShowHomeCategories()
    {
        HomeView = (HomeView)HomeContainer.Children[0];
        HomeView.IsVisible = false;
        _homeRpdPlaylistView.IsVisible = false;
        _homeCategoriesView.IsVisible = true;
        Title = "Categories";

        _homeCategoriesView.Init();
    }

    internal async Task ShowRpdPlaylistView()
    {
        HomeView = (HomeView)HomeContainer.Children[0];
        HomeView.IsVisible = false;
        _homeCategoriesView.IsVisible = false;
        _homeRpdPlaylistView.IsVisible = true;

        await _homeRpdPlaylistView.Init();
        _homeRpdPlaylistView.UpdateModeControls();
    }
    // Not used
    private void OnBackToHomeView(object? sender, EventArgs e) => BackToHomeView();

    private void BackToHomeView()
    {
        _homeCategoriesView.IsVisible = false;
        _homeRpdPlaylistView.IsVisible = false;
        HomeView.IsVisible = true;
        Title = "Home";

        HomeView.UpdateNewsBadge([]);
    }

    private async void OnBackToPlaylists(object? sender, EventArgs e) => await BackToPlaylists();

    private async Task BackToPlaylists()
    {
        _currentPlaylistView.ResetCurrentPlaylist();
        await LibraryView!.LoadPlaylists(isDirty: true);

        _currentPlaylistView.IsVisible = false;
        LibraryView.IsVisible = true;

        SetupLibraryOrCurrentPlaylistToolbar();
    }

    internal void ShowPlaylistView()
    {
        LibraryView = (LibraryView)LibraryContainer.Children[0];
        LibraryView.IsVisible = false;
        _currentPlaylistView.IsVisible = true;
        _currentPlaylistView.InitializeView();

        SetupLibraryOrCurrentPlaylistToolbar();
        _currentPlaylistView.RefreshCurrentPlaylist();
    }

    private void OnPlaySongPart(object? sender, EventArgs e)
    {
        if (AppState.CurrentSongPart.Id < 0) { return; }

        switch (AppState.PlayMode)
        {
            case PlayModeValue.Playlist:
                if (CurrentPlaylistManager.Instance.CurrentlyPlayingPlaylist is not null)
                {

                }
                break;

            case PlayModeValue.Queue:
                if (SearchSongPartsView.songParts is not null && SearchSongPartsView.songParts.Count > 0)
                {
                    // TODO: Based on song history?
                    foreach (var songPart in SearchSongPartsView.songParts)
                    {
                        songPart.IsPlaying = false;
                    }
                }
                break;
        }
    }

    private async void OnShowSortBy(object? sender, EventArgs e)
    {
        if (_sortByBottomSheet is not null) { return; }

        var popup = new SortByPopup();

        popup.isShown = true;

        _sortByBottomSheet = popup;

        await this.ShowPopupAsync(popup);
        _sortByBottomSheet = null;

        // TODO: If changed
        SearchSongPartsView.RefreshSort();
    }

    private async void OnShowNewsPopup(object? sender, EventArgs e)
    {
        var popup = new NewsPopup(); // Gets disposed on close.
        popup.NewsItems.AddRange(NewsManager.SongPartsDifference);
        await this.ShowPopupAsync(popup);
    }

    internal void HandleAutoStartRpd()
    {
        if (Preferences.ContainsKey(CommonSettings.START_RPD_AUTOMATIC))
        {
            bool startRpd = Preferences.Get(CommonSettings.START_RPD_AUTOMATIC, false);
            if (startRpd) { _ = ShowRpdPlaylistView(); }
        }
    }

    #region AudioPlayerControl Events

    private void OnPause(object? sender, EventArgs e) => SearchSongPartsView.songParts.ToList().ForEach(s => s.IsPlaying = false);

    private void OnUpdateAudioSlider(object? sender, EventArgs e)
    {
        double progressPercentage = AudioPlayerControl.AudioSlider!.Value; // 0.0 - 100.0

        if (_detailBottomSheet is not null && _detailBottomSheet.isShown)
        {
            _detailBottomSheet.UpdateAudioProgress(progressPercentage);
        }
        SongPart current = AppState.CurrentSongPart;

        // Update playlistSlider if visible.
        if (_currentPlaylistView.IsVisible && (byte)MainContainer.SelectedIndex == 2 && CurrentPlaylistManager.Instance.ChosenPlaylist == CurrentPlaylistManager.Instance.CurrentlyPlayingPlaylist)
        {
            var percentage = progressPercentage / 100;
            var secondsIntoSegment = (percentage * current.ClipLength);
            var currentSecondsIntoPlaylist = current.PlaylistStartTime.TotalSeconds + secondsIntoSegment;

            var playlistProgressValue = currentSecondsIntoPlaylist / CurrentPlaylistManager.Instance.CurrentlyPlayingPlaylist!.LengthInSeconds * 100.0;
            _currentPlaylistView.ProgressSlider.Value = playlistProgressValue;
            _currentPlaylistView.Progress = TimeSpan.FromSeconds(currentSecondsIntoPlaylist);
            _currentPlaylistView.RefreshProgress();
        }
    }

    #endregion AudioPlayerControl Events

    protected override bool OnBackButtonPressed()
    {
        if (_detailBottomSheet is not null && _detailBottomSheet.isShown)
        {
            _detailBottomSheet.Close();
            _detailBottomSheet = null;
            return true;
        }
        else if (_sortByBottomSheet is not null && _sortByBottomSheet.isShown)
        {
            _sortByBottomSheet.Close();
            _sortByBottomSheet = null;
            return true;
        }
        // Home view navigation.
        else if (_homeRpdPlaylistView.IsVisible && (byte)MainContainer.SelectedIndex == 0)
        {
            BackToHomeView();
            return true;
        }
        else if (_homeCategoriesView.IsVisible && (byte)MainContainer.SelectedIndex == 0)
        {
            BackToHomeView();
            return true;
        }
        // Library navigation.
        else if (_currentPlaylistView.IsVisible && (byte)MainContainer.SelectedIndex == 2)
        {
            _currentPlaylistView.BackImageButtonClicked(null, EventArgs.Empty);
            return true;
        }
        else if ((byte)MainContainer.SelectedIndex == 1 || (byte)MainContainer.SelectedIndex == 2)
        {
            // Go to homeview.
            MainContainer.SelectedIndex = 0;

            // Check what was visible on the homeview.
            if (_homeCategoriesView.IsVisible) { ShowHomeCategories(); }
            else if (_homeRpdPlaylistView.IsVisible) { _ = ShowRpdPlaylistView(); }
            else { BackToHomeView(); }

            SetupHomeToolbar();

            return true;
        }
        else
        {
            base.OnBackButtonPressed();
            return false;
        }
    }
}