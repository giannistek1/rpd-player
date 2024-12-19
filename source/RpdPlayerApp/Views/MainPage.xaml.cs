using RpdPlayerApp.Architecture;
using RpdPlayerApp.Models;
using RpdPlayerApp.ViewModels;

namespace RpdPlayerApp.Views;

public partial class MainPage
{
    private readonly CurrentPlaylistView _currentPlaylistView = new();
    private readonly SortByBottomSheet _sortByBottomSheet = new();
    private readonly SongPartDetailBottomSheet _detailBottomSheet = new();

    // TODO: Settings class
    private const string MAIN_VOLUME = "MAIN_VOLUME";

    public MainPage()
    {
        InitializeComponent();

        HomeView.ParentPage = this;
        SearchSongPartsView.ParentPage = this;
        LibraryView.ParentPage = this;
        _currentPlaylistView.ParentPage = this;

        // Load settings.
        if (Preferences.ContainsKey(MAIN_VOLUME))
        {
            MainViewModel.MainVolume = Preferences.Get(key: MAIN_VOLUME, 1.0);
        }

        this.Appearing += OnAppearing;

        SetContentViewEvents();

        if (!LibraryContainer.Children.Contains(_currentPlaylistView))
        {
            LibraryContainer.Children.Add(_currentPlaylistView);
        }
        if (!LibraryContainer.Children.Contains(LibraryView))
        {
            LibraryContainer.Children.Add(LibraryView);
        }

        // Has to be run on MainThread (UI thread) for iOS.
        MainThread.BeginInvokeOnMainThread(() =>
        {
            DeviceDisplay.Current.KeepScreenOn = true;
        });

        SetupHomeToolbar();
    }

    private void OnAppearing(object? sender, EventArgs e)
    {
        AudioPlayerControl.UpdateUI();
    }

    private void SetContentViewEvents()
    {
        HomeView.FilterPressed += OnFilterPressed;

        SearchSongPartsView.PlaySongPart += OnPlaySongPart;
        SearchSongPartsView.StopSongPart += OnStopSongPart;
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
        _detailBottomSheet.PreviousSong += OnPreviousSong;
        _detailBottomSheet.NextSong += OnNextSong;
        _detailBottomSheet.Close += OnCloseDetailSheet;

        AudioPlayerControl.Pause += OnPause;
        AudioPlayerControl.ShowDetails += OnShowDetails;
        AudioPlayerControl.UpdateProgress += OnUpdateProgress;
        AudioPlayerControl.AudioEnded += OnAudioEnded;
    }

    #region Toolbar

    private void MainContainerTabItemTapped(object sender, Syncfusion.Maui.TabView.TabItemTappedEventArgs e)
    {
        // Can't make a switch out of this. A constant value of type SfTabItem is needed.
        if      (HomeTabItem == e.TabItem)      { SetupHomeToolbar(); }
        else if (SearchTabItem == e.TabItem)    { SetupSearchToolbar(); }
        else if (LibraryTabItem == e.TabItem)   { SetupLibraryToolbar(); }
    }

    internal void SetupHomeToolbar()
    {
        Title = "Home";

        ToolbarItems.Clear();

        ToolbarItem feedbackToolbarItem = new()
        {
            Text = "",
            IconImageSource = IconManager.ToolbarRateReviewIcon,
            Order = ToolbarItemOrder.Default, // Primary or Secondary
            Priority = 0
        };
        feedbackToolbarItem.Clicked += HomeView.FeedbackButtonPressed;
        ToolbarItems.Add(feedbackToolbarItem);

        ToolbarItem settingsToolbarItem = new()
        {
            Text = "",
            IconImageSource = IconManager.ToolbarSettingsIcon,
            Order = ToolbarItemOrder.Default, // Primary or Secondary
            Priority = 1
        };
        settingsToolbarItem.Clicked += HomeView.SettingsButtonPressed;
        ToolbarItems.Add(settingsToolbarItem);
    }
    /// <summary>
    /// Setups title and toolbar items or refreshes the toolbar.
    /// </summary>
    internal void SetupSearchToolbar(object? sender = null, EventArgs? e = null)
    {
        ToolbarItems.Clear();

        MainViewModel.SearchFilterModeText = (MainViewModel.SearchFilterMode) switch
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
            SearchFilterMode.Secondgen => "2nd gen (2003..12)",
            SearchFilterMode.Thirdgen => "3rd gen (2012..17)",
            SearchFilterMode.Fourthgen => "4th gen (2018..22)",
            SearchFilterMode.Fifthgen => "5th gen (2023 >)",

            SearchFilterMode.KR => "K-pop",
            SearchFilterMode.JP => "J-pop",
            SearchFilterMode.EN => "English pop",
            SearchFilterMode.CH => "C-pop",
            SearchFilterMode.TH => "T-pop",

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
            _ => "Unknown"
        };

        Title = MainViewModel.SearchFilterModeText;

