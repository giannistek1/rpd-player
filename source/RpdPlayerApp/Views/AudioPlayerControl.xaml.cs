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
    internal EventHandler? ShowDetails;
    internal EventHandler? UpdateProgress;

    internal Slider audioProgressSlider;


    public AudioPlayerControl()
    {
        InitializeComponent();

        audioProgressSlider = AudioProgressSlider;

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


        //if ((AudioMediaElement.Volume < MainViewModel.MainVolume - 0.02) && (AudioMediaElement.Volume > MainViewModel.MainVolume + 0.02))
        //{
            AudioMediaElement.Volume = MainViewModel.MainVolume;
        //}

        UpdateProgress?.Invoke(sender, e);
    }

    private void AudioMediaElementMediaEnded(object? sender, EventArgs e)
    {
        switch (MainViewModel.PlayMode)
        {
            case PlayMode.Queue:

                if (!MainViewModel.SongPartsQueue.Any()) {
                    if (AudioProgressSlider.Value >= AudioProgressSlider.Maximum - 2) { StopAudio(); }
                    return; 
                }

                MainViewModel.SongPartHistory.Add(MainViewModel.CurrentSongPart);

                // Next song
                MainViewModel.CurrentSongPart = MainViewModel.SongPartsQueue.Dequeue();
                PlayAudio(MainViewModel.CurrentSongPart);

                if (MainViewModel.SongPartsQueue.Count > 0)
                {
                    NextSwipeItem.IsVisible = true;
                    NextSwipeItem.Text = MainViewModel.SongPartsQueue.Peek().Title;
                }

                PreviousSwipeItem.IsVisible = true;
                PreviousSwipeItem.Text = MainViewModel.SongPartHistory[MainViewModel.SongPartHistory.Count - 1].Title;

                break;

            case PlayMode.Playlist:

                // TODO: When play new song, clear queue, add queue?

                if (!MainViewModel.PlaylistQueue.Any()) { return; }

                MainViewModel.SongPartHistory.Add(MainViewModel.CurrentSongPart);
 
                MainViewModel.CurrentSongPart = MainViewModel.PlaylistQueue.Dequeue();
                PlayAudio(MainViewModel.CurrentSongPart);

                if (MainViewModel.PlaylistQueue.Count > 0)
                {
                    NextSwipeItem.IsVisible = true;
                    NextSwipeItem.Text = MainViewModel.PlaylistQueue.Peek().Title;
                }

                PreviousSwipeItem.IsVisible = true;
                PreviousSwipeItem.Text = MainViewModel.SongPartHistory[MainViewModel.SongPartHistory.Count-1].Title;

                break;
        }
    }

    internal void PlayToggleButton_Pressed(object sender, EventArgs e)
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

    internal void PlayPreviousSongPart(object sender, EventArgs e)
    {
        if (MainViewModel.SongPartHistory.Count > 0)
        {
            MainViewModel.CurrentSongPart = MainViewModel.SongPartHistory[MainViewModel.SongPartHistory.Count - 1];

            PlayAudio(MainViewModel.SongPartHistory[MainViewModel.SongPartHistory.Count - 1]);
            MainViewModel.SongPartHistory.RemoveAt(MainViewModel.SongPartHistory.Count - 1);

            UpdatePreviousSwipeItem();
        }
    }

    internal void NextButton_Pressed(object sender, EventArgs e)
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

        // This is very slow :(
        //CheckValidUrl(songPart);

        AudioMediaElement.Play();

        // Update UI
        AlbumImage.Source = ImageSource.FromUri(new Uri(songPart.AlbumURL));
        NowPlayingLabel.Text = $"{songPart.Title}";
        NowPlayingPartLabel.Text = $"{songPart.PartNameFull}";
        PlayToggleImage.Source = _pauseIcon;

        TimeSpan duration = TimeSpan.FromSeconds(songPart.ClipLength);
        DurationLabel.Text = String.Format("{0:mm\\:ss}", duration);

        UpdateNextSwipeItem();

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

    private void CheckValidUrl(SongPart songPart)
    {
        using (var client = new MyClient())
        {
            client.HeadOnly = true;
            // throws 404
            try
            {
                client.DownloadString(songPart.AudioURL);
            }
            catch
            {
                StopAudio();
                Toast.Make($"Media URL of the song is invalid.", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
                SentrySdk.CaptureMessage($"Tried to play Invalid URL: {songPart.AudioURL}");
            }
        }
    }

    private void NowPlayingSwipeViewSwipeEnded(object sender, SwipeEndedEventArgs e)
    {
        // Next song.
        if (e.SwipeDirection == SwipeDirection.Left && e.IsOpen)
        {
            NowPlayingSwipeView.Close();
            AudioMediaElementMediaEnded(sender, e);
        }
        // Previous song
        else if (e.SwipeDirection == SwipeDirection.Right && e.IsOpen)
        {
            NowPlayingSwipeView.Close();

            // Or PlayNext/Previous bool
            PlayPreviousSongPart(sender, e);
        }
    }

    internal void UpdatePreviousSwipeItem()
    {
        if (MainViewModel.PlayMode == PlayMode.Playlist && MainViewModel.SongPartHistory.Count > 0)
        {
            PreviousSwipeItem.IsVisible = true;
            PreviousSwipeItem.Text = MainViewModel.SongPartHistory[MainViewModel.SongPartHistory.Count - 1].Title;
        }
        else if (MainViewModel.PlayMode == PlayMode.Queue && MainViewModel.SongPartHistory.Count > 0)
        {
            PreviousSwipeItem.IsVisible = true;
            PreviousSwipeItem.Text = MainViewModel.SongPartHistory[MainViewModel.SongPartHistory.Count - 1].Title;
        }
        else
        {
            PreviousSwipeItem.IsVisible = false;
            PreviousSwipeItem.Text = string.Empty;
        }
    }

    internal void UpdateNextSwipeItem()
    {
        if (MainViewModel.PlayMode == PlayMode.Playlist && MainViewModel.PlaylistQueue.Count > 0)
        {
            NextSwipeItem.IsVisible = true;
            NextSwipeItem.Text = MainViewModel.PlaylistQueue.Peek().Title;
        }
        else if (MainViewModel.PlayMode == PlayMode.Queue && MainViewModel.SongPartsQueue.Count > 0)
        {
            NextSwipeItem.IsVisible = true;
            NextSwipeItem.Text = MainViewModel.SongPartsQueue.Peek().Title;
        }
        else
        {
            NextSwipeItem.IsVisible = false;
            NextSwipeItem.Text = string.Empty;
        }
    }
    private void ViewSongPartDetailsTapped(object sender, TappedEventArgs e)
    {
        if (MainViewModel.CurrentSongPart is null || MainViewModel.CurrentSongPart.Id <= 0) { return; }

        ShowDetails?.Invoke(sender, e);
    }
}