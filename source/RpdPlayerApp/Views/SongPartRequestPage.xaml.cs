using RpdPlayerApp.ViewModels;

namespace RpdPlayerApp.Views;

/// <summary>
/// TODO: Song part request page AND bug report page.
/// </summary>
public partial class SongPartRequestPage : ContentPage
{
    SongPartRequestViewModel _viewModel = new();

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
        if (e.NewIndex == 0) { /* Show song request */ }
        else { /* Show feedback */ }
    }

    private void SubmitButtonClicked(object sender, EventArgs e)
    {

    }

    private void PartPickerIndexChanged(object sender, EventArgs e)
    {

    }
}