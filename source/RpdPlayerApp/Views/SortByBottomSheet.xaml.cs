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

    private void SortByCategory(object sender, EventArgs e)
    {
        if (sender == ReleaseDateOption) { MainViewModel.SortMode = SortMode.ReleaseDate; }
        if (sender == ArtistNameOption) { MainViewModel.SortMode = SortMode.Artist; }
        if (sender == SongTitleOption) { MainViewModel.SortMode = SortMode.Title; }
        if (sender == GroupTypeOption) { MainViewModel.SortMode = SortMode.GroupType; }
        if (sender == SongPartOption) { MainViewModel.SortMode = SortMode.SongPart; }
        if (sender == ClipLengthOption) { MainViewModel.SortMode = SortMode.ClipLength; }
        if (sender == SongCountPerArtisOption) { MainViewModel.SortMode = SortMode.ArtistSongCount; }
        if (sender == GenreOption) { MainViewModel.SortMode = SortMode.Genre; }
        if (sender == MemberCountOption) { MainViewModel.SortMode = SortMode.MemberCount; }
        if (sender == AlbumNameOption) { MainViewModel.SortMode = SortMode.AlbumName; }
        if (sender == CompanyOption) { MainViewModel.SortMode = SortMode.Company; }
        if (sender == GenerationOption) { MainViewModel.SortMode = SortMode.Generation; }
        if (sender == ReleaseWeekDayOption) { MainViewModel.SortMode = SortMode.ReleaseWeekDay; }
        if (sender == YearlyDateOption) { MainViewModel.SortMode = SortMode.YearlyDate; }

        Close?.Invoke(sender, e);
    }

    /// <summary> Close button. </summary>
    /// <param name="sender"> </param>
    /// <param name="e"> </param>
    private void CancelSort(object sender, EventArgs e) => Close?.Invoke(sender, e);

    #endregion Sorting
}