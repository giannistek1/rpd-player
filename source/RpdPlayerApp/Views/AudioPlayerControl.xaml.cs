using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views;
using RpdPlayerApp.Models;
using RpdPlayerApp.ViewModel;
using UraniumUI.Icons.MaterialSymbols;

namespace RpdPlayerApp.Views;

public partial class AudioPlayerControl : ContentView
{
    private FontImageSource _pauseIcon = new();
    private  FontImageSource _playIcon = new();

    public EventHandler Pause;
    
    public AudioPlayerControl()
    {
        InitializeComponent();

        AudioMediaElement.MediaEnded += AudioMediaElementMediaEnded;
        AudioMediaElement.PositionChanged += AudioMediaElementPositionChanged;

        AudioProgressSlider.DragStarted += AudioProgressSliderDragStarted;
        AudioProgressSlider.DragCompleted += AudioProgressSliderDragCompleted;

        //_pauseIcon = new FontImageSource
        //{
        //    FontFamily = "MaterialRegular",
        //    Glyph = MaterialOutlined.Pause,
        //};

        //_playIcon = new FontImageSource
        //{
        //    FontFamily = "MaterialRegular",
        //    Glyph = MaterialOutlined.Play_arrow,
        //};
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
            MainViewModel.CurrentSongPart = PlaylistManager.Instance.CurrentPlaylist.SongParts[index];

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
                AudioMediaElement.Stop();
                AudioMediaElement.SeekTo(new TimeSpan(0));
            }
        }
    }

    private void PlayToggleButton_Pressed(object sender, EventArgs e)
    {
        // If audio is done playing
        if (AudioMediaElement.CurrentState == MediaElementState.Stopped && AudioMediaElement.Position >= AudioMediaElement.Duration)
        {
            AudioMediaElement.Play();
        }
        // If audio is paused (in middle)
        else if (AudioMediaElement.CurrentState == MediaElementState.Paused || AudioMediaElement.CurrentState == MediaElementState.Stopped)
        {
            AudioMediaElement.Play();
        }
        // Else pause
        else if (AudioMediaElement.CurrentState == MediaElementState.Playing)
        {
            AudioMediaElement.Pause();
            Pause.Invoke(sender, e);
        }
    }

    internal void PlayAudio(SongPart songPart)
    {
        AudioMediaElement.Source = MediaSource.FromUri(songPart.AudioURL);

        AlbumImage.Source = ImageSource.FromUri(new Uri(songPart.AlbumURL));
        NowPlayingLabel.Text = $"{songPart.Title} - {songPart.PartNameFull}";

        AudioMediaElement.Play();

        // TODO: add cliplength to songparta data cuz it is always slow
        DurationLabel.Text = $"0:{(int)AudioMediaElement.Duration.TotalSeconds}";
    }
}