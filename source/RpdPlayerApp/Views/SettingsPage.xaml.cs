using RpdPlayerApp.Models;

namespace RpdPlayerApp.Views;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();
	}

    private async void BackButton_Pressed(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}