using RpdPlayerApp.ViewModels;

namespace RpdPlayerApp.Views;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();

        ThemeViewModel _viewModel = new();

        MasterVolumeSlider.Value = MainViewModel.MainVolume * 100;

        BindingContext = _viewModel;
    }

    private async void BackButton_Pressed(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private void MasterVolumeSlider_ValueChanged(object sender, Syncfusion.Maui.Sliders.SliderValueChangedEventArgs e)
    {
        MainViewModel.MainVolume = e.NewValue / 100;
    }

    private void ThemePickerSelectedIndexChanged(object sender, EventArgs e)
    {
        MasterVolumeSlider.TrackStyle.ActiveFill = (Color)Application.Current!.Resources["Primary"];
        MasterVolumeSlider.ThumbStyle.Fill = (Color)Application.Current!.Resources["Primary"];
    }
}