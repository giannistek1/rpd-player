using CommunityToolkit.Maui.Views;
using RpdPlayerApp.Models;
using RpdPlayerApp.ViewModels;

namespace RpdPlayerApp.Views;

public partial class VideoPage : ContentPage
{
	internal VideoPage(SongPart songPart)
	{
		InitializeComponent();

        VideoMediaElement.Source = MediaSource.FromUri(songPart.VideoURL);
        VideoMediaElement.Volume = MainViewModel.MainVolume;

        //AlbumImage.Source = ImageSource.FromUri(new Uri(songPart.AlbumURL));
        //NowPlayingLabel.Text = $"{songPart.Title} - {songPart.PartNameFull}";

        //TimeSpan duration = TimeSpan.FromSeconds(songPart.ClipLength);

        //DurationLabel.Text = String.Format("{0:mm\\:ss}", duration);

        this.Disappearing += OnDisappearing;

        SongTitleLabel.Text = $"{songPart.Title}";
        ArtistLabel.Text = songPart.ArtistName;
        SongPartLabel.Text = $"{songPart.PartNameFull}";

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