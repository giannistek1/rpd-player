using CommunityToolkit.Maui.Views;
using RpdPlayerApp.Architecture;
using RpdPlayerApp.Enums;
using RpdPlayerApp.Managers;
using RpdPlayerApp.Models;

namespace RpdPlayerApp.Views;

public partial class VideoPage : ContentPage
{
    internal SongPart SongPart { get; set; }

    internal VideoPage(SongPart songPart)
    {
        InitializeComponent();

        SongPart = songPart;
        Title = songPart.Title;

        Appearing += OnAppearing;
        Disappearing += OnDisappearing;
    }

    private void OnAppearing(object? sender, EventArgs e)
    {
        //AlbumImage.Source = ImageSource.FromUri(new Uri(songPart.AlbumURL));
        //NowPlayingLabel.Text = $"{songPart.Title} - {songPart.PartNameFull}";

        //TimeSpan duration = TimeSpan.FromSeconds(songPart.ClipLength);

        //DurationLabel.Text = String.Format("{0:mm\\:ss}", duration);

        VideoMediaElement.Source = MediaSource.FromUri(SongPart.VideoURL);
        VideoMediaElement.Volume = CommonSettings.MainVolume;

        ArtistLabel.Text = SongPart.ArtistName;
        SongPartLabel.Text = $" • {SongPart.PartNameFull} • Mirrored";

        VideoMediaElement.ShouldAutoPlay = true;
        VideoMediaElement.ShouldLoopPlayback = CommonSettings.ShouldLoopVideo;

        // Don't put the starting value in the XML because then you can't move the slider!
        SpeedSlider.Value = 1;

        AppState.CurrentlyPlayingState = CurrentlyPlayingStateValue.SongPart;
        PlayToggleImageButton.Source = IconManager.PauseIcon;
    }

    private void SpeedSlider_ValueChanged(object sender, Syncfusion.Maui.Sliders.SliderValueChangedEventArgs e) => VideoMediaElement.Speed = e.NewValue;

    private void OnDisappearing(object? sender, EventArgs e) => VideoMediaElement.Stop();

    internal void PlayToggleButtonPressed(object sender, EventArgs e)
    {
        if (AppState.CurrentlyPlayingState == CurrentlyPlayingStateValue.SongPart)
        {
            AppState.CurrentlyPlayingState = CurrentlyPlayingStateValue.SongPartPaused;
            VideoMediaElement.Pause();

        }
        else if (AppState.CurrentlyPlayingState == CurrentlyPlayingStateValue.SongPartPaused || AppState.CurrentlyPlayingState == CurrentlyPlayingStateValue.None)
        {
            AppState.CurrentlyPlayingState = CurrentlyPlayingStateValue.SongPart;
            VideoMediaElement.Play();
        }
        PlayToggleImageButton.Source = General.IsOddEnumValue(AppState.CurrentlyPlayingState) ? IconManager.PauseIcon : IconManager.PlayIcon;
    }
}