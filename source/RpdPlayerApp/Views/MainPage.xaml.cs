using CommunityToolkit.Maui.Storage;
using RpdPlayerApp.ViewModel;
using UraniumUI.Pages;

namespace RpdPlayerApp.Views;

public partial class MainPage : UraniumContentPage
{
    IFileSaver fileSaver; 
    public MainPage(IFileSaver fileSaver)
	{
		InitializeComponent();
        // In case you want to save files to a user chosen place
        this.fileSaver = fileSaver;

        SearchSongPartsView.PlaySongPart += OnPlaySongPart;
        SearchSongPartsView.SortPressed += OnSortPressed;
        LibraryView.PlaySongPart += OnPlaySongPart;
    }

    private void OnPlaySongPart(object sender, EventArgs e)
    {
        if (MainViewModel.CurrentSongPart is not null)
        {
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
}