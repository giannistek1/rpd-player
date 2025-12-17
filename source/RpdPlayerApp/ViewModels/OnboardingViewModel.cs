using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls;
using RpdPlayerApp.Architecture;
using RpdPlayerApp.Managers;
using RpdPlayerApp.Models;
using RpdPlayerApp.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RpdPlayerApp.ViewModels;

internal class OnboardingViewModel : ViewModelBase
{
    private int _step = 1;

    public string Username { get; set; }
    public bool CreateAccount { get; set; }

    public string FavoriteGroupsInput { get; set; }

    public ObservableCollection<string> AvailableGenres { get; } =
        new() { "K-Pop", "Pop", "C-pop", "K-RnB", "T-pop" };

    public ObservableCollection<object> SelectedGenres { get; set; } = new();

    // Computed fields
    public bool IsStep1 => _step == 1;
    public bool IsStep2 => _step == 2;
    public bool IsStep3 => _step == 3;

    public double Progress => _step / 3.0;

    public string NextButtonText => _step == 3 ? "Finish" : "Next";

    public ICommand NextCommand => new Command(async () =>
    {
        if (_step < 3)
        {
            _step++;
            OnPropertyChanged(nameof(IsStep1));
            OnPropertyChanged(nameof(IsStep2));
            OnPropertyChanged(nameof(IsStep3));
            OnPropertyChanged(nameof(Progress));
            OnPropertyChanged(nameof(NextButtonText));
        }
        else
        {
            await CompleteOnboarding();
        }
    });

    public ICommand SkipCommand => new Command(SkipOnboarding);

    private void SkipOnboarding()
    {
        SaveOnboardingAndExit();
    }

    private void SaveOnboardingAndExit()
    {
        Preferences.Set(CommonSettings.ONBOARDING_COMPLETED, true);

        Application.Current!.MainPage = new AppShell();
    }

    private async Task CompleteOnboarding()
    {
        var profile = new UserProfile
        {
            Username = Username,
            FavoriteGroups = FavoriteGroupsInput
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList(),
            Genres = SelectedGenres.Cast<string>().ToList()
        };

        AppState.Username = Username;
        Preferences.Set(CommonSettings.USERNAME, AppState.Username);

        Preferences.Set(CommonSettings.ONBOARDING_COMPLETED, true);
        Application.Current!.MainPage = new AppShell();

        // TODO:
        // - Save locally
        // - Create account if enabled
    }
}
