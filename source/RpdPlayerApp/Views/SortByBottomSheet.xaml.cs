using RpdPlayerApp.ViewModels;
using The49.Maui.BottomSheet;
using RpdPlayerApp.Enums;

namespace RpdPlayerApp.Views;

public partial class SortByBottomSheet
{
    internal bool isShown = false;

    internal EventHandler? Close;

	public SortByBottomSheet()
	{
		InitializeComponent();

        Shown += OnShown;
        Dismissed += OnDismissed;
	}

    private async void OnShown(object? sender, EventArgs e)
    {
        SortTable.BackgroundColor = (Color)Application.Current!.Resources["BackgroundColor"];
        SortByLabel.TextColor = (Color)Application.Current!.Resources["Good"];
        await ScrollToTop(); // TODO: Needed?
    }
    private void OnDismissed(object? sender, DismissOrigin e) => isShown = false;

    private async Task ScrollToTop() => await SortByBottomSheetScrollView.ScrollToAsync(x: 0, y: 0, animated: false);

    #region Sorting


    private void SortByReleaseDate(object sender, EventArgs e)
    {
        MainViewModel.SortMode = SortMode.ReleaseDate;
        Close?.Invoke(sender, e);
    }

    private void SortByArtistName(object sender, EventArgs e)
    {
        MainViewModel.SortMode = SortMode.Artist;
        Close?.Invoke(sender, e);
    }

    private void SortBySongTitle(object sender, EventArgs e)
    {    
        MainViewModel.SortMode = SortMode.Title;
        Close?.Invoke(sender, e);
    }

    private void SortByGroupType(object sender, EventArgs e)
    {
        MainViewModel.SortMode = SortMode.GroupType;
        Close?.Invoke(sender, e);
    }

    private void SortBySongPart(object sender, EventArgs e)
    {   
        MainViewModel.SortMode = SortMode.SongPart;
        Close?.Invoke(sender, e);
    }

    private void SortByClipLength(object sender, EventArgs e)
    {
        MainViewModel.SortMode = SortMode.ClipLength;
        Close?.Invoke(sender, e);
    }

    private void SortBySongCountPerArtist(object sender, EventArgs e)
    {   
        MainViewModel.SortMode = SortMode.ArtistSongCount;
        Close?.Invoke(sender, e);
    }

    private void SortByGenre(object sender, EventArgs e)
    {      
        MainViewModel.SortMode = SortMode.Genre;
        Close?.Invoke(sender, e);
    }

    private void SortByMemberCount(object sender, EventArgs e)
    {     
        MainViewModel.SortMode = SortMode.MemberCount;
        Close?.Invoke(sender, e);
    }

    private void SortByAlbumName(object sender, EventArgs e)
    {    
        MainViewModel.SortMode = SortMode.AlbumName;
        Close?.Invoke(sender, e);
    }

    private void SortByCompany(object sender, EventArgs e)
    {  
        MainViewModel.SortMode = SortMode.Company;
        Close?.Invoke(sender, e);
    }

    private void SortByGeneration(object sender, EventArgs e)
    {      
        MainViewModel.SortMode = SortMode.Generation;
        Close?.Invoke(sender, e);
    }

    private void SortByReleaseWeekDay(object sender, EventArgs e)
    {  
        MainViewModel.SortMode = SortMode.ReleaseWeekDay;
        Close?.Invoke(sender, e);
    }

    private void SortByYearlyDate(object sender, EventArgs e)
    {
        MainViewModel.SortMode = SortMode.YearlyDate;
        Close?.Invoke(sender, e);
    }

    private void CancelSort(object sender, EventArgs e)
    {
        Close?.Invoke(sender, e);
    }
    #endregion
}