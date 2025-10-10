using RpdPlayerApp.Architecture;
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

    private async void SubmitButtonClicked(object sender, EventArgs e)
    {
        if (SongRequestContainer.IsVisible)
        {
            if (SongTitleEntry.Text.IsNullOrWhiteSpace())
            {
                General.ShowToast("Title is empty.");
                return;
            }
            else if (ArtistEntry.Text.IsNullOrWhiteSpace())
            {
                General.ShowToast("Artist is empty.");
                return;
            }

            if (await _viewModel.SubmitSongRequest(title: SongTitleEntry.Text, artist: ArtistEntry.Text, songPart: PartPicker.SelectedItem.ToString()!, withDancePractice: DancePracticeSwitch.IsToggled))
            {
                General.ShowToast("Song request submitted. Thank you!");

                SongTitleEntry.Text = string.Empty;
                ArtistEntry.Text = string.Empty;
            }
            else
            {
                General.ShowToast("Something went wrong. Try again later.");
            }
        }
        else // Feedback
        {
            if (FeedbackEditor.Text.IsNullOrWhiteSpace())
            {
                General.ShowToast("Feedback is empty.");
                return;
            }

            if (await _viewModel.SubmitFeedback(feedback: FeedbackEditor.Text, isBug: IsBugSwitch.IsToggled))
            {
                General.ShowToast("Feedback submitted. Thank you!");

                FeedbackEditor.Text = string.Empty;
            }
            else
            {
                General.ShowToast("Something went wrong. Try again later.");
            }
        }
    }
}