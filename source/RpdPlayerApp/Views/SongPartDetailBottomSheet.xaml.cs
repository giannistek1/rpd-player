using CommunityToolkit.Maui.Core.Primitives;
using RpdPlayerApp.Models;

namespace RpdPlayerApp.Views;

public partial class SongPartDetailBottomSheet
{
    internal SongPart? songPart = null;

    internal EventHandler? PlayToggleSongPart;
    internal EventHandler? PreviousSong;
    internal EventHandler? NextSong;


	public SongPartDetailBottomSheet()
	{
		InitializeComponent();
    }

    internal void UpdateUI()
    {
        AlbumImage.Source = ImageSource.FromUri(new Uri(songPart.AlbumURL));

        AlbumLabel.Text = $"{songPart.AlbumTitle} ";
        ReleaseDateLabel.Text = $"{songPart.Album.ReleaseDate:d}";

        SongTitleLabel.Text = $"{songPart.Title}";
        SongPartLabel.Text = $"{songPart.PartNameFull}";
        ArtistLabel.Text = songPart.ArtistName;

        TimeSpan duration = TimeSpan.FromSeconds(songPart.ClipLength);

        DurationLabel.Text = String.Format("{0:mm\\:ss}", duration);
    }

    internal void UpdateProgress(double value)
    {
        AudioProgressSlider.Value = value;

        if (songPart is not null)
        {
            TimeSpan duration = TimeSpan.FromSeconds(value/100 * songPart.ClipLength);

            ProgressLabel.Text = String.Format("{0:mm\\:ss}", duration);
        }
        else
        {
            ProgressLabel.Text = "0:00";
        }
    }

    private void PreviousButton_Pressed(object sender, EventArgs e)
    {
        PreviousSong?.Invoke(sender, e);
    }

    private void PlayToggleButton_Pressed(object sender, EventArgs e)
    {
        PlayToggleSongPart?.Invoke(sender, e);
    }

    private void NextButton_Pressed(object sender, EventArgs e)
    {
        NextSong?.Invoke(sender, e);
    }
}