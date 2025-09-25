using CommunityToolkit.Maui.Views;
using RpdPlayerApp.Architecture;
using RpdPlayerApp.Enums;
using RpdPlayerApp.Managers;
using RpdPlayerApp.ViewModels;

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

        DebugService.Instance.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(DebugService.DebugLog))
            {
                // Scroll after UI updates
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await DebugScrollView.ScrollToAsync(0, double.MaxValue, animated: true);
                });
            }
        };

        CommonSettings.ActivityTimeStopWatch.Start();

        AudioManager.DetailBottomSheet = _detailBottomSheet;

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

    private void LoadSettings()
    {
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
        HomeView.PlaySongPart += OnPlaySongPart;
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
        AudioPlayerControl.UpdateProgress += OnUpdateProgress;
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

    private void OnFavoriteSongPart(object? sender, EventArgs e)
    {
        if (_currentPlaylistView.IsVisible)
        {
            _currentPlaylistView.RefreshCurrentPlaylist();
            _currentPlaylistView.SavePlaylistButtonClicked(sender, e);
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
            SearchFilterMode.All => "All songs",
            SearchFilterMode.DanceVideos => "Dance videos",
            SearchFilterMode.Male => "Boy(groups)",
            SearchFilterMode.Female => "Girl(groups)",
            SearchFilterMode.Hybe => "Hybe Labels",
            SearchFilterMode.YG => "YG Entertainment",
            SearchFilterMode.JYP => "JYP Entertainment",
            SearchFilterMode.SM => "SM Entertainment",
            SearchFilterMode.Cube => "Cube Entertainment",
            SearchFilterMode.FNC => "FNC Entertainment",
            SearchFilterMode.Pledis => "Pledis Entertainment",
            SearchFilterMode.Starship => "Starship Entertainment",
            SearchFilterMode.RBW => "RBW Entertainment",
            SearchFilterMode.Woollim => "Woollim Entertainment",
            SearchFilterMode.IST => "IST Entertainment",
            SearchFilterMode.CJ_ENM_Music => "CJ ENM Music",
            SearchFilterMode.Kakao_Entertainment => "Kakao Entertainment",
            SearchFilterMode.Firstgen => "1st gen (< 2002)",
            SearchFilterMode.Secondgen => "2nd gen (2003-2012)",
            SearchFilterMode.Thirdgen => "3rd gen (2012-2017)",
            SearchFilterMode.Fourthgen => "4th gen (2018-2022)",
            SearchFilterMode.Fifthgen => "5th gen (2023 >)",
            SearchFilterMode.Kpop => "Korean pop",
            SearchFilterMode.Jpop => "Japanese pop",
            SearchFilterMode.EN => "English pop",
            SearchFilterMode.Cpop => "Chinese pop",
            SearchFilterMode.Tpop => "Thai pop",
            SearchFilterMode.Solo => "Solo artists",
            SearchFilterMode.Duo => "Duos",
            SearchFilterMode.Trio => "Trios",
            SearchFilterMode.Quadruplet => "Quadruplets",
            SearchFilterMode.Quintet => "Quintets",
            SearchFilterMode.Sextet => "Sextets",
            SearchFilterMode.Septet => "Septets",
            SearchFilterMode.Octet => "Octets",
            SearchFilterMode.Nonet => "Nonets",
            SearchFilterMode.Group => "Groups (2+ members)",
            SearchFilterMode.KpopSoonerThan2012 => "K-pop < 2012",
            SearchFilterMode.kpop2012 => "K-pop 2012",
            SearchFilterMode.kpop2013 => "K-pop 2013",
            SearchFilterMode.kpop2014 => "K-pop 2014",
            SearchFilterMode.kpop2015 => "K-pop 2015",
            SearchFilterMode.kpop2016 => "K-pop 2016",
            SearchFilterMode.kpop2017 => "K-pop 2017",
            SearchFilterMode.kpop2018 => "K-pop 2018",
            SearchFilterMode.kpop2019 => "K-pop 2019",
            SearchFilterMode.kpop2020 => "K-pop 2020",
            SearchFilterMode.kpop2021 => "K-pop 2021",
            SearchFilterMode.kpop2022 => "K-pop 2022",
            SearchFilterMode.kpop2023 => "K-pop 2023",
            SearchFilterMode.kpop2024 => "K-pop 2024",
            SearchFilterMode.kpop2025 => "K-pop 2025",
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
        }
        else // CurrentPlaylist is visible.
        {
            Title = CurrentPlaylistManager.Instance.CurrentPlaylist.Name;
            AddToolbarItem(IconManager.ToolbarPlayIcon, _currentPlaylistView.PlayPlaylistButtonClicked, 1);
            AddToolbarItem(IconManager.ToolbarSaveIcon, _currentPlaylistView.SavePlaylistButtonClicked, 2);
            AddToolbarItem(IconManager.ToolbarClearIcon, _currentPlaylistView.ClearPlaylistButtonClicked, 3);
            AddToolbarItem(AppState.UsingCloudMode ? IconManager.ToolbarCloudIcon : IconManager.ToolbarCloudOffIcon, _currentPlaylistView.ToggleCloudModePressed, 4);

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

    private void OnPlayToggleSongPart(object? sender, EventArgs e) => AudioPlayerControl.PlayToggleButton_Pressed(sender!, e);

    private void OnPreviousSong(object? sender, EventArgs e) => AudioPlayerControl.PlayPreviousSongPart(sender!, e);

    private void OnNextSong(object? sender, EventArgs e) => AudioPlayerControl.NextButton_Pressed(sender!, e);

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

    private void OnEnqueueSongPart(object? sender, EventArgs e) => AudioPlayerControl.UpdateNextSwipeItem();

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
            case PlayMode.Playlist:
                if (CurrentPlaylistManager.Instance.CurrentPlaylist is not null && SearchSongPartsView.songParts.Count > 0)
                {
                    // Nothing
                }
                break;

            case PlayMode.Queue:
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

        if (AppState.TimerMode > 0)
        {
            AudioPlayerControl.PlayCountdownAndUpdateCurrentSong();
        }
        else
        {
            AudioPlayerControl.PlayAudio(AppState.CurrentSongPart, updateCurrentSong: true);
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
        popup.NewsItems.AddRange(AppState.SongPartsDifference);
        this.ShowPopup(popup);
    }

    #region AudioPlayerControl Events

    private void OnPause(object? sender, EventArgs e) => SearchSongPartsView.songParts.ToList().ForEach(s => s.IsPlaying = false);

    private void OnUpdateProgress(object? sender, EventArgs e) => _detailBottomSheet.UpdateProgress(AudioPlayerControl.audioProgressSlider!.Value);

    #endregion AudioPlayerControl Events

    protected override bool OnBackButtonPressed()
    {
        if (_categoriesView.IsVisible && (byte)MainContainer.SelectedIndex == 0)
        {
            BackToHomeView();
            return true;
        }
        else if (_currentPlaylistView.IsVisible && (byte)MainContainer.SelectedIndex == 1)
        {
            BackToPlaylists();
            return true;
        }
        else if (_detailBottomSheet.isShown)
        {
            _detailBottomSheet.DismissAsync();
            return true;
        }
        else if (_sortByBottomSheet.isShown)
        {
            _sortByBottomSheet.DismissAsync();
            return true;
        }
        else if ((byte)MainContainer.SelectedIndex == 1 || (byte)MainContainer.SelectedIndex == 2)
        {
            MainContainer.SelectedIndex = 0;

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