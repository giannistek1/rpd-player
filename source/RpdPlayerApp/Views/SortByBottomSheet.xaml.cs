using RpdPlayerApp.ViewModels;
using The49.Maui.BottomSheet;

namespace RpdPlayerApp.Views;

public partial class SortByBottomSheet
{
    internal bool isShown = false;

    internal EventHandler? Close;

	public SortByBottomSheet()
	{
		InitializeComponent();

        this.Shown += OnShown;
        this.Dismissed += OnDismissed;
	}

    private void OnShown(object? sender, EventArgs e)
    {
        SortTable.BackgroundColor = (Color)Application.Current!.Resources["BackgroundColor"];
        SortByLabel.TextColor = (Color)Application.Current!.Resources["Good"];
    }

    #region Sorting

    private void OnDismissed(object? sender, DismissOrigin e)
    {
        isShown = false;
    }

    private void SortByReleaseDate(object sender, EventArgs e)
    {
        MainViewModel.SortMode = Architecture.SortMode.ReleaseDate;
        Close?.Invoke(sender, e);
    }

    private void SortByArtistName(object sender, EventArgs e)
    {
        MainViewModel.SortMode = Architecture.SortMode.Artist;
        Close?.Invoke(sender, e);
    }

    private void SortBySongTitle(object sender, EventArgs e)
    {    
        MainViewModel.SortMode = Architecture.SortMode.Title;
        Close?.Invoke(sender, e);
    }

    private void SortByGroupType(object sender, EventArgs e)
    {
        MainViewModel.SortMode = Architecture.SortMode.GroupType;
        Close?.Invoke(sender, e);
    }

    private void SortBySongPart(object sender, EventArgs e)
    {   
        MainViewModel.SortMode = Architecture.SortMode.SongPart;
        Close?.Invoke(sender, e);
    }

    private void SortByClipLength(object sender, EventArgs e)
    {       
        MainViewModel.SortMode = Architecture.SortMode.ClipLength;
        Close?.Invoke(sender, e);
    }

    private void SortBySongCountPerArtist(object sender, EventArgs e)
    {   
        MainViewModel.SortMode = Architecture.SortMode.ArtistSongCount;
        Close?.Invoke(sender, e);
    }

    private void SortByGenre(object sender, EventArgs e)
    {      
        MainViewModel.SortMode = Architecture.SortMode.Genre;
        Close?.Invoke(sender, e);
    }

    private void SortByMemberCount(object sender, EventArgs e)
    {     
        MainViewModel.SortMode = Architecture.SortMode.MemberCount;
        Close?.Invoke(sender, e);
    }

    private void SortByAlbumName(object sender, EventArgs e)
    {    
        MainViewModel.SortMode = Architecture.SortMode.AlbumName;
        Close?.Invoke(sender, e);
    }

    private void SortByCompany(object sender, EventArgs e)
    {  
        MainViewModel.SortMode = Architecture.SortMode.Company;
        Close?.Invoke(sender, e);
    }

    private void SortByGeneration(object sender, EventArgs e)
    {      
        MainViewModel.SortMode = Architecture.SortMode.Generation;
        Close?.Invoke(sender, e);
    }

    private void SortByReleaseWeekDay(object sender, EventArgs e)
    {  
        MainViewModel.SortMode = Architecture.SortMode.ReleaseWeekDay;
        Close?.Invoke(sender, e);
    }

    private void SortByYearlyDate(object sender, EventArgs e)
    {
        MainViewModel.SortMode = Architecture.SortMode.YearlyDate;
        Close?.Invoke(sender, e);
    }

    private void CancelSort(object sender, EventArgs e)
    {
        Close?.Invoke(sender, e);
    }
    #endregion
}