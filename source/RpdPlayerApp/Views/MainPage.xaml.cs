using CommunityToolkit.Maui.Core.Primitives;
using RpdPlayerApp.Models;
using RpdPlayerApp.ViewModel;

namespace RpdPlayerApp.Views;

public partial class MainPage
{
    private CurrentPlaylistView currentPlaylistView = new();
    private readonly SortByBottomSheet sortBySheet = new();
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

        sortBySheet.CloseSheet += OnCloseSheet;

        detailBottomSheet.PlayToggleSongPart += OnPlayToggleSongPart;
        detailBottomSheet.PreviousSong += OnPreviousSong;
        detailBottomSheet.NextSong += OnNextSong;

        AudioPlayerControl.Pause += OnPause;
        AudioPlayerControl.ShowDetails += OnShowDetails;
        AudioPlayerControl.UpdateProgress += OnUpdateProgress;

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

    private void OnUpdateProgress(object? sender, EventArgs e)
    {
        detailBottomSheet.UpdateProgress(AudioPlayerControl.audioProgressSlider.Value);
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

    private void OnCloseSheet(object? sender, EventArgs e)
    {
        sortBySheet.DismissAsync();
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

    private void OnPause(object? sender, EventArgs e)
    {
        SearchSongPartsView.songParts.ToList().ForEach(s => s.IsPlaying = false);
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
        sortBySheet.HasHandle = true;
        sortBySheet.IsCancelable = true;
        sortBySheet.HasBackdrop = true;
        sortBySheet.ShowAsync();
    }

    private void OnShowDetails(object sender, EventArgs e)
    {
        detailBottomSheet.songPart = MainViewModel.CurrentSongPart;
        detailBottomSheet.UpdateUI();
        
        detailBottomSheet.HasHandle = true;
        detailBottomSheet.IsCancelable = true;
        detailBottomSheet.HasBackdrop = true;
        detailBottomSheet.ShowAsync();
    }
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

        sortBySheet.CloseSheet -= OnCloseSheet;
    }
}