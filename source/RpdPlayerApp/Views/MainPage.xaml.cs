using RpdPlayerApp.Models;
using RpdPlayerApp.ViewModel;

namespace RpdPlayerApp.Views;

public partial class MainPage
{
    private readonly CurrentPlaylistView _currentPlaylistView = new();
    private readonly SortByBottomSheet _sortByBottomSheet = new();
    private readonly SongPartDetailBottomSheet _detailBottomSheet = new();

    public MainPage()
    {
        InitializeComponent();

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
    }

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
        if (MainViewModel.CurrentSongPart.Id == -1) { return; }

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
        if (MainViewModel.CurrentSongPart.Id != -1)
        {
            AudioPlayerControl.StopAudio();
        }
    }

    #region Show BottomSheets
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


    #endregion

    private void MainContainer_TabItemTapped(object sender, Syncfusion.Maui.TabView.TabItemTappedEventArgs e)
    {
        if (HomeTabItem == e.TabItem)
        {

        }
        else if (SearchTabItem == e.TabItem)
        {

        }
        else if (LibraryTabItem == e.TabItem)
        {

        }
    }
}