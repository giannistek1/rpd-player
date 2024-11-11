using CommunityToolkit.Maui.Views;
using RpdPlayerApp.Models;
using RpdPlayerApp.ViewModels;

namespace RpdPlayerApp.Views;

public partial class VideoPage : ContentPage
{
    internal SongPart SongPart { get; set; }
    internal VideoPage(SongPart songPart)
    {
        InitializeComponent();

        SongPart = songPart;

        this.Appearing += OnAppearing;
        this.Disappearing += OnDisappearing;
    }

    private void OnAppearing(object? sender, EventArgs e)
    {
        //AlbumImage.Source = ImageSource.FromUri(new Uri(songPart.AlbumURL));
        //NowPlayingLabel.Text = $"{songPart.Title} - {songPart.PartNameFull}";

        //TimeSpan duration = TimeSpan.FromSeconds(songPart.ClipLength);

        //DurationLabel.Text = String.Format("{0:mm\\:ss}", duration);

        VideoMediaElement.Source = MediaSource.FromUri(SongPart.VideoURL);
        VideoMediaElement.Volume = MainViewModel.MainVolume;

        SongTitleLabel.Text = $"{SongPart.Title}";
        ArtistLabel.Text = SongPart.ArtistName;
        SongPartLabel.Text = $"{SongPart.PartNameFull}";

        VideoMediaElement.ShouldAutoPlay = true;
        VideoMediaElement.ShouldLoopPlayback = MainViewModel.ShouldLoopVideo;

        // Don't put the starting value in the XML because then you can't move the slider!
        SpeedSlider.Value = 1;
    }

    private void SpeedSlider_ValueChanged(object sender, Syncfusion.Maui.Sliders.SliderValueChangedEventArgs e)
    {
        VideoMediaElement.Speed = e.NewValue;
    }

    private void OnDisappearing(object? sender, EventArgs e)
    {
        VideoMediaElement.Stop();
    }
}