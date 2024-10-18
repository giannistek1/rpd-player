using RpdPlayerApp.ViewModel;

namespace RpdPlayerApp.Views;

public partial class SettingsPage
{
	public SettingsPage()
	{
		InitializeComponent();

        MasterVolumeSlider.Value = MainViewModel.MainVolume * 100;
    }

    private async void BackButton_Pressed(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private void MasterVolumeSlider_ValueChanged(object sender, Syncfusion.Maui.Sliders.SliderValueChangedEventArgs e)
    {
        MainViewModel.MainVolume = e.NewValue / 100;
    }
}