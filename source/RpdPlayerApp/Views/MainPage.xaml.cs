using RpdPlayerApp.Models;
using RpdPlayerApp.ViewModel;
using UraniumUI.Pages;

namespace RpdPlayerApp.Views;

public partial class MainPage : UraniumContentPage
{
    public CurrentPlaylistView currentPlaylistView;
    public MainPage()
	{
		InitializeComponent();

        HomeView.FilterPressed += OnFilterPressed;

        SearchSongPartsView.PlaySongPart += OnPlaySongPart;
        SearchSongPartsView.StopSongPart += OnStopSongPart;
        SearchSongPartsView.AddSongPart += OnAddSongPart;
        SearchSongPartsView.EnqueueSongPart += OnEnqueueSongPart;
        SearchSongPartsView.SortPressed += OnSortPressed;

        LibraryView.PlayPlaylist += OnPlaySongPart; // Not used
        LibraryView.ShowPlaylist += OnShowPlaylist;

        currentPlaylistView = new CurrentPlaylistView();
        currentPlaylistView.IsVisible = false;
        currentPlaylistView.BackToPlaylists += OnBackToPlaylists;
        currentPlaylistView.PlaySongPart += OnPlaySongPart;

        AudioPlayerControl.Pause += OnPause;

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
                    // TODO: Based on song history
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

    #region Sorting
    private void OnSortPressed(object sender, EventArgs e)
    {
        SortModeBottomSheet.IsVisible = true;
        SortModeBottomSheet.IsPresented = true;
    }

    private void SortByReleaseDate(object sender, EventArgs e)
    {
        SortModeBottomSheet.IsPresented = false;
        SortModeBottomSheet.IsVisible = false;
        MainViewModel.SortMode = Architecture.SortMode.ReleaseDate;
        SearchSongPartsView.RefreshSort();
    }

    private void SortByArtistName(object sender, EventArgs e)
    {
        SortModeBottomSheet.IsPresented = false;
        SortModeBottomSheet.IsVisible = false;
        MainViewModel.SortMode = Architecture.SortMode.Artist;
        SearchSongPartsView.RefreshSort();
    }

    private void SortBySongTitle(object sender, EventArgs e)
    {
        SortModeBottomSheet.IsPresented = false;
        SortModeBottomSheet.IsVisible = false;
        MainViewModel.SortMode = Architecture.SortMode.Title;
        SearchSongPartsView.RefreshSort();
    }

    private void SortByGroupType(object sender, EventArgs e)
    {
        SortModeBottomSheet.IsPresented = false;
        SortModeBottomSheet.IsVisible = false;
        MainViewModel.SortMode = Architecture.SortMode.GroupType;
        SearchSongPartsView.RefreshSort();
    }

    private void SortBySongPart(object sender, EventArgs e)
    {
        SortModeBottomSheet.IsPresented = false;
        SortModeBottomSheet.IsVisible = false;
        MainViewModel.SortMode = Architecture.SortMode.SongPart;
        SearchSongPartsView.RefreshSort();
    }

    private void SortByClipLength(object sender, EventArgs e)
    {
        SortModeBottomSheet.IsPresented = false;
        SortModeBottomSheet.IsVisible = false;
        MainViewModel.SortMode = Architecture.SortMode.ClipLength;
        SearchSongPartsView.RefreshSort();
    }

    private void SortBySongCountPerArtist(object sender, EventArgs e)
    {
        SortModeBottomSheet.IsPresented = false;
        SortModeBottomSheet.IsVisible = false;
        MainViewModel.SortMode = Architecture.SortMode.ArtistSongCount;
        SearchSongPartsView.RefreshSort();
    }

    private void SortByLanguage(object sender, EventArgs e)
    {
        SortModeBottomSheet.IsPresented = false;
        SortModeBottomSheet.IsVisible = false;
        MainViewModel.SortMode = Architecture.SortMode.Language;
        SearchSongPartsView.RefreshSort();
    }

    private void SortByMemberCount(object sender, EventArgs e)
    {
        SortModeBottomSheet.IsPresented = false;
        SortModeBottomSheet.IsVisible = false;
        MainViewModel.SortMode = Architecture.SortMode.MemberCount;
        SearchSongPartsView.RefreshSort();
    }

    private void SortByAlbumName(object sender, EventArgs e)
    {
        SortModeBottomSheet.IsPresented = false;
        SortModeBottomSheet.IsVisible = false;
        MainViewModel.SortMode = Architecture.SortMode.AlbumName;
        SearchSongPartsView.RefreshSort();
    }

    private void SortByCompany(object sender, EventArgs e)
    {
        SortModeBottomSheet.IsPresented = false;
        SortModeBottomSheet.IsVisible = false;
        MainViewModel.SortMode = Architecture.SortMode.Company;
        SearchSongPartsView.RefreshSort();
    }

    private void SortByGeneration(object sender, EventArgs e)
    {
        SortModeBottomSheet.IsPresented = false;
        SortModeBottomSheet.IsVisible = false;
        MainViewModel.SortMode = Architecture.SortMode.Generation;
        SearchSongPartsView.RefreshSort();
    }

    private void SortByReleaseWeekDay(object sender, EventArgs e)
    {
        SortModeBottomSheet.IsPresented = false;
        SortModeBottomSheet.IsVisible = false;
        MainViewModel.SortMode = Architecture.SortMode.ReleaseWeekDay;
        SearchSongPartsView.RefreshSort();
    }

    private void SortByYearlyDate(object sender, EventArgs e)
    {
        SortModeBottomSheet.IsPresented = false;
        SortModeBottomSheet.IsVisible = false;
        MainViewModel.SortMode = Architecture.SortMode.YearlyDate;
        SearchSongPartsView.RefreshSort();
    }

    private void CancelSort(object sender, EventArgs e)
    {
        SortModeBottomSheet.IsPresented = false;
        SortModeBottomSheet.IsVisible = false;
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
        SearchSongPartsView.SortPressed -= OnSortPressed;

        LibraryView.PlayPlaylist -= OnPlaySongPart; // Not used
        LibraryView.ShowPlaylist -= OnShowPlaylist;

        currentPlaylistView.BackToPlaylists -= OnBackToPlaylists;
        currentPlaylistView.PlaySongPart -= OnPlaySongPart;
    }
}