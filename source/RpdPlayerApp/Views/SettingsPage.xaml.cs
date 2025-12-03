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
    }

    private void OnLoad(object? sender, EventArgs e)
    {
        ThemeViewModel _viewModel = new();
        BindingContext = _viewModel;

        //AnalyticsSwitch.IsToggled = true;

        // Load settings.
        if (Preferences.ContainsKey(CommonSettings.USERNAME)) { UsernameValueLabel.Text = Preferences.Get(key: CommonSettings.USERNAME, defaultValue: Constants.DEFAULT_USERNAME); }
        else { UsernameValueLabel.Text = Constants.DEFAULT_USERNAME; }

        //if (Preferences.ContainsKey(CommonSettings.USE_ANALYTICS)) { AnalyticsSwitch.IsToggled = Preferences.Get(key: CommonSettings.USE_ANALYTICS, defaultValue: true); }
        if (Preferences.ContainsKey(CommonSettings.START_RPD_AUTOMATIC)) { StartRpdAutomaticSwitch.IsToggled = Preferences.Get(key: CommonSettings.START_RPD_AUTOMATIC, defaultValue: true); }
        if (Preferences.ContainsKey(CommonSettings.USE_NONCHOREO_SONGS)) { NonChoreographySwitch.IsToggled = Preferences.Get(key: CommonSettings.USE_NONCHOREO_SONGS, defaultValue: true); }
        if (Preferences.ContainsKey(CommonSettings.USE_TAB_ANIMATION)) { TabAnimationSwitch.IsToggled = Preferences.Get(key: CommonSettings.USE_TAB_ANIMATION, defaultValue: false); }

        if (Preferences.ContainsKey(CommonSettings.DEBUG_MODE)) { DebugSwitch.IsToggled = Preferences.Get(key: CommonSettings.DEBUG_MODE, defaultValue: false); }

        //AnalyticsSwitch.Toggled += Switch_Toggled;
        StartRpdAutomaticSwitch.Toggled += SwitchToggled;
        NonChoreographySwitch.Toggled += SwitchToggled;
        TabAnimationSwitch.Toggled += SwitchToggled;
        DebugSwitch.Toggled += SwitchToggled;
        UsernameImageButton.Clicked += UsernameImageButtonClicked;
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

    private async void BackButtonPressed(object sender, EventArgs e) => await Navigation.PopAsync();

    private void SwitchToggled(object? sender, ToggledEventArgs e)
    {
        //if (AnalyticsSwitch == sender) { Preferences.Set(CommonSettings.USE_ANALYTICS, AnalyticsSwitch.IsToggled); } // Not used
        if (StartRpdAutomaticSwitch == sender) { Preferences.Set(CommonSettings.START_RPD_AUTOMATIC, StartRpdAutomaticSwitch.IsToggled); }
        if (NonChoreographySwitch == sender) { Preferences.Set(CommonSettings.USE_NONCHOREO_SONGS, NonChoreographySwitch.IsToggled); } // Not used
        if (TabAnimationSwitch == sender) { Preferences.Set(CommonSettings.USE_TAB_ANIMATION, TabAnimationSwitch.IsToggled); }

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


    private async void UsernameImageButtonClicked(object? sender, EventArgs e)
    {
        var result = await General.ShowInputPromptAsync("Username (3-20)", UsernameValueLabel.Text, minLength: 3, maxLength: 20);
        if (result.IsCanceled || string.IsNullOrWhiteSpace(result.Text)) { return; }

        UsernameValueLabel.Text = result.Text;
        AppState.Username = result.Text;
        Preferences.Set(CommonSettings.USERNAME, AppState.Username);

        // Update all local and cloud playlists with current username.
        try
        {
            await PlaylistRepository.UpdatePlaylistsUsername(AppState.Username);
        }
        catch (Exception ex)
        {
            DebugService.Instance.Error($"SettingsPage: Failed update playlists with username: {ex.Message}");
        }

        CacheState.IsDirty = true;
    }
}