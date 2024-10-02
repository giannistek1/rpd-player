using RpdPlayerApp.Models;

namespace RpdPlayerApp.Views;

public partial class SongPartDetailPage : ContentPage
{
	internal SongPartDetailPage(SongPart songPart)
	{
        InitializeComponent();

        AlbumImage.Source = ImageSource.FromUri(new Uri(songPart.AlbumURL));

        AlbumLabel.Text = $"{songPart.AlbumTitle} ";
        ReleaseDateLabel.Text = $"{songPart.Album.ReleaseDate:d}";

        SongTitleLabel.Text = $"{songPart.Title}";
        SongPartLabel.Text = $"{songPart.PartNameFull}";
        ArtistLabel.Text = songPart.ArtistName;

        TimeSpan duration = TimeSpan.FromSeconds(songPart.ClipLength);

        DurationLabel.Text = String.Format("{0:mm\\:ss}", duration);
    }
}