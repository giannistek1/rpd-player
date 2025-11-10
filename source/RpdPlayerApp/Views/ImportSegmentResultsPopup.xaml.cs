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
        RequestSongsButton.Pressed += RequestSongsButtonPressed;
        FailedImportsListView.ItemsSource = FailedImports;
    }

    private async void RequestSongsButtonPressed(object? sender, EventArgs e)
    {
        string deviceId = await DeviceIdManager.GetDeviceIdAsync();
        int success = 1;
        foreach (var import in FailedImports)
        {
            int noError = await SongRequestManager.SubmitSongRequest(title: import.Title,
                                    artist: import.Artist,
                                    songPart: SongSegmentType.Chorus1.ToString(),
                                    withDancePractice: true,
                                    requestedBy: AppState.Username,
                                    deviceId: deviceId,
                                    note: "From import playlist.",
                                    enforceCooldown: false,
                                    origin: "IMPORT");

            if (noError < 1) // TODO: Enum
            {
                success = noError;
            }
        }

        if (success == 1)
        {
            General.ShowToast("Song request submitted. Thank you!");
        }
        else
        {
            General.ShowToast("Something went wrong. Try again later.");
        }

        Close();
    }

    private void CloseButtonPressed(object? sender, EventArgs e) => Close();
}