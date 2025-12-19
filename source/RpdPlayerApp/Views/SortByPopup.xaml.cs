using RpdPlayerApp.Enums;
using RpdPlayerApp.Managers;

namespace RpdPlayerApp.Views;

public partial class SortByPopup
{
    internal bool isShown = false;

    internal EventHandler? ClosePopup;

    public SortByPopup()
    {
        InitializeComponent();

        SortTable.BackgroundColor = (Color)Application.Current!.Resources["BackgroundColor"];
        SortByLabel.TextColor = (Color)Application.Current!.Resources["Good"];
        ScrollToTop();
    }

    private void ScrollToTop() => SortByBottomSheetScrollView.ScrollToAsync(x: 0, y: 0, animated: true);

    #region Sorting

    private void SortByCategory(object sender, EventArgs e)
    {
        if (sender == ReleaseDateOption) { AppState.SortMode = SortModeValue.ReleaseDate; }
        if (sender == ArtistNameOption) { AppState.SortMode = SortModeValue.Artist; }
        if (sender == SongTitleOption) { AppState.SortMode = SortModeValue.Title; }
        if (sender == GroupTypeOption) { AppState.SortMode = SortModeValue.GroupType; }
        if (sender == SongPartOption) { AppState.SortMode = SortModeValue.SongPart; }
        if (sender == ClipLengthOption) { AppState.SortMode = SortModeValue.ClipLength; }
        if (sender == SongCountPerArtisOption) { AppState.SortMode = SortModeValue.ArtistSongCount; }
        if (sender == GenreOption) { AppState.SortMode = SortModeValue.Genre; }
        if (sender == MemberCountOption) { AppState.SortMode = SortModeValue.MemberCount; }
        if (sender == AlbumNameOption) { AppState.SortMode = SortModeValue.AlbumName; }
        if (sender == CompanyOption) { AppState.SortMode = SortModeValue.Company; }
        if (sender == GenerationOption) { AppState.SortMode = SortModeValue.Generation; }
        if (sender == ReleaseWeekDayOption) { AppState.SortMode = SortModeValue.ReleaseWeekDay; }
        if (sender == YearlyDateOption) { AppState.SortMode = SortModeValue.YearlyDate; }

        ClosePopup?.Invoke(sender, e);
    }

    /// <summary> Close button. </summary>
    /// <param name="sender"> </param>
    /// <param name="e"> </param>
    private void CancelSort(object sender, EventArgs e) => ClosePopup?.Invoke(sender, e);

    #endregion Sorting
}