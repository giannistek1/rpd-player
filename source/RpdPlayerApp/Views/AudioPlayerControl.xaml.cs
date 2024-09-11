using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views;
using RpdPlayerApp.Architecture;
using RpdPlayerApp.Models;
using RpdPlayerApp.ViewModel;
using UraniumUI.Icons.MaterialSymbols;

namespace RpdPlayerApp.Views;

public partial class AudioPlayerControl : ContentView
{
    private FontImageSource _pauseIcon = new();
    private FontImageSource _playIcon = new();

    public EventHandler Pause;
    
    public AudioPlayerControl()
    {
        InitializeComponent();

        AudioMediaElement.MediaEnded += AudioMediaElementMediaEnded;
        AudioMediaElement.PositionChanged += AudioMediaElementPositionChanged;

        AudioProgressSlider.DragStarted += AudioProgressSliderDragStarted;
        AudioProgressSlider.DragCompleted += AudioProgressSliderDragCompleted;

        _pauseIcon = new FontImageSource
        {
            FontFamily = "MaterialRegular",
            Glyph = MaterialOutlined.Pause,
        };

        _playIcon = new FontImageSource
        {
            FontFamily = "MaterialRegular",
            Glyph = MaterialOutlined.Play_arrow,
        };

        PlayToggleImage.Source = _playIcon;
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
        switch (MainViewModel.PlayMode)
        {
            case PlayMode.Queue:
                if (MainViewModel.SongPartsQueue.Count > 0)
                {
                    SongPart songPart = MainViewModel.SongPartsQueue.Dequeue();
                    PlayAudio(songPart);
                    MainViewModel.CurrentSongPart = songPart;
                }
                else
                {
                    AudioMediaElement.Stop();
                    AudioMediaElement.SeekTo(new TimeSpan(0));
                    PlayToggleImage.Source = _playIcon;
                    MainViewModel.CurrentSongPart.IsPlaying = false;
                    MainViewModel.CurrentlyPlaying = false;
                }
                break;

            case PlayMode.Playlist:

                PlaylistManager.Instance.IncrementSongPartIndex();

                int index = PlaylistManager.Instance.CurrentSongPartIndex;
                MainViewModel.CurrentSongPart = PlaylistManager.Instance.CurrentPlaylist.SongParts[index];

                PlayAudio(MainViewModel.CurrentSongPart);

                break;
        }
    }

    private void PlayToggleButton_Pressed(object sender, EventArgs e)
    {
        // If audio is done playing
        if (AudioMediaElement.CurrentState == MediaElementState.Stopped && AudioMediaElement.Position >= AudioMediaElement.Duration)
        {
            AudioMediaElement.Play();
            PlayToggleImage.Source = _pauseIcon;
            MainViewModel.CurrentSongPart.IsPlaying = true;
            TimerManager.StartInfiniteScaleYAnimationWithTimer();
        }
        // If audio is paused (in middle)
        else if (AudioMediaElement.CurrentState == MediaElementState.Paused || AudioMediaElement.CurrentState == MediaElementState.Stopped)
        {
            AudioMediaElement.Play();
            PlayToggleImage.Source = _pauseIcon;
            MainViewModel.CurrentSongPart.IsPlaying = true;
            TimerManager.StartInfiniteScaleYAnimationWithTimer();
        }
        // Else pause
        else if (AudioMediaElement.CurrentState == MediaElementState.Playing)
        {
            AudioMediaElement.Pause();
            Pause.Invoke(sender, e);
            PlayToggleImage.Source = _playIcon;
            MainViewModel.CurrentSongPart.IsPlaying = false;
        }
    }

    internal void PlayAudio(SongPart songPart)
    {
        AudioMediaElement.Source = MediaSource.FromUri(songPart.AudioURL);

        AlbumImage.Source = ImageSource.FromUri(new Uri(songPart.AlbumURL));
        NowPlayingLabel.Text = $"{songPart.Title}";
        NowPlayingPartLabel.Text = $"{songPart.PartNameFull}";

        AudioMediaElement.Play();
        songPart.IsPlaying = true;
        PlayToggleImage.Source = _pauseIcon;
        MainViewModel.CurrentlyPlaying = true;

        TimeSpan duration = TimeSpan.FromSeconds(songPart.ClipLength);

        DurationLabel.Text = String.Format("{0:mm\\:ss}", duration);
    }

    internal void StopAudio()
    {
        AudioMediaElement.Stop();
        MainViewModel.CurrentSongPart.IsPlaying = false;
        PlayToggleImage.Source = _playIcon;
    }
}