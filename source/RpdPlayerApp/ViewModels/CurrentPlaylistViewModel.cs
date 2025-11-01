using CommunityToolkit.Mvvm.ComponentModel;

namespace RpdPlayerApp.ViewModels;

internal partial class CurrentPlaylistViewModel : ObservableObject
{
    [ObservableProperty]
    internal int _currentIndex;
}
