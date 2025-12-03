using RpdPlayerApp.Architecture;
using RpdPlayerApp.Enums;
using RpdPlayerApp.Managers;
using RpdPlayerApp.ViewModels;

namespace RpdPlayerApp.Views;

public partial class FeedbackPage : ContentPage
{
    readonly SongSegmentRequestViewModel _viewModel = new();

    internal HomeView? HomeView { get; set; }
    public FeedbackPage()
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
        if (!General.HasInternetConnection()) { return; }

        string deviceId = await DeviceIdManager.GetDeviceIdAsync();

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

            SongRequestResultValue success = await SongRequestManager.SubmitSongRequest(title: SongTitleEntry.Text,
                                                            artist: ArtistEntry.Text,
                                                            songPart: SegmentPicker.SelectedItem.ToString()!,
                                                            withDancePractice: DancePracticeSwitch.IsToggled,
                                                            requestedBy: AppState.Username,
                                                            deviceId: deviceId,
                                                            note: NoteEditor.Text);

            if (success == SongRequestResultValue.Success)
            {
                General.ShowToast("Song request submitted. Thank you <3");

                SongTitleEntry.Text = string.Empty;
                ArtistEntry.Text = string.Empty;
                NoteEditor.Text = string.Empty;
            }
            else if (success == SongRequestResultValue.Cooldown)
            {
                General.ShowToast("Please wait a few seconds before submitting again.");
            }
            else if (success == SongRequestResultValue.ApiKeyMissing)
            {
                General.ShowToast("API key is missing. Cannot submit song.");
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

            SubmitFeedbackResultValue success = await _viewModel.SubmitFeedback(feedback: FeedbackEditor.Text, isBug: IsBugSwitch.IsToggled, requestedBy: AppState.Username, deviceId: deviceId);
            if (success == SubmitFeedbackResultValue.Success)
            {
                General.ShowToast("Feedback submitted. Thank you <3");

                FeedbackEditor.Text = string.Empty;
            }
            else if (success == SubmitFeedbackResultValue.Error)
            {
                General.ShowToast("Something went wrong. Try again later.");
            }
            else if (success == SubmitFeedbackResultValue.Cooldown)
            {
                General.ShowToast("Please wait a few seconds before submitting again.");
            }
            else if (success == SubmitFeedbackResultValue.ApiKeyMissing)
            {
                General.ShowToast("API key is missing. Cannot submit feedback.");
            }
            else
            {
                General.ShowToast("Something went wrong. Try again later.");
            }
        }
    }
}