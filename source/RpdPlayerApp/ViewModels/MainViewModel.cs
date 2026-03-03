using CommunityToolkit.Mvvm.ComponentModel;
using RpdPlayerApp.Services;

namespace RpdPlayerApp.ViewModels;

internal partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool _isTabBarEnabled = false;

    [ObservableProperty]
    private bool _debugInfoVisible;

    public string DebugLog => DebugService.Instance.DebugLog;

    public MainViewModel()
    {
        // Initialize with current value from DebugService
        _debugInfoVisible = DebugService.Instance.DebugInfoVisible;

        // Subscribe to DebugService property changes to forward them to the view
        DebugService.Instance.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(DebugService.DebugLog))
            {
                OnPropertyChanged(nameof(DebugLog));
            }
            else if (e.PropertyName == nameof(DebugService.DebugInfoVisible))
            {
                DebugInfoVisible = DebugService.Instance.DebugInfoVisible;
            }
        };
    }
}
