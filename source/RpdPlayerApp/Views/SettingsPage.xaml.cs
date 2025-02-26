using RpdPlayerApp.Architecture;
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

        AnalyticsSwitch.IsToggled = true;

        // Load settings.
        if (Preferences.ContainsKey(CommonSettings.USE_ANALYTICS)) { AnalyticsSwitch.IsToggled = Preferences.Get(key: CommonSettings.USE_ANALYTICS, defaultValue: true); }
        if (Preferences.ContainsKey(CommonSettings.START_RPD_AUTOMATIC)) { StartRpdAutomaticSwitch.IsToggled = Preferences.Get(key: CommonSettings.START_RPD_AUTOMATIC, defaultValue: true); }
        if (Preferences.ContainsKey(CommonSettings.USE_NONCHOREO_SONGS)) { NonChoreographySwitch.IsToggled = Preferences.Get(key: CommonSettings.USE_NONCHOREO_SONGS, defaultValue: true); }

        AnalyticsSwitch.Toggled += Switch_Toggled;
        StartRpdAutomaticSwitch.Toggled += Switch_Toggled;
        NonChoreographySwitch.Toggled += Switch_Toggled;
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

    private async void BackButton_Pressed(object sender, EventArgs e) => await Navigation.PopAsync();

    private void Switch_Toggled(object? sender, ToggledEventArgs e)
    {
        if (AnalyticsSwitch == sender) { Preferences.Set(CommonSettings.USE_ANALYTICS, AnalyticsSwitch.IsToggled); } // Not used
        if (StartRpdAutomaticSwitch == sender) { Preferences.Set(CommonSettings.START_RPD_AUTOMATIC, StartRpdAutomaticSwitch.IsToggled); }
        if (NonChoreographySwitch == sender) { Preferences.Set(CommonSettings.USE_NONCHOREO_SONGS, NonChoreographySwitch.IsToggled); } // Not used

        //await DisplayAlert("NOTE", "Please restart the app for the change to take affect.", "OK");
    }

    private void MasterVolumeSlider_ValueChanged(object sender, Syncfusion.Maui.Sliders.SliderValueChangedEventArgs e)
    {
        CommonSettings.MainVolume = e.NewValue / 100;
        Preferences.Set(CommonSettings.MAIN_VOLUME, CommonSettings.MainVolume);
    }

    private void ThemePickerSelectedIndexChanged(object sender, EventArgs e)
    {
        MasterVolumeSlider.TrackStyle.ActiveFill = (Color)Application.Current!.Resources["Primary"];
        MasterVolumeSlider.ThumbStyle.Fill = (Color)Application.Current!.Resources["Primary"];
    }
}