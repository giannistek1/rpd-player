using CommunityToolkit.Maui.Alerts;
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

    internal EventHandler? Pause;
    
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

        AudioMediaElement.PositionChanged += AudioMediaElementPositionChanged;
    }

    private void AudioProgressSliderDragStarted(object? sender, EventArgs e)
    {
        AudioMediaElement.PositionChanged -= AudioMediaElementPositionChanged;
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

                if (MainViewModel.SongPartsQueue.Any())
                {
                    SongPart queueSongPart = MainViewModel.SongPartsQueue.Dequeue();
                    MainViewModel.CurrentSongPart = queueSongPart;
                    PlayAudio(MainViewModel.CurrentSongPart);

                }
                else if (MainViewModel.CurrentSongPart.IsPlaying)
                {
                    StopAudio();
                }

                break;

            case PlayMode.Playlist:

                // TODO: When play new song, clear queue, add queue?

                if (!MainViewModel.PlaylistQueue.Any()) { return; }

                // Remove song from queue that was just played and add to history.
                SongPart playlistSongPart = MainViewModel.PlaylistQueue.Dequeue();
                MainViewModel.SongPartHistory.Add(playlistSongPart);
                
                // If there is a next song, next song will be current song.
                if (MainViewModel.PlaylistQueue.Count > 0)
                {
                    MainViewModel.CurrentSongPart = MainViewModel.PlaylistQueue.Peek();
                    PlayAudio(MainViewModel.CurrentSongPart);
                }

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
            Pause?.Invoke(sender, e);
            PlayToggleImage.Source = _playIcon;
            MainViewModel.CurrentSongPart.IsPlaying = false;
        }
    }

    private void NextButton_Pressed(object sender, EventArgs e)
    {
        AudioMediaElementMediaEnded(sender, e);
    }
    /// <summary>
    /// Plays audio from audioURL, updates AudioPlayerControl controls
    /// </summary>
    /// <param name="songPart"></param>
    internal void PlayAudio(SongPart songPart)
    {
        if (!HelperClass.HasInternetConnection()) { return; }

        AudioMediaElement.Source = MediaSource.FromUri(songPart.AudioURL);

        //using (var client = new MyClient())
        //{
        //    client.HeadOnly = true;
        //    // throws 404
        //    try
        //    {
        //        client.DownloadString(songPart.AudioURL);
        //    }
        //    catch
        //    {
        //        StopAudio();
        //        Toast.Make($"Media URL of the song is invalid.", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        //        SentrySdk.CaptureMessage($"Tried to play Invalid URL: {songPart.AudioURL}");
        //        return;
        //    }
        //}

        AudioMediaElement.Play();

        // Update UI
        AlbumImage.Source = ImageSource.FromUri(new Uri(songPart.AlbumURL));
        NowPlayingLabel.Text = $"{songPart.Title}";
        NowPlayingPartLabel.Text = $"{songPart.PartNameFull}";
        PlayToggleImage.Source = _pauseIcon;

        TimeSpan duration = TimeSpan.FromSeconds(songPart.ClipLength);
        DurationLabel.Text = String.Format("{0:mm\\:ss}", duration);

        // Update vars
        songPart.IsPlaying = true;
        MainViewModel.CurrentlyPlaying = true;
    }

    internal void StopAudio()
    {
        AudioMediaElement.Stop();
        AudioMediaElement.SeekTo(new TimeSpan(0));

        PlayToggleImage.Source = _playIcon;

        MainViewModel.CurrentSongPart.IsPlaying = false;
        MainViewModel.CurrentlyPlaying = false;
    }

    private void NowPlayingSwipeViewSwipeEnded(object sender, SwipeEndedEventArgs e)
    {
        AudioMediaElementMediaEnded(sender, e);
    }

    private void ViewSongPartDetailsTapped(object sender, TappedEventArgs e)
    {
        Toast.Make($"Showing detail screen!", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
    }
}