using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views;
using RpdPlayerApp.Models;
using RpdPlayerApp.ViewModel;
using UraniumUI.Icons.MaterialIcons;

namespace RpdPlayerApp.Views;

public partial class AudioPlayerControl : ContentView
{
    public AudioPlayerControl()
    {
        InitializeComponent();

        AudioMediaElement.MediaEnded += AudioMediaElementMediaEnded;
        AudioMediaElement.PositionChanged += AudioMediaElementPositionChanged;

        AudioProgressSlider.DragStarted += AudioProgressSliderDragStarted;
        AudioProgressSlider.DragCompleted += AudioProgressSliderDragCompleted;
    }

    #region AudioProgressSlider
    private void AudioProgressSliderDragCompleted(object? sender, EventArgs e)
    {
        AudioMediaElement.SeekTo(TimeSpan.FromSeconds(AudioProgressSlider.Value/100*AudioMediaElement.Duration.TotalSeconds));
        //AudioMediaElement.Play();

        AudioMediaElement.PositionChanged += AudioMediaElementPositionChanged;
    }

    private void AudioProgressSliderDragStarted(object? sender, EventArgs e)
    {
        AudioMediaElement.PositionChanged -= AudioMediaElementPositionChanged;

        //AudioMediaElement.Pause();
    }
    #endregion

    private void AudioMediaElementPositionChanged(object? sender, MediaPositionChangedEventArgs e)
    {
        // Can also use a timer and update value on main thread after time elapsed.
        AudioProgressSlider.Value = e.Position.TotalSeconds / AudioMediaElement.Duration.TotalSeconds * 100;
    }

    private void AudioMediaElementMediaEnded(object? sender, EventArgs e)
    {
        // TODO: switch mode enum
        if (MainViewModel.IsPlayingPlaylist)
        {
            PlaylistManager.Instance.IncrementSongPartIndex();
            
            int index = PlaylistManager.Instance.CurrentSongPartIndex;
            MainViewModel.CurrentSongPart = PlaylistManager.Instance.CurrentPlaylist[index];

            PlayAudio(MainViewModel.CurrentSongPart);
        }
        else
        {
            if (MainViewModel.SongPartsQueue.Count > 0)
            {
                PlayAudio(MainViewModel.SongPartsQueue.Dequeue());
            }
            else
            {
                var fontImageSource = new FontImageSource
                {
                    FontFamily = "MaterialRegular",
                    Glyph = MaterialRegular.Play_arrow,
                    Color = (Color)Application.Current.Resources["White"]
                };

                PlayToggleImage.Source = fontImageSource;
            }
        }
    }

    private void PlayToggleButton_Pressed(object sender, EventArgs e)
    {
        // If audio is done playing
        if (AudioMediaElement.CurrentState == MediaElementState.Stopped && AudioMediaElement.Position >= AudioMediaElement.Duration)
        {
            AudioMediaElement.SeekTo(new TimeSpan(0));
            AudioMediaElement.Play();
            var fontImageSource = new FontImageSource
            {
                FontFamily = "MaterialRegular",
                Glyph = MaterialRegular.Pause, 
                Color = (Color)Application.Current.Resources["White"]
            };

            PlayToggleImage.Source = fontImageSource;
        }
        // If audio is paused (in middle)
        else if (AudioMediaElement.CurrentState == MediaElementState.Paused || AudioMediaElement.CurrentState == MediaElementState.Stopped)
        {
            AudioMediaElement.Play();
            var fontImageSource = new FontImageSource
            {
                FontFamily = "MaterialRegular",
                Glyph = MaterialRegular.Pause, // Assuming "\uE037" is the Unicode for the Pause glyph
                Color = (Color)Application.Current.Resources["White"]
            };

            PlayToggleImage.Source = fontImageSource;
        }
        // Else pause
        else if (AudioMediaElement.CurrentState == MediaElementState.Playing)
        {
            AudioMediaElement.Pause();
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
        AudioMediaElement.Source = MediaSource.FromUri(songPart.AudioURL);
        AudioMediaElement.Play();
        
        AlbumImage.Source = ImageSource.FromUri(new Uri(songPart.AlbumURL));
        NowPlayingLabel.Text = $"{songPart.Title} - {songPart.PartNameFull}";

        var fontImageSource = new FontImageSource
        {
            FontFamily = "MaterialRegular",
            Glyph = MaterialRegular.Pause,
            Color = (Color)Application.Current.Resources["White"]
        };

        PlayToggleImage.Source = fontImageSource;

        //CommunityToolkit.Maui.Alerts.Toast.Make($"Now playing: {songPart.Title}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
    }
}