        ToolbarItem moreItemsToolbarItem = new()
        {
            Text = "",
            IconImageSource = IconManager.ToolbarMoreItemsIcon,
            Order = ToolbarItemOrder.Default, // Secondary does not create a vertical dots icon on iOS.
            Priority = 3
        };
        moreItemsToolbarItem.Clicked += ShowSecondaryToolbarItems;
        ToolbarItems.Add(moreItemsToolbarItem);
    }

    // iOS secondary workaround until MAUI team fixes this
    internal void ShowSecondaryToolbarItems(object? sender = null, EventArgs? e = null)
    {
        Title = string.Empty;

        ToolbarItems.Clear();

        ToolbarItem backToolbarItem = new()
        {
            Text = "",
            IconImageSource = IconManager.ToolbarBackIcon,
            Order = ToolbarItemOrder.Primary,
            Priority = 10
        };
        backToolbarItem.Clicked += SetupSearchToolbar;
        ToolbarItems.Add(backToolbarItem);

        ToolbarItem playOrAddRandomToolbarItem = new()
        {
            Text = "",
            IconImageSource = IconManager.ToolbarCasinoIcon,
            Order = ToolbarItemOrder.Default, // Primary or Secondary
            Priority = 11
        };
        playOrAddRandomToolbarItem.Clicked += SearchSongPartsView.PlayRandomButtonClicked;
        ToolbarItems.Add(playOrAddRandomToolbarItem);

        ToolbarItem videoModeToolbarItem = new()
        {
            Text = "",
            IconImageSource = (MainViewModel.UsingVideoMode ? IconManager.ToolbarVideoIcon : IconManager.ToolbarVideoOffIcon),
            Order = ToolbarItemOrder.Default,
            Priority = 12
        };
        videoModeToolbarItem.Clicked += SearchSongPartsView.ToggleAudioModeButtonClicked;
        ToolbarItems.Add(videoModeToolbarItem);

        ToolbarItem collapseAllToolbarItem = new()
        {
            Text = "",
            IconImageSource = IconManager.ToolbarCollapseAllIcon,
            Order = ToolbarItemOrder.Primary,
            Priority = 20
        };
        collapseAllToolbarItem.Clicked += SearchSongPartsView.CollapseAllButtonClicked;
        ToolbarItems.Add(collapseAllToolbarItem);

        ToolbarItem expandAllToolbarItem = new()
        {
            Text = "",
            IconImageSource = IconManager.ToolbarExpandAllIcon,
            Order = ToolbarItemOrder.Primary,
            Priority = 21
        };
        expandAllToolbarItem.Clicked += SearchSongPartsView.ExpandAllButtonClicked;
        ToolbarItems.Add(expandAllToolbarItem);

        ToolbarItem sortToolbarItem = new()
        {
            Text = "",
            IconImageSource = IconManager.ToolbarSortIcon,
            Order = ToolbarItemOrder.Default,
            Priority = 30
        };
        sortToolbarItem.Clicked += SearchSongPartsView.SortButtonClicked;
        ToolbarItems.Add(sortToolbarItem);
    }

    internal void SetupLibraryToolbar()
    {
        ToolbarItems.Clear();

        if (LibraryView.IsVisible)
        {
            Title = "Playlists";
            ToolbarItem newPlaylistToolbarItem = new()
            {
                Text = "",
                IconImageSource = IconManager.ToolbarAddIcon,
                Order = ToolbarItemOrder.Default, // Primary or Secondary
                Priority = 1
            };
            newPlaylistToolbarItem.Clicked += LibraryView.NewPlaylistButtonClicked;
            ToolbarItems.Add(newPlaylistToolbarItem);
        }
        else
        {
            Title = PlaylistManager.Instance.CurrentPlaylist.Name;
            ToolbarItem playPlaylistToolbarItem = new()
            {
                Text = "",
                IconImageSource = IconManager.ToolbarPlayIcon,
                Order = ToolbarItemOrder.Default, // Primary or Secondary
                Priority = 1
            };
            playPlaylistToolbarItem.Clicked += _currentPlaylistView.PlayPlaylistButtonClicked;
            ToolbarItems.Add(playPlaylistToolbarItem);

            ToolbarItem savePlaylistToolbarItem = new()
            {
                Text = "",
                IconImageSource = IconManager.ToolbarSaveIcon,
                Order = ToolbarItemOrder.Default, // Primary or Secondary
                Priority = 2
            };
            savePlaylistToolbarItem.Clicked += _currentPlaylistView.SavePlaylistButtonClicked;
            ToolbarItems.Add(savePlaylistToolbarItem);

            ToolbarItem clearToolbarItem = new()
            {
                Text = "",
                IconImageSource = IconManager.ToolbarClearIcon,
                Order = ToolbarItemOrder.Default, // Primary or Secondary
                Priority = 3
            };
            clearToolbarItem.Clicked += _currentPlaylistView.ClearPlaylistButtonClicked;
            ToolbarItems.Add(clearToolbarItem);

            ToolbarItem cloudToolbarItem = new()
            {
                Text = "",
                IconImageSource = MainViewModel.UsingCloudMode ? IconManager.ToolbarCloudIcon : IconManager.ToolbarCloudOffIcon,
                Order = ToolbarItemOrder.Default, // Primary or Secondary
                Priority = 4
            };
            cloudToolbarItem.Clicked += _currentPlaylistView.ToggleCloudModePressed;
            ToolbarItems.Add(cloudToolbarItem);
        }
    }

    #endregion

    private void OnPlayToggleSongPart(object? sender, EventArgs e)
    {
        AudioPlayerControl.PlayToggleButton_Pressed(sender!, e);
    }

    private void OnPreviousSong(object? sender, EventArgs e)
    {
        AudioPlayerControl.PlayPreviousSongPart(sender!, e);

        _detailBottomSheet.songPart = MainViewModel.CurrentSongPart;
        _detailBottomSheet.UpdateUI();
    }

    private void OnNextSong(object? sender, EventArgs e)
    {
        AudioPlayerControl.NextButton_Pressed(sender!, e);

        _detailBottomSheet.songPart = MainViewModel.CurrentSongPart;
        _detailBottomSheet.UpdateUI();
    }

    private void OnCloseDetailSheet(object? sender, EventArgs e)
    {
        _detailBottomSheet.DismissAsync();
    }

    private void OnCloseSortBySheet(object? sender, EventArgs e)
    {
        _sortByBottomSheet.DismissAsync();
        SearchSongPartsView.RefreshSort();
    }

    private void OnEnqueueSongPart(object? sender, EventArgs e)
    {
        AudioPlayerControl.UpdateNextSwipeItem();
    }

    private void OnFilterPressed(object? sender, EventArgs e)
    {
        SearchSongPartsView.SetFilterMode();
        SearchSongPartsView.RefreshSort();
        MainContainer.SelectedIndex = 1;
    }

    private void OnAddSongPart(object? sender, EventArgs e)
    {
        _currentPlaylistView.RefreshCurrentPlaylist();
    }

    private void OnBackToPlaylists(object? sender, EventArgs e)
    {
        BackToPlaylists();
    }

    private void BackToPlaylists()
    {
        _currentPlaylistView.ResetCurrentPlaylist();
        LibraryView!.LoadPlaylists();

        _currentPlaylistView.IsVisible = false;
        LibraryView.IsVisible = true;

        SetupLibraryToolbar();
    }

    private void OnShowPlaylist(object? sender, EventArgs e)
    {
        LibraryView = (LibraryView)LibraryContainer.Children[0];
        LibraryView.IsVisible = false;
        _currentPlaylistView.IsVisible = true;

        SetupLibraryToolbar();
        _currentPlaylistView.RefreshCurrentPlaylist();
    }

    // Used by searchsongpartsView, currentplaylistView
    private void OnPlaySongPart(object? sender, EventArgs e)
    {
        if (MainViewModel.CurrentSongPart.Id < 0) { return; }

        switch (MainViewModel.PlayMode)
        {
            case PlayMode.Playlist:
                if (PlaylistManager.Instance.CurrentPlaylist is not null && SearchSongPartsView.songParts.Count > 0)
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

        AudioPlayerControl.PlayAudio(MainViewModel.CurrentSongPart);
    }

    private void OnStopSongPart(object? sender, EventArgs e)
    {
        if (MainViewModel.CurrentSongPart.Id > 0)
        {
            AudioPlayerControl.StopAudio();
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

    #region AudioPlayerControl Events
    private void OnShowDetails(object? sender, EventArgs e)
    {
        _detailBottomSheet.songPart = MainViewModel.CurrentSongPart;
        _detailBottomSheet.UpdateUI();

        _detailBottomSheet.HasHandle = true;
        _detailBottomSheet.IsCancelable = true;
        _detailBottomSheet.HasBackdrop = true;
        _detailBottomSheet.isShown = true;
        _detailBottomSheet.ShowAsync();
    }
    private void OnPause(object? sender, EventArgs e)
    {
        SearchSongPartsView.songParts.ToList().ForEach(s => s.IsPlaying = false);
    }
    private void OnAudioEnded(object? sender, EventArgs e)
    {
        _detailBottomSheet.songPart = MainViewModel.CurrentSongPart;
        _detailBottomSheet.UpdateUI();
    }

    private void OnUpdateProgress(object? sender, EventArgs e)
    {
        _detailBottomSheet.UpdateProgress(AudioPlayerControl.audioProgressSlider.Value);
    }
    #endregion

    protected override bool OnBackButtonPressed()
    {
        if (_currentPlaylistView.IsVisible)
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
        else
        {
            base.OnBackButtonPressed();
            return false;
        }
    }
}