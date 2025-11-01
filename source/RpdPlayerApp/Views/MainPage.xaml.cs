using CommunityToolkit.Maui.Views;
using RpdPlayerApp.Architecture;
using RpdPlayerApp.Enums;
using RpdPlayerApp.Managers;
using RpdPlayerApp.Models;
using RpdPlayerApp.Services;

namespace RpdPlayerApp.Views;

public partial class MainPage
{
    private readonly CategoriesView _categoriesView = new(); // Home
    private readonly CurrentPlaylistView _currentPlaylistView = new(); // Playlists
    private readonly SortByBottomSheet _sortByBottomSheet = new();
    private readonly SongPartDetailBottomSheet _detailBottomSheet = new();

    public MainPage()
    {
        InitializeComponent();

        BindingContext = DebugService.Instance;

        CommonSettings.ActivityTimeStopWatch.Start();

        AudioManager.DetailBottomSheet = _detailBottomSheet;
        AudioPlayerControl.CurrentPlaylistViewModel = _currentPlaylistView._viewModel;

        SetupHomeToolbar();
        Loaded += OnLoad;
        Appearing += OnAppearing;
    }

    private void OnLoad(object? sender, EventArgs e)
    {
        LoadSettings();
        SetParentPages();
        SetContentViewEvents();
        InitializePageContainers();
        KeepScreenOn();
    }

    private void OnAppearing(object? sender, EventArgs e) => AudioPlayerControl.UpdateUI();

