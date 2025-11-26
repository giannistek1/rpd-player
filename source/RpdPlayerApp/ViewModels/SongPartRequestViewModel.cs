using CommunityToolkit.Mvvm.ComponentModel;
using RpdPlayerApp.Enums;
using RpdPlayerApp.Repositories;
using RpdPlayerApp.Services;

namespace RpdPlayerApp.ViewModels;

internal partial class SongSegmentRequestViewModel : ObservableObject
{
    [ObservableProperty]
    private string _artist = string.Empty;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private SongSegmentType _selectedSegment = SongSegmentType.Chorus2;

    [ObservableProperty]
    private bool _wantsDancePractice = false;

    [ObservableProperty]
    private List<SongSegmentType> _segmentNames = Enum.GetValues(typeof(SongSegmentType))
                                               .Cast<SongSegmentType>()
                                               .ToList();

    internal async Task<SubmitFeedbackResultValue> SubmitFeedback(string feedback, bool isBug, string deviceId, string requestedBy = "Guest")
    {
        SubmitFeedbackResultValue success = SubmitFeedbackResultValue.Failure;
        try
        {
            success = await FeedbackRepository.InsertFeedbackAsync(new()
            {
                Text = feedback,
                IsBug = isBug,
                RequestedBy = requestedBy,
                DeviceId = deviceId
            });
        }
        catch (Exception ex)
        {
            DebugService.Instance.Debug($"SongPartRequestViewModel: {ex.Message}");
        }
        return success;
    }
}
