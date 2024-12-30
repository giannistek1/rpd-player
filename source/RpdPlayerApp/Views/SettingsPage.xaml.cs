using RpdPlayerApp.ViewModels;

namespace RpdPlayerApp.Views;

public partial class SettingsPage : ContentPage
{
    private const string USE_SENTRY = "USE_SENTRY";
    // TODO: Settings class
    private const string MAIN_VOLUME = "MAIN_VOLUME";

	public SettingsPage()
	{
		InitializeComponent();

        ThemeViewModel _viewModel = new();
        BindingContext = _viewModel;

        this.Appearing += OnPageAppearing;

        // Load settings.
        if (Preferences.ContainsKey(USE_SENTRY))
        {
            AnalyticsCheckBox.IsChecked = Preferences.Get(key: USE_SENTRY, defaultValue: true);
        }
        else
        {
            AnalyticsCheckBox.IsChecked = true;
        }
        AnalyticsCheckBox.CheckedChanged += AnalyticsCheckBox_CheckedChanged;
    }

    private void OnPageAppearing(object? sender, EventArgs e)
    {
        MasterVolumeSlider.Value = MainViewModel.MainVolume * 100;
    }

    private async void BackButton_Pressed(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void AnalyticsCheckBox_CheckedChanged(object? sender, CheckedChangedEventArgs e)
    {
        Preferences.Set(USE_SENTRY, AnalyticsCheckBox.IsChecked);
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