using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views;
using RpdPlayerApp.Models;
using UraniumUI.Icons.MaterialIcons;

namespace RpdPlayerApp.Views;

public partial class AudioPlayerControl : ContentView
{
    public static readonly BindableProperty CardTitleProperty =
       BindableProperty.Create(nameof(CardTitle), typeof(string), typeof(AudioPlayerControl), string.Empty);

    public static readonly BindableProperty CardDescriptionProperty =
        BindableProperty.Create(nameof(CardDescription), typeof(string), typeof(AudioPlayerControl), string.Empty);

    public string CardTitle
    {
        get => (string)GetValue(CardTitleProperty);
        set => SetValue(CardTitleProperty, value);
    }

    public string CardDescription
    {
        get => (string)GetValue(CardDescriptionProperty);
        set => SetValue(CardDescriptionProperty, value);
    }
    public AudioPlayerControl()
    {
        InitializeComponent();
    }

    private void PlayToggleButton_Pressed(object sender, EventArgs e)
    {
        if (audioMediaElement.CurrentState == MediaElementState.Stopped ||
            audioMediaElement.CurrentState == MediaElementState.Paused)
        {
            audioMediaElement.Play();
            var fontImageSource = new FontImageSource
            {
                FontFamily = "MaterialRegular",
                Glyph = MaterialRegular.Pause, // Assuming "\uE037" is the Unicode for the Pause glyph
                Color = (Color)Application.Current.Resources["White"]
            };

            PlayToggleImage.Source = fontImageSource;
        }
        else if (audioMediaElement.CurrentState == MediaElementState.Playing)
        {
            audioMediaElement.Pause();
            var fontImageSource = new FontImageSource
            {
                FontFamily = "MaterialRegular",
                Glyph = MaterialRegular.Play_arrow, // Assuming "\uE037" is the Unicode for the Pause glyph
                Color = (Color)Application.Current.Resources["White"]
            };

            PlayToggleImage.Source = fontImageSource;
        }
    }

    internal void PlayAudio(SongPart songPart)
    {
        audioMediaElement.Source = MediaSource.FromUri(songPart.AudioURL);
        audioMediaElement.Play();
        
        AlbumImage.Source = ImageSource.FromUri(new Uri(songPart.AlbumURL));
        NowPlayingLabel.Text = $"{songPart.Title} - {songPart.PartNameFull}";

        //CommunityToolkit.Maui.Alerts.Toast.Make($"Now playing: {songPart.Title}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
    }
}