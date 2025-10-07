
using CommunityToolkit.Mvvm.ComponentModel;

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
}
