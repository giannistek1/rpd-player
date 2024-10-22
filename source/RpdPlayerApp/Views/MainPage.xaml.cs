using RpdPlayerApp.Architecture;
using RpdPlayerApp.Models;
using RpdPlayerApp.ViewModels;
using UraniumUI.Icons.MaterialSymbols;

namespace RpdPlayerApp.Views;

public partial class MainPage
{
    private readonly CurrentPlaylistView _currentPlaylistView = new();
    private readonly SortByBottomSheet _sortByBottomSheet = new();
    private readonly SongPartDetailBottomSheet _detailBottomSheet = new();

    private FontImageSource _videoOnIcon = new();
    private FontImageSource _videoOffIcon = new();

    public MainPage()
    {
        InitializeComponent();

        _videoOnIcon = new FontImageSource
        {
            FontFamily = "MaterialRegular",
            Glyph = MaterialOutlined.Videocam,
            Color = (Color)Application.Current!.Resources["ToolbarIconColor"] // TODO: Sucks because this only gets set once.
        };

        _videoOffIcon = new FontImageSource
        {
            FontFamily = "MaterialRegular",
            Glyph = MaterialOutlined.Videocam_off,
            Color = (Color)Application.Current!.Resources["ToolbarIconColor"] // TODO: Sucks because this only gets set once.
        };

        HomeView.ParentPage = this;
        SearchSongPartsView.ParentPage = this;
        LibraryView.ParentPage = this;

        SetViewEvents();

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

    private void SetViewEvents()
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

        _sortByBottomSheet.CloseSheet += OnCloseSortBySheet;

        _detailBottomSheet.PlayToggleSongPart += OnPlayToggleSongPart;
        _detailBottomSheet.PreviousSong += OnPreviousSong;
        _detailBottomSheet.NextSong += OnNextSong;

        AudioPlayerControl.Pause += OnPause;
        AudioPlayerControl.ShowDetails += OnShowDetails;
        AudioPlayerControl.UpdateProgress += OnUpdateProgress;
        AudioPlayerControl.AudioEnded += OnAudioEnded;
    }

    #region Toolbar

    private void MainContainerTabItemTapped(object sender, Syncfusion.Maui.TabView.TabItemTappedEventArgs e)
    {
        if (HomeTabItem == e.TabItem)
        {
            SetupHomeToolbar();
        }
        else if (SearchTabItem == e.TabItem)
        {
            SetupSearchToolbar();
        }
        else if (LibraryTabItem == e.TabItem)
        {
            SetupLibraryToolbar();
        }
    }

    private void SetupHomeToolbar()
    {
        ToolbarItems.Clear();

        Title = "Home";

        ToolbarItem feedbackToolbarItem = new ToolbarItem
        {
            Text = "",
            IconImageSource = new FontImageSource() { FontFamily = "MaterialRounded", Glyph = MaterialRounded.Rate_review },
            Order = ToolbarItemOrder.Default, // Primary or Secondary
            Priority = 0 // the lower the number, the higher the priority
        };

        ToolbarItem settingsToolbarItem = new ToolbarItem
        {
            Text = "",
            IconImageSource = new FontImageSource() { FontFamily = "MaterialRounded", Glyph = MaterialRounded.Settings },
            Order = ToolbarItemOrder.Default, // Primary or Secondary
            Priority = 1 // the lower the number, the higher the priority
        };

        feedbackToolbarItem.Clicked += HomeView.FeedbackButtonPressed;
        settingsToolbarItem.Clicked += HomeView.SettingsButtonPressed;

        ToolbarItems.Add(feedbackToolbarItem);
        ToolbarItems.Add(settingsToolbarItem);
    }
    /// <summary>
    /// Setups or refreshes the toolbar.
    /// </summary>
    internal void SetupSearchToolbar()
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

            SearchFilterMode.Firstgen => "First gen (< 2002)",
            SearchFilterMode.Secondgen => "Second gen (2003 - 2012)",
            SearchFilterMode.Thirdgen => "Third gen (2012 - 2017)",
            SearchFilterMode.Fourthgen => "Fourth gen (2018 - 2022)",
            SearchFilterMode.Fifthgen => "Fifth gen (2023 >)",

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

