using RpdPlayerApp.Models;
using RpdPlayerApp.ViewModel;

namespace RpdPlayerApp.Views;

public partial class MainPage
{
    private readonly CurrentPlaylistView currentPlaylistView = new();
    private readonly SortByBottomSheet sortByBottomSheet = new();
    private readonly SongPartDetailBottomSheet detailBottomSheet = new();

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

        currentPlaylistView.IsVisible = false;
        currentPlaylistView.BackToPlaylists += OnBackToPlaylists;
        currentPlaylistView.PlaySongPart += OnPlaySongPart;

        sortByBottomSheet.CloseSheet += OnCloseSortBySheet;

        detailBottomSheet.PlayToggleSongPart += OnPlayToggleSongPart;
        detailBottomSheet.PreviousSong += OnPreviousSong;
        detailBottomSheet.NextSong += OnNextSong;

        AudioPlayerControl.Pause += OnPause;
        AudioPlayerControl.ShowDetails += OnShowDetails;
        AudioPlayerControl.UpdateProgress += OnUpdateProgress;
        AudioPlayerControl.AudioEnded += OnAudioEnded;

        if (!LibraryContainer.Children.Contains(currentPlaylistView))
        {
            LibraryContainer.Children.Add(currentPlaylistView);
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

        detailBottomSheet.songPart = MainViewModel.CurrentSongPart;
        detailBottomSheet.UpdateUI();
    }

    private void OnNextSong(object? sender, EventArgs e)
    {
        AudioPlayerControl.NextButton_Pressed(sender, e);

        detailBottomSheet.songPart = MainViewModel.CurrentSongPart;
        detailBottomSheet.UpdateUI();
    }

    private void OnCloseSortBySheet(object? sender, EventArgs e)
    {
        sortByBottomSheet.DismissAsync();
        SearchSongPartsView.RefreshSort();
    }

    private void OnEnqueueSongPart(object? sender, EventArgs e)
    {
        AudioPlayerControl.UpdateNextSwipeItem();
    }

    protected override bool OnBackButtonPressed()
    {
        if (currentPlaylistView.IsVisible)
        {
            BackToPlaylists();
            return true;
        }
        else if (detailBottomSheet.isShown)
        {
            detailBottomSheet.DismissAsync();
            return true;
        }
        else if (sortByBottomSheet.isShown)
        {
            sortByBottomSheet.DismissAsync();
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
        currentPlaylistView.RefreshCurrentPlaylist();
    }

    private void OnBackToPlaylists(object? sender, EventArgs e)
    {
        BackToPlaylists();
    }

    private void BackToPlaylists()
    {
        currentPlaylistView.ResetCurrentPlaylist();
        LibraryView!.LoadPlaylists();
        currentPlaylistView.IsVisible = false;
        LibraryView.IsVisible = true;
    }

    private void OnShowPlaylist(object? sender, EventArgs e)
    {
        LibraryView = (LibraryView)LibraryContainer.Children[0];
        LibraryView.IsVisible = false;
        currentPlaylistView.IsVisible = true;

        currentPlaylistView.RefreshCurrentPlaylist();
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
        sortByBottomSheet.HasHandle = true;
        sortByBottomSheet.IsCancelable = true;
        sortByBottomSheet.HasBackdrop = true;
        sortByBottomSheet.isShown = true;
        sortByBottomSheet.ShowAsync();
    }

    #region AudioPlayerControl Events
    private void OnShowDetails(object sender, EventArgs e)
    {
        detailBottomSheet.songPart = MainViewModel.CurrentSongPart;
        detailBottomSheet.UpdateUI();

        detailBottomSheet.HasHandle = true;
        detailBottomSheet.IsCancelable = true;
        detailBottomSheet.HasBackdrop = true;
        detailBottomSheet.isShown = true;
        detailBottomSheet.ShowAsync();
    }
    private void OnPause(object? sender, EventArgs e)
    {
        SearchSongPartsView.songParts.ToList().ForEach(s => s.IsPlaying = false);
    }
    private void OnAudioEnded(object? sender, EventArgs e)
    {
        detailBottomSheet.songPart = MainViewModel.CurrentSongPart;
        detailBottomSheet.UpdateUI();
    }

    private void OnUpdateProgress(object? sender, EventArgs e)
    {
        detailBottomSheet.UpdateProgress(AudioPlayerControl.audioProgressSlider.Value);
    }
    #endregion


    #endregion

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        HomeView.FilterPressed -= OnFilterPressed;

        SearchSongPartsView.PlaySongPart -= OnPlaySongPart;
        SearchSongPartsView.StopSongPart -= OnStopSongPart;
        SearchSongPartsView.AddSongPart -= OnAddSongPart;
        SearchSongPartsView.EnqueueSongPart -= OnEnqueueSongPart;
        SearchSongPartsView.ShowSortBy -= OnShowSortBy;

        LibraryView.PlayPlaylist -= OnPlaySongPart; // Not used
        LibraryView.ShowPlaylist -= OnShowPlaylist;

        currentPlaylistView.BackToPlaylists -= OnBackToPlaylists;
        currentPlaylistView.PlaySongPart -= OnPlaySongPart;

        AudioPlayerControl.Pause -= OnPause;
        AudioPlayerControl.ShowDetails -= OnShowDetails;

        sortByBottomSheet.CloseSheet -= OnCloseSortBySheet;
    }

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