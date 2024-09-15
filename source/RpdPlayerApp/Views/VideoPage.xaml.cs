using CommunityToolkit.Maui.Views;
using RpdPlayerApp.Models;

namespace RpdPlayerApp.Views;

public partial class VideoPage : ContentPage
{
	internal VideoPage(SongPart songPart)
	{
		InitializeComponent();

        VideoMediaElement.Source = MediaSource.FromUri(songPart.VideoURL);

        //AlbumImage.Source = ImageSource.FromUri(new Uri(songPart.AlbumURL));
        //NowPlayingLabel.Text = $"{songPart.Title} - {songPart.PartNameFull}";

        VideoMediaElement.Play();

        //TimeSpan duration = TimeSpan.FromSeconds(songPart.ClipLength);

        //DurationLabel.Text = String.Format("{0:mm\\:ss}", duration);

        this.Disappearing += OnDisappearing;

        ArtistLabel.Text = songPart.ArtistName;
        SongTitleLabel.Text = $"{songPart.Title} - {songPart.PartNameFull}";
    }

    private void OnDisappearing(object? sender, EventArgs e)
    {
        VideoMediaElement.Stop();
    }
}