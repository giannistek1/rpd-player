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

    private void OnShown(object? sender, EventArgs e)
    {
        SortTable.BackgroundColor = (Color)Application.Current!.Resources["BackgroundColor"];
        SortByLabel.TextColor = (Color)Application.Current!.Resources["Good"];
        ScrollToTop();
    }

    private void OnDismissed(object? sender, DismissOrigin e) => isShown = false;

    private void ScrollToTop() => SortByBottomSheetScrollView.ScrollToAsync(x: 0, y: 0, animated: true);

    #region Sorting

    private void SortByCategory(object sender, EventArgs e)
    {
        if (sender == ReleaseDateOption) { AppState.SortMode = SortMode.ReleaseDate; }
        if (sender == ArtistNameOption) { AppState.SortMode = SortMode.Artist; }
        if (sender == SongTitleOption) { AppState.SortMode = SortMode.Title; }
        if (sender == GroupTypeOption) { AppState.SortMode = SortMode.GroupType; }
        if (sender == SongPartOption) { AppState.SortMode = SortMode.SongPart; }
        if (sender == ClipLengthOption) { AppState.SortMode = SortMode.ClipLength; }
        if (sender == SongCountPerArtisOption) { AppState.SortMode = SortMode.ArtistSongCount; }
        if (sender == GenreOption) { AppState.SortMode = SortMode.Genre; }
        if (sender == MemberCountOption) { AppState.SortMode = SortMode.MemberCount; }
        if (sender == AlbumNameOption) { AppState.SortMode = SortMode.AlbumName; }
        if (sender == CompanyOption) { AppState.SortMode = SortMode.Company; }
        if (sender == GenerationOption) { AppState.SortMode = SortMode.Generation; }
        if (sender == ReleaseWeekDayOption) { AppState.SortMode = SortMode.ReleaseWeekDay; }
        if (sender == YearlyDateOption) { AppState.SortMode = SortMode.YearlyDate; }

        Close?.Invoke(sender, e);
    }

    /// <summary> Close button. </summary>
    /// <param name="sender"> </param>
    /// <param name="e"> </param>
    private void CancelSort(object sender, EventArgs e) => Close?.Invoke(sender, e);

    #endregion Sorting
}