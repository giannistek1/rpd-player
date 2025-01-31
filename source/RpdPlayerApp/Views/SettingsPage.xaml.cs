using RpdPlayerApp.ViewModels;

namespace RpdPlayerApp.Views;

public partial class SettingsPage : ContentPage
{
    private const string USE_SENTRY = "USE_SENTRY";
    // TODO: Settings class
    private const string MAIN_VOLUME = "MAIN_VOLUME";

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


        // Load settings.
        if (Preferences.ContainsKey(USE_SENTRY))
        {
            AnalyticsSwitch.IsToggled = Preferences.Get(key: USE_SENTRY, defaultValue: true);
        }
        else
        {
            AnalyticsSwitch.IsToggled = true;
        }
        AnalyticsSwitch.Toggled += AnalyticsSwitch_Toggled;
    }

    private void OnPageAppearing(object? sender, EventArgs e) => MasterVolumeSlider.Value = MainViewModel.MainVolume * 100;

    private void OnDisappearing(object? sender, EventArgs e) => HomeView?.RefreshThemeColors();

    private async void BackButton_Pressed(object sender, EventArgs e) => await Navigation.PopAsync();

    private async void AnalyticsSwitch_Toggled(object? sender, ToggledEventArgs e)
    {
        Preferences.Set(USE_SENTRY, AnalyticsSwitch.IsToggled);
        await DisplayAlert("NOTE", "Please restart the app for the change to take affect.", "OK");
    }


    private void MasterVolumeSlider_ValueChanged(object sender, Syncfusion.Maui.Sliders.SliderValueChangedEventArgs e)
    {
        MainViewModel.MainVolume = e.NewValue / 100;
        Preferences.Set(MAIN_VOLUME, MainViewModel.MainVolume);
    }

    private void ThemePickerSelectedIndexChanged(object sender, EventArgs e)
    {
        MasterVolumeSlider.TrackStyle.ActiveFill = (Color)Application.Current!.Resources["Primary"];
        MasterVolumeSlider.ThumbStyle.Fill = (Color)Application.Current!.Resources["Primary"];
    }
}