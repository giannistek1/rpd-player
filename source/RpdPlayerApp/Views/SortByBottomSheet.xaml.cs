using RpdPlayerApp.ViewModels;
using The49.Maui.BottomSheet;

namespace RpdPlayerApp.Views;

public partial class SortByBottomSheet
{
    internal bool isShown = false;

    internal EventHandler? CloseSheet;

	public SortByBottomSheet()
	{
		InitializeComponent();

        this.Shown += OnShown;
        this.Dismissed += OnDismissed;
	}

    private void OnShown(object? sender, EventArgs e)
    {
        SortTable.BackgroundColor = (Color)Application.Current!.Resources["BackgroundColor"];
    }

    #region Sorting

    private void OnDismissed(object? sender, DismissOrigin e)
    {
        isShown = false;
    }

    private void SortByReleaseDate(object sender, EventArgs e)
    {
        MainViewModel.SortMode = Architecture.SortMode.ReleaseDate;
        CloseSheet?.Invoke(sender, e);
    }

    private void SortByArtistName(object sender, EventArgs e)
    {
        MainViewModel.SortMode = Architecture.SortMode.Artist;
        CloseSheet?.Invoke(sender, e);
    }

    private void SortBySongTitle(object sender, EventArgs e)
    {    
        MainViewModel.SortMode = Architecture.SortMode.Title;
        CloseSheet?.Invoke(sender, e);
    }

    private void SortByGroupType(object sender, EventArgs e)
    {
        MainViewModel.SortMode = Architecture.SortMode.GroupType;
        CloseSheet?.Invoke(sender, e);
    }

    private void SortBySongPart(object sender, EventArgs e)
    {   
        MainViewModel.SortMode = Architecture.SortMode.SongPart;
        CloseSheet?.Invoke(sender, e);
    }

    private void SortByClipLength(object sender, EventArgs e)
    {       
        MainViewModel.SortMode = Architecture.SortMode.ClipLength;
        CloseSheet?.Invoke(sender, e);
    }

    private void SortBySongCountPerArtist(object sender, EventArgs e)
    {   
        MainViewModel.SortMode = Architecture.SortMode.ArtistSongCount;
        CloseSheet?.Invoke(sender, e);
    }

    private void SortByGenre(object sender, EventArgs e)
    {      
        MainViewModel.SortMode = Architecture.SortMode.Genre;
        CloseSheet?.Invoke(sender, e);
    }

    private void SortByMemberCount(object sender, EventArgs e)
    {     
        MainViewModel.SortMode = Architecture.SortMode.MemberCount;
        CloseSheet?.Invoke(sender, e);
    }

    private void SortByAlbumName(object sender, EventArgs e)
    {    
        MainViewModel.SortMode = Architecture.SortMode.AlbumName;
        CloseSheet?.Invoke(sender, e);
    }

    private void SortByCompany(object sender, EventArgs e)
    {  
        MainViewModel.SortMode = Architecture.SortMode.Company;
        CloseSheet?.Invoke(sender, e);
    }

    private void SortByGeneration(object sender, EventArgs e)
    {      
        MainViewModel.SortMode = Architecture.SortMode.Generation;
        CloseSheet?.Invoke(sender, e);
    }

    private void SortByReleaseWeekDay(object sender, EventArgs e)
    {  
        MainViewModel.SortMode = Architecture.SortMode.ReleaseWeekDay;
        CloseSheet?.Invoke(sender, e);
    }

    private void SortByYearlyDate(object sender, EventArgs e)
    {
        MainViewModel.SortMode = Architecture.SortMode.YearlyDate;
        CloseSheet?.Invoke(sender, e);
    }

    private void CancelSort(object sender, EventArgs e)
    {
        CloseSheet?.Invoke(sender, e);
    }
    #endregion
}