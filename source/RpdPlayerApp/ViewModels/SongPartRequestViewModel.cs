
using CommunityToolkit.Mvvm.ComponentModel;
using RpdPlayerApp.Repositories;

namespace RpdPlayerApp.ViewModels;

internal partial class SongPartRequestViewModel : ObservableObject
{
    [ObservableProperty]
    private string _artist = string.Empty;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _selectedPart = string.Empty;

    [ObservableProperty]
    private bool _wantsDancePractice = false;

    [ObservableProperty]
    private List<string> _partNames = ["Chorus", "Verse", "Bridge", "Intro", "Outro", "Dance Break", "Instrumental", "Other"];

    private FeedbackRepository _feedbackRepository = new();

    internal void SubmitSongRequest(string title, string artist, string songPart, bool withDancePractice)
    {

    }

    internal async void SubmitFeedback(string feedback, bool isBug)
    {
        await _feedbackRepository.InsertFeedbackAsync(new()
        {
            Text = feedback,
            IsBug = isBug
        });
    }
}