            SearchFilterMode.kpop2019 => "K-pop 2019",
            SearchFilterMode.kpop2020 => "K-pop 2020",
            SearchFilterMode.kpop2021 => "K-pop 2021",
            SearchFilterMode.kpop2022 => "K-pop 2022",
            SearchFilterMode.kpop2023 => "K-pop 2023",
            SearchFilterMode.kpop2024 => "K-pop 2024",
            _ => "Unknown"
        };

        Title = MainViewModel.SearchFilterModeText;

        ToolbarItem playOrAddRandomToolbarItem = new ToolbarItem
        {
            Text = "",
            IconImageSource = new FontImageSource() { FontFamily = "MaterialRounded", Glyph = MaterialRounded.Casino },
            Order = ToolbarItemOrder.Default, // Primary or Secondary
            Priority = 0 // the lower the number, the higher the priority
        };

        ToolbarItem videoModeToolbarItem = new ToolbarItem
        {
            Text = "",
            IconImageSource = (MainViewModel.UsingVideoMode ? _videoOnIcon : _videoOffIcon),
            Order = ToolbarItemOrder.Default, // Primary or Secondary
            Priority = 1 // the lower the number, the higher the priority
        };

        ToolbarItem sortToolbarItem = new ToolbarItem
        {
            Text = "",
            IconImageSource = new FontImageSource() { FontFamily = "MaterialRounded", Glyph = MaterialRounded.Sort },
            Order = ToolbarItemOrder.Default, // Primary or Secondary
            Priority = 2 // the lower the number, the higher the priority
        };

        ToolbarItem collapseAllToolbarItem = new ToolbarItem
        {
            Text = "Collapse all",
            IconImageSource = new FontImageSource() { FontFamily = "MaterialRounded", Glyph = MaterialRounded.Collapse_all },
            Order = ToolbarItemOrder.Secondary, // Primary or Secondary
            Priority = 4 // the lower the number, the higher the priority
        };

        ToolbarItem expandAllToolbarItem = new ToolbarItem
        {
            Text = "Expand all",
            IconImageSource = new FontImageSource() { FontFamily = "MaterialRounded", Glyph = MaterialRounded.Expand_all },
            Order = ToolbarItemOrder.Secondary, // Primary or Secondary
            Priority = 5 // the lower the number, the higher the priority
        };

        playOrAddRandomToolbarItem.Clicked += SearchSongPartsView.PlayRandomButtonClicked;
        videoModeToolbarItem.Clicked += SearchSongPartsView.ToggleAudioModeButtonClicked;
        sortToolbarItem.Clicked += SearchSongPartsView.SortButtonClicked;
        collapseAllToolbarItem.Clicked += SearchSongPartsView.CollapseAllButtonClicked;
        expandAllToolbarItem.Clicked += SearchSongPartsView.ExpandAllButtonClicked;
        ToolbarItems.Add(playOrAddRandomToolbarItem);
        ToolbarItems.Add(videoModeToolbarItem);
        ToolbarItems.Add(sortToolbarItem);
        ToolbarItems.Add(collapseAllToolbarItem);
        ToolbarItems.Add(expandAllToolbarItem);
    }

    private void SetupLibraryToolbar()
    {
        ToolbarItems.Clear();

        Title = "Playlists";

        ToolbarItem newPlaylistToolbarItem = new ToolbarItem
        {
            Text = "",
            IconImageSource = new FontImageSource() { FontFamily = "MaterialRounded", Glyph = MaterialRounded.Add },
            Order = ToolbarItemOrder.Default, // Primary or Secondary
            Priority = 1 // the lower the number, the higher the priority
        };

        newPlaylistToolbarItem.Clicked += LibraryView.NewPlaylistButtonClicked;
        ToolbarItems.Add(newPlaylistToolbarItem);
    }

    #endregion

    private void OnPlayToggleSongPart(object? sender, EventArgs e)
    {
        AudioPlayerControl.PlayToggleButton_Pressed(sender, e);
    }

    private void OnPreviousSong(object? sender, EventArgs e)
    {
        AudioPlayerControl.PlayPreviousSongPart(sender, e);

        _detailBottomSheet.songPart = MainViewModel.CurrentSongPart;
        _detailBottomSheet.UpdateUI();
    }

    private void OnNextSong(object? sender, EventArgs e)
    {
        AudioPlayerControl.NextButton_Pressed(sender, e);

        _detailBottomSheet.songPart = MainViewModel.CurrentSongPart;
        _detailBottomSheet.UpdateUI();
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
    }

    private void OnShowPlaylist(object? sender, EventArgs e)
    {
        LibraryView = (LibraryView)LibraryContainer.Children[0];
        LibraryView.IsVisible = false;
        _currentPlaylistView.IsVisible = true;

        _currentPlaylistView.RefreshCurrentPlaylist();
    }

    // Used by searchsongpartsView, currentplaylistView
    private void OnPlaySongPart(object sender, EventArgs e)
    {
        if (MainViewModel.CurrentSongPart.Id < 0) { return; }

        switch (MainViewModel.PlayMode)
        {
            case Architecture.PlayMode.Playlist:
                if (PlaylistManager.Instance.CurrentPlaylist is not null && SearchSongPartsView.songParts.Count > 0)
                {

                }

                break;

            case Architecture.PlayMode.Queue:
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

    private void OnStopSongPart(object sender, EventArgs e)
    {
        if (MainViewModel.CurrentSongPart.Id > 0)
        {
            AudioPlayerControl.StopAudio();
        }
    }

    private void OnShowSortBy(object sender, EventArgs e)
    {
        _sortByBottomSheet.HasHandle = true;
        _sortByBottomSheet.IsCancelable = true;
        _sortByBottomSheet.HasBackdrop = true;
        _sortByBottomSheet.isShown = true;
        _sortByBottomSheet.ShowAsync();
    }

    #region AudioPlayerControl Events
    private void OnShowDetails(object sender, EventArgs e)
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