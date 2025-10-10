
using CommunityToolkit.Mvvm.ComponentModel;
using RpdPlayerApp.Architecture;
using RpdPlayerApp.Enums;
using RpdPlayerApp.Repositories;

namespace RpdPlayerApp.ViewModels;

internal partial class SongPartRequestViewModel : ObservableObject
{
    [ObservableProperty]
    private string _artist = string.Empty;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private SongSegmentType _selectedPart = SongSegmentType.Chorus2;

    [ObservableProperty]
    private bool _wantsDancePractice = false;

    [ObservableProperty]
    private List<SongSegmentType> _partNames = Enum.GetValues(typeof(SongSegmentType))
                                               .Cast<SongSegmentType>()
                                               .ToList();

    private FeedbackRepository _feedbackRepository = new();
    private SongRequestRepository _songRequestRepository = new();

    internal async Task<bool> SubmitSongRequest(string title, string artist, string songPart, bool withDancePractice)
    {
        bool success = false;
        try
        {
            success = await _songRequestRepository.InsertSongRequestAsync(new()
            {
                Artist = artist,
                Title = title,
                Part = songPart,
                WithDancePractice = withDancePractice,
                RequestedBy = "anonymous" // TODO: username / device id
            });
        }
        catch (Exception ex)
        {
            DebugService.Instance.AddDebug(ex.Message);
        }
        return success;
    }

    internal async Task<bool> SubmitFeedback(string feedback, bool isBug)
    {
        bool success = false;
        try
        {
            success = await _feedbackRepository.InsertFeedbackAsync(new()
            {
                Text = feedback,
                IsBug = isBug,
                RequestedBy = "anonymous" // TODO: username / device id
            });
        }
        catch (Exception ex)
        {
            DebugService.Instance.AddDebug(ex.Message);
        }
        return success;
    }
}