    private async void LoadSettings()
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
        SearchSongPartsView.ParentPage = this;
        LibraryView.ParentPage = this;
        _currentPlaylistView.ParentPage = this;
        _detailBottomSheet.AudioPlayerControl = AudioPlayerControl;
    }

    private void SetContentViewEvents()
    {
        //HomeView.PlaySongPart += OnPlaySongPart;
        HomeView.CreatePlaylistButtonPressed += OnCreatePlaylistButtonPressed;
        HomeView.ShowCategories += OnShowCategories;
        HomeView.ShowNewsPopup += OnShowNewsPopup;

        _categoriesView.IsVisible = false;
        _categoriesView.FilterPressed += OnFilterPressed;
        _categoriesView.BackPressed += OnBackPressed;

        SearchSongPartsView.PlaySongPart += OnPlaySongPart;
        SearchSongPartsView.AddSongPart += OnAddSongPart;
        SearchSongPartsView.EnqueueSongPart += OnEnqueueSongPart;
        SearchSongPartsView.ShowSortBy += OnShowSortBy;

        LibraryView.PlayPlaylist += OnPlaySongPart; // Not used
        LibraryView.ShowPlaylist += OnShowPlaylist;

        _currentPlaylistView.IsVisible = false;
        _currentPlaylistView.BackToPlaylists += OnBackToPlaylists;
        _currentPlaylistView.PlaySongPart += OnPlaySongPart;

        _sortByBottomSheet.Close += OnCloseSortBySheet;

        _detailBottomSheet.PlayToggleSongPart += OnPlayToggleSongPart;
        _detailBottomSheet.PreviousSongPart += OnPreviousSong;
        _detailBottomSheet.NextSongPart += OnNextSong;
        _detailBottomSheet.Close += OnCloseDetailSheet;
        _detailBottomSheet.FavoriteSongPart += OnFavoriteSongPart;

        AudioPlayerControl.Pause += OnPause;
        AudioPlayerControl.ShowDetails += OnOpenSongPartDetailBottomSheet;
        AudioPlayerControl.UpdateProgress += OnUpdateAudioSlider;
    }

    private void InitializePageContainers()
    {
        AddViewToContainerIfNotExists(LibraryContainer, _currentPlaylistView);
        AddViewToContainerIfNotExists(LibraryContainer, LibraryView);
        AddViewToContainerIfNotExists(HomeContainer, HomeView);
        AddViewToContainerIfNotExists(HomeContainer, _categoriesView);
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

    private void OnBackPressed(object? sender, EventArgs e) => BackToHomeView();

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
        if (HomeTabItem == e.TabItem) { SetupHomeToolbar(); }
        else if (SearchTabItem == e.TabItem) { SetupSearchToolbar(); }
        else if (LibraryTabItem == e.TabItem) { SetupLibraryOrCurrentPlaylistToolbar(); }
    }

    internal void SetupHomeToolbar()
    {
        Title = HomeView.IsVisible ? "Home" : "Categories";
        ToolbarItems.Clear();

        AddToolbarItem(IconManager.ToolbarRateReviewIcon, HomeView.FeedbackButtonPressed, 0);
        AddToolbarItem(IconManager.ToolbarSettingsIcon, HomeView.SettingsButtonPressed, 1);
    }

    internal void SetupSearchToolbar(object? sender = null, EventArgs? e = null)
    {
        ToolbarItems.Clear();
        Title = GetSearchFilterModeText();

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
            Title = CurrentPlaylistManager.Instance.ChosenPlaylist.Name;
            AddToolbarItem(IconManager.ToolbarPlayIcon, _currentPlaylistView.PlayPlaylistButtonClicked, 1);
            //AddToolbarItem(IconManager.ToolbarSaveIcon, _currentPlaylistView.SavePlaylistButtonClicked, 2);
            AddToolbarItem(IconManager.ToolbarClearIcon, _currentPlaylistView.ClearPlaylistButtonClicked, 3);
            //AddToolbarItem(AppState.UsingCloudMode ? IconManager.ToolbarCloudIcon : IconManager.ToolbarCloudOffIcon, _currentPlaylistView.ToggleCloudModePressed, 4);

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

    private void OnPlayToggleSongPart(object? sender, EventArgs e) => AudioPlayerControl.PlayToggleButtonPressed(sender!, e);

    private void OnPreviousSong(object? sender, EventArgs e) => AudioPlayerControl.PlayPreviousSongPart(sender!, e);

    private void OnNextSong(object? sender, EventArgs e) => AudioPlayerControl.NextButtonPressed(sender!, e);

    private void OnOpenSongPartDetailBottomSheet(object? sender, EventArgs e)
    {
        _detailBottomSheet.songPart = AppState.CurrentSongPart;
        _detailBottomSheet.UpdateSongDetails();
        _detailBottomSheet.UpdateIcons();

        _detailBottomSheet.HasHandle = true;
        _detailBottomSheet.IsCancelable = true;
        _detailBottomSheet.HasBackdrop = true;
        _detailBottomSheet.isShown = true;
        _detailBottomSheet.ShowAsync();
    }

    private void OnCloseDetailSheet(object? sender, EventArgs e) => _detailBottomSheet.DismissAsync();

    private void OnCloseSortBySheet(object? sender, EventArgs e)
    {
        _sortByBottomSheet.DismissAsync();
        SearchSongPartsView.RefreshSort();
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
        SetupLibraryOrCurrentPlaylistToolbar();
        LibraryView.FocusNewPlaylistEntry();
    }

    private void OnAddSongPart(object? sender, EventArgs e) => _currentPlaylistView.RefreshCurrentPlaylist();

    private void OnShowCategories(object? sender, EventArgs e)
    {
        HomeView = (HomeView)HomeContainer.Children[0];
        HomeView.IsVisible = false;
        _categoriesView.IsVisible = true;
        Title = "Categories";
    }
    // Not used
    private void OnBackToHomeView(object? sender, EventArgs e) => BackToHomeView();

    private void BackToHomeView()
    {
        _categoriesView.IsVisible = false;
        HomeView.IsVisible = true;
        Title = "Home";
    }

    private void OnBackToPlaylists(object? sender, EventArgs e) => BackToPlaylists();

    private void BackToPlaylists()
    {
        _currentPlaylistView.ResetCurrentPlaylist();
        LibraryView!.LoadPlaylists();

        _currentPlaylistView.IsVisible = false;
        LibraryView.IsVisible = true;

        SetupLibraryOrCurrentPlaylistToolbar();
    }

    private void OnShowPlaylist(object? sender, EventArgs e)
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

    private void OnShowSortBy(object? sender, EventArgs e)
    {
        _sortByBottomSheet.HasHandle = true;
        _sortByBottomSheet.IsCancelable = true;
        _sortByBottomSheet.HasBackdrop = true;
        _sortByBottomSheet.isShown = true;
        _sortByBottomSheet.ShowAsync();
    }

    private void OnShowNewsPopup(object? sender, EventArgs e)
    {
        var popup = new NewsPopup(); // Gets disposed on close.
        popup.NewsItems.AddRange(NewsManager.SongPartsDifference);
        this.ShowPopup(popup);
    }

    #region AudioPlayerControl Events

    private void OnPause(object? sender, EventArgs e) => SearchSongPartsView.songParts.ToList().ForEach(s => s.IsPlaying = false);

    private void OnUpdateAudioSlider(object? sender, EventArgs e)
    {
        double progressPercentage = AudioPlayerControl.AudioSlider!.Value; // 0.0 - 100.0

        _detailBottomSheet.UpdateAudioProgress(progressPercentage);
        SongPart current = AppState.CurrentSongPart;

        // Update playlistSlider if visible.
        if (_currentPlaylistView.IsVisible && (byte)MainContainer.SelectedIndex == 2)
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
        if (_detailBottomSheet.isShown)
        {
            _detailBottomSheet.DismissAsync();
            return true;
        }
        else if (_sortByBottomSheet.isShown)
        {
            _sortByBottomSheet.DismissAsync();
            return true;
        }
        else if (_categoriesView.IsVisible && (byte)MainContainer.SelectedIndex == 0)
        {
            BackToHomeView();
            return true;
        }
        else if (_currentPlaylistView.IsVisible && (byte)MainContainer.SelectedIndex == 2)
        {
            _currentPlaylistView.BackButtonClicked(null, EventArgs.Empty);
            return true;
        }
        else if ((byte)MainContainer.SelectedIndex == 1 || (byte)MainContainer.SelectedIndex == 2)
        {
            // Go to homeview.
            MainContainer.SelectedIndex = 0;

            // Check what was visible on the homeview.
            if (_categoriesView.IsVisible) { OnShowCategories(null, EventArgs.Empty); }
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