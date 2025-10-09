using RpdPlayerApp.ViewModels;

namespace RpdPlayerApp.Views;

/// <summary>
/// TODO: Song part request page AND bug report page.
/// </summary>
public partial class SongPartRequestPage : ContentPage
{
    readonly SongPartRequestViewModel _viewModel = new();

    internal HomeView? HomeView { get; set; }
    public SongPartRequestPage()
    {
        InitializeComponent();
        BindingContext = _viewModel;
        InitializeHomeModeSegmentedControl();
    }

    private void InitializeHomeModeSegmentedControl()
    {
        SongRequestFeedbackSegmentedControl.ItemsSource = new[] { "Song request", "Feedback" };
        SongRequestFeedbackSegmentedControl.SelectedIndex = 0;
        SongRequestFeedbackSegmentedControl.SelectionChanged += SongRequestFeedbackSegmentedControlSelectionChanged;
    }

    private void SongRequestFeedbackSegmentedControlSelectionChanged(object? sender, Syncfusion.Maui.Buttons.SelectionChangedEventArgs e)
    {
        if (e.NewIndex == 0)
        {
            SongRequestContainer.IsVisible = true;
            FeedbackContainer.IsVisible = false;
        }
        else
        {
            SongRequestContainer.IsVisible = false;
            FeedbackContainer.IsVisible = true;
        }
    }

    private void SubmitButtonClicked(object sender, EventArgs e)
    {
        if (SongRequestContainer.IsVisible)
        {
            _viewModel.SubmitSongRequest(title: SongTitleEntry.Text, artist: ArtistEntry.Text, songPart: PartPicker.SelectedItem.ToString()!, withDancePractice: DancePracticeSwitch.IsToggled);
        }
        else
        {
            _viewModel.SubmitFeedback(feedback: FeedbackEditor.Text, isBug: IsBugSwitch.IsToggled);
        }
    }
}