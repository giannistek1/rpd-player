using RpdPlayerApp.Architecture;
using RpdPlayerApp.Enums;
using RpdPlayerApp.Managers;
using System.Collections.ObjectModel;

namespace RpdPlayerApp.Views;

public partial class ImportSegmentResultsPopup
{
    private ObservableCollection<ImportResult> FailedImports { get; } = [];

    internal ImportSegmentResultsPopup(ObservableCollection<ImportResult> failedImports)
    {
        InitializeComponent();
        FailedImports = failedImports;

        FailedImportsLabel.Text = $"Failed imports: {FailedImports.Count}";

        CloseButton.Pressed += CloseButtonPressed;
        RequestSongsButton.IsEnabled = General.HasInternetConnection();
        if (RequestSongsButton.IsEnabled)
        {
            RequestSongsButton.Pressed += RequestSongsButtonPressed;
        }
        FailedImportsListView.ItemsSource = FailedImports;
    }

    private async void RequestSongsButtonPressed(object? sender, EventArgs e)
    {
        if (!General.HasInternetConnection()) { return; }

        string deviceId = await DeviceIdManager.GetDeviceIdAsync();
        SongRequestResultValue success = SongRequestResultValue.Success;
        foreach (var import in FailedImports)
        {
            SongRequestResultValue noError = await SongRequestManager.SubmitSongRequest(title: import.Title,
                                    artist: import.Artist,
                                    songPart: SongSegmentType.Chorus1.ToString(),
                                    withDancePractice: true,
                                    requestedBy: AppState.Username,
                                    deviceId: deviceId,
                                    note: "From import playlist.",
                                    enforceCooldown: false,
                                    origin: "IMPORT");

            if ((int)noError < 1)
            {
                success = noError;
            }
        }

        if (success == SongRequestResultValue.Success)
        {
            General.ShowToast("Song request submitted. Thank you!");
        }
        else
        {
            General.ShowToast("Something went wrong. Try again later.");
        }

        await CloseAsync();
    }

    private void CloseButtonPressed(object? sender, EventArgs e) => Close();
}