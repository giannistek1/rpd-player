using CommunityToolkit.Maui.Storage;
using RpdPlayerApp.Models;
using RpdPlayerApp.ViewModel;
using UraniumUI.Pages;

namespace RpdPlayerApp.Views;

public partial class MainPage : UraniumContentPage
{
    IFileSaver fileSaver;

    public LibraryView libraryView;
    public CurrentPlaylistView currentPlaylistView;
    public MainPage(IFileSaver fileSaver)
	{
		InitializeComponent();
        // In case you want to save files to a user chosen place
        this.fileSaver = fileSaver;

        SearchSongPartsView.PlaySongPart += OnPlaySongPart;
        SearchSongPartsView.SortPressed += OnSortPressed;
        LibraryView.PlayPlaylist += OnPlaySongPart;
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
        if (!LibraryContainer.Children.Contains(libraryView))
        {
            LibraryContainer.Children.Add(libraryView);
        }
    }

    private void OnBackToPlaylists(object? sender, EventArgs e)
    {
        currentPlaylistView.ResetCurrentPlaylist();
        currentPlaylistView.IsVisible = false;
        libraryView.IsVisible = true;
    }

    private void OnShowPlaylist(object? sender, EventArgs e)
    {
        libraryView = (LibraryView)LibraryContainer.Children[0];
        libraryView.IsVisible = false;
        currentPlaylistView.IsVisible = true;

        currentPlaylistView.InitCurrentPlaylist();
    }

    private void OnPause(object? sender, EventArgs e)
    {
        SearchSongPartsView.songParts.ToList().ForEach(s => s.IsPlaying = false);
    }

    private void OnPlaySongPart(object sender, EventArgs e)
    {
        if (MainViewModel.CurrentSongPart is not null)
        {
            if (SearchSongPartsView.songParts != null && SearchSongPartsView.songParts.Count > 0)
            {
                // TODO: NOT WORKING
                //SearchSongPartsView.songParts.CollectionChanged -= SearchSongPartsView.SongPartsCollectionChanged;
                //SearchSongPartsView.songParts.FirstOrDefault(s => s.Id == MainViewModel.CurrentSongPart?.Id).IsPlaying = true;
                //SearchSongPartsView.RefreshSongParts();
                //SearchSongPartsView.songParts.CollectionChanged += SearchSongPartsView.SongPartsCollectionChanged;
            }
                

            AudioPlayerControl.PlayAudio(MainViewModel.CurrentSongPart);
        }
    }

    private void OnSortPressed(object sender, EventArgs e)
    {
        
        BottomSheet.IsVisible = true;
        BottomSheet.IsPresented = true;
    }

    private void SortByReleaseDate(object sender, EventArgs e)
    {
        BottomSheet.IsPresented = false;
        BottomSheet.IsVisible = false;
        MainViewModel.SortMode = Architecture.SortMode.ReleaseDate;
        SearchSongPartsView.RefreshSort();
    }

    private void SortByArtistName(object sender, EventArgs e)
    {
        BottomSheet.IsPresented = false;
        BottomSheet.IsVisible = false;
        MainViewModel.SortMode = Architecture.SortMode.Artist;
        SearchSongPartsView.RefreshSort();
    }

    private void SortBySongTitle(object sender, EventArgs e)
    {
        BottomSheet.IsPresented = false;
        BottomSheet.IsVisible = false;
        MainViewModel.SortMode = Architecture.SortMode.Title;
        SearchSongPartsView.RefreshSort();
    }

    private void SortByGroupType(object sender, EventArgs e)
    {
        BottomSheet.IsPresented = false;
        BottomSheet.IsVisible = false;
        MainViewModel.SortMode = Architecture.SortMode.GroupType;
        SearchSongPartsView.RefreshSort();
    }

    private void SortBySongPart(object sender, EventArgs e)
    {
        BottomSheet.IsPresented = false;
        BottomSheet.IsVisible = false;
        MainViewModel.SortMode = Architecture.SortMode.SongPart;
        SearchSongPartsView.RefreshSort();
    }

    private void CancelSort(object sender, EventArgs e)
    {
        BottomSheet.IsPresented = false;
        BottomSheet.IsVisible = false;
    }
}