using CommunityToolkit.Maui.Views;
using RpdPlayerApp.Models;
using RpdPlayerApp.ViewModel;

namespace RpdPlayerApp.Views;

public partial class VideoPage : ContentPage
{
	internal VideoPage(SongPart songPart)
	{
		InitializeComponent();

        VideoMediaElement.Source = MediaSource.FromUri(songPart.VideoURL);

        //AlbumImage.Source = ImageSource.FromUri(new Uri(songPart.AlbumURL));
        //NowPlayingLabel.Text = $"{songPart.Title} - {songPart.PartNameFull}";

        //VideoMediaElement.Play();


        //TimeSpan duration = TimeSpan.FromSeconds(songPart.ClipLength);

        //DurationLabel.Text = String.Format("{0:mm\\:ss}", duration);

        this.Disappearing += OnDisappearing;

        ArtistLabel.Text = songPart.ArtistName;
        SongTitleLabel.Text = $"{songPart.Title} - {songPart.PartNameFull}";

        VideoMediaElement.ShouldAutoPlay = true;
        VideoMediaElement.ShouldLoopPlayback = MainViewModel.ShouldLoopVideo;
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