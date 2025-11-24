using RpdPlayerApp.Architecture;
using RpdPlayerApp.Managers;
using RpdPlayerApp.Repositories;
using RpdPlayerApp.Services;
using RpdPlayerApp.ViewModels;

namespace RpdPlayerApp.Views;

public partial class SettingsPage : ContentPage
{
    internal HomeView? HomeView { get; set; }

    public SettingsPage()
    {
        InitializeComponent();
        Loaded += OnLoad;

        Appearing += OnPageAppearing;
        Disappearing += OnDisappearing;
    }

    private void OnLoad(object? sender, EventArgs e)
    {
        ThemeViewModel _viewModel = new();
        BindingContext = _viewModel;

        //AnalyticsSwitch.IsToggled = true;

        // Load settings.
        //if (Preferences.ContainsKey(CommonSettings.USE_ANALYTICS)) { AnalyticsSwitch.IsToggled = Preferences.Get(key: CommonSettings.USE_ANALYTICS, defaultValue: true); }
        if (Preferences.ContainsKey(CommonSettings.START_RPD_AUTOMATIC)) { StartRpdAutomaticSwitch.IsToggled = Preferences.Get(key: CommonSettings.START_RPD_AUTOMATIC, defaultValue: true); }
        if (Preferences.ContainsKey(CommonSettings.USE_NONCHOREO_SONGS)) { NonChoreographySwitch.IsToggled = Preferences.Get(key: CommonSettings.USE_NONCHOREO_SONGS, defaultValue: true); }
        if (Preferences.ContainsKey(CommonSettings.DEBUG_MODE)) { DebugSwitch.IsToggled = Preferences.Get(key: CommonSettings.DEBUG_MODE, defaultValue: false); }
        if (Preferences.ContainsKey(CommonSettings.USERNAME)) { UsernameEntry.Text = Preferences.Get(key: CommonSettings.USERNAME, defaultValue: Constants.DEFAULT_USERNAME); }
        else { UsernameEntry.Text = Constants.DEFAULT_USERNAME; }

        //AnalyticsSwitch.Toggled += Switch_Toggled;
        StartRpdAutomaticSwitch.Toggled += SwitchToggled;
        NonChoreographySwitch.Toggled += SwitchToggled;
        DebugSwitch.Toggled += SwitchToggled;
        UsernameEntry.TextChanged += UsernameEntryTextChanged;
    }

    private void OnPageAppearing(object? sender, EventArgs e)
    {
        MasterVolumeSlider.Value = CommonSettings.MainVolume * 100;

        // Save total activity time
        CommonSettings.RecalculateTotalActivityTime();
        Preferences.Set(CommonSettings.TOTAL_ACTIVITY_TIME, CommonSettings.TotalActivityTime.ToString());

        string timeSpan = Preferences.Get(key: CommonSettings.TOTAL_ACTIVITY_TIME, defaultValue: new TimeSpan(0).ToString());
        TimeSpan parsedTimeSpan = TimeSpan.Parse(timeSpan);

        TotalActivityTimeLabel.Text = $"{(int)parsedTimeSpan.TotalHours}h {parsedTimeSpan.Minutes:00}m {parsedTimeSpan.Seconds:00}s";
    }

    private void OnDisappearing(object? sender, EventArgs e) => HomeView?.RefreshThemeColors();

    private async void BackButtonPressed(object sender, EventArgs e) => await Navigation.PopAsync();

    private void SwitchToggled(object? sender, ToggledEventArgs e)
    {
        //if (AnalyticsSwitch == sender) { Preferences.Set(CommonSettings.USE_ANALYTICS, AnalyticsSwitch.IsToggled); } // Not used
        if (StartRpdAutomaticSwitch == sender) { Preferences.Set(CommonSettings.START_RPD_AUTOMATIC, StartRpdAutomaticSwitch.IsToggled); }
        if (NonChoreographySwitch == sender) { Preferences.Set(CommonSettings.USE_NONCHOREO_SONGS, NonChoreographySwitch.IsToggled); } // Not used
        if (DebugSwitch == sender) { DebugService.Instance.DebugInfoVisible = DebugSwitch.IsToggled; }

        //await DisplayAlert("NOTE", "Please restart the app for the change to take affect.", "OK");
    }

    private void MasterVolumeSliderValueChanged(object sender, Syncfusion.Maui.Sliders.SliderValueChangedEventArgs e)
    {
        CommonSettings.MainVolume = e.NewValue / 100;
        Preferences.Set(CommonSettings.MAIN_VOLUME, CommonSettings.MainVolume);
    }

    private void ThemePickerSelectedIndexChanged(object sender, EventArgs e)
    {
        MasterVolumeSlider.TrackStyle.ActiveFill = (Color)Application.Current!.Resources["Primary"];
        MasterVolumeSlider.ThumbStyle.Fill = (Color)Application.Current!.Resources["Primary"];
    }

    private void TestNewsButtonClicked(object sender, EventArgs e)
    {
        // Simulate ATEEZ as new songs.
        var differentNewSongs = SongPartRepository.SongParts.Where(s => s.ArtistName!.Equals("ATEEZ", StringComparison.OrdinalIgnoreCase)).ToList();
        // Randomize HasNewVideo boolean.
        foreach (var item in differentNewSongs)
        {
            item.NewVideoAvailable = Convert.ToBoolean(General.Rng.Next(2));
        }

        HomeView!.UpdateNewsBadge(differentNewSongs);
    }

    private void UsernameEntryTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (UsernameEntry.Text.IsNullOrWhiteSpace() && UsernameEntry.Text.Length < 3) { return; }

        AppState.Username = UsernameEntry.Text!.Trim();
        Preferences.Set(CommonSettings.USERNAME, AppState.Username);

        CacheState.IsDirty = true;
    }
}