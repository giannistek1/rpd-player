using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views;
using RpdPlayerApp.Architecture;
using RpdPlayerApp.Models;
using RpdPlayerApp.ViewModels;
using UraniumUI.Icons.MaterialSymbols;

namespace RpdPlayerApp.Views;

public partial class AudioPlayerControl : ContentView
{
    private Random random = new();

    private FontImageSource _pauseIcon = new();
    private FontImageSource _playIcon = new();

    internal EventHandler? Pause;
    internal EventHandler? ShowDetails;
    internal EventHandler? UpdateProgress;
    internal EventHandler? AudioEnded;

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
            FontFamily = "MaterialRounded",
            Glyph = MaterialRounded.Pause,
            Color = (Color)Application.Current!.Resources["IconColor"] // TODO: Iconmanager, that updates all icons when theme changes? Currently sucks because this only gets set once.
        };

        _playIcon = new FontImageSource
        {
            FontFamily = "MaterialRounded",
            Glyph = MaterialRounded.Play_arrow,
            Color = (Color)Application.Current!.Resources["IconColor"] // TODO: Sucks because this only gets set once.
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

                MainViewModel.SongPartHistory.Add(MainViewModel.CurrentSongPart);

                if (MainViewModel.AutoplayMode == 1) // Autoplay
                {
                    int index = MainViewModel.SongParts.FindIndex(s => s.Id == MainViewModel.CurrentSongPart.Id);

                    // Imagine index = 1220 count = 1221
                    if (index + 1 < MainViewModel.SongParts.Count)
                    {
                        MainViewModel.SongPartsQueue.Enqueue(MainViewModel.SongParts[index + 1]);
                    }
                }
                else if (MainViewModel.AutoplayMode == 2) // Shuffle
                {
                    int index = random.Next(MainViewModel.SongParts.Count);
                    MainViewModel.SongPartsQueue.Enqueue(MainViewModel.SongParts[index]);
                }
                else if (MainViewModel.AutoplayMode == 3) // Repeat one
                {
                    // TOOD: Rewrite with audioplayer.LoopPlayback = true (saves data and performance?)
                    MainViewModel.SongPartsQueue.Enqueue(MainViewModel.CurrentSongPart);
                }

                if (!MainViewModel.SongPartsQueue.Any()) {
                    if (AudioProgressSlider.Value >= AudioProgressSlider.Maximum - 2) { StopAudio(); }
                    return; 
                }

                // Next song
                MainViewModel.CurrentSongPart = MainViewModel.SongPartsQueue.Dequeue();
                PlayAudio(MainViewModel.CurrentSongPart);

                SetPreviousSwipeItem(isVisible: true, songPart: MainViewModel.SongPartHistory[MainViewModel.SongPartHistory.Count - 1]);

                if (MainViewModel.SongPartsQueue.Count > 0)
                {
                    SetNextSwipeItem(isVisible: true, songPart: MainViewModel.SongPartsQueue.Peek());
                }

                break;

            case PlayMode.Playlist:

                // TODO: When play new song, clear queue, add queue?

                MainViewModel.SongPartHistory.Add(MainViewModel.CurrentSongPart);

                // If queue empty, return
                if (!MainViewModel.PlaylistQueue.Any()) { return; }
 
                MainViewModel.CurrentSongPart = MainViewModel.PlaylistQueue.Dequeue();
                PlayAudio(MainViewModel.CurrentSongPart);

                SetPreviousSwipeItem(isVisible: true, songPart: MainViewModel.SongPartHistory[MainViewModel.SongPartHistory.Count - 1]);

                if (MainViewModel.PlaylistQueue.Count > 0)
                {
                    SetNextSwipeItem(isVisible: true, songPart: MainViewModel.PlaylistQueue.Peek());
                }

                break;
        }

        // Updates song detail UI
        AudioEnded?.Invoke(sender, e);
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
            // TODO: UGLY, needs fadeout or a delay or text invisible, be creative (PROBABLY CHANGE TO CAROUSELVIEW?).

            NowPlayingSwipeView.Close();
            AudioMediaElementMediaEnded(sender, e);
        }
        // Previous song
        else if (e.SwipeDirection == SwipeDirection.Right && e.IsOpen)
        {
            // TODO: UGLY, needs fadeout or a delay or text invisible, be creative (PROBABLY CHANGE TO CAROUSELVIEW?).

            NowPlayingSwipeView.Close();

            // Or PlayNext/Previous bool
            PlayPreviousSongPart(sender, e);
        }
    }

    internal void UpdatePreviousSwipeItem()
    {
        if (MainViewModel.PlayMode == PlayMode.Playlist && MainViewModel.SongPartHistory.Count > 0)
        {
            SetPreviousSwipeItem(isVisible: true, songPart: MainViewModel.SongPartHistory[MainViewModel.SongPartHistory.Count - 1]);

        }
        else if (MainViewModel.PlayMode == PlayMode.Queue && MainViewModel.SongPartHistory.Count > 0)
        {
            SetPreviousSwipeItem(isVisible: true, songPart: MainViewModel.SongPartHistory[MainViewModel.SongPartHistory.Count - 1]);
        }
        else
        {
            SetPreviousSwipeItem(isVisible: false, songPart: null);
        }
    }

    private void SetPreviousSwipeItem(bool isVisible, SongPart? songPart)
    {
        PreviousSwipeItem.IsVisible = isVisible;
        PreviousSwipeItemTitle.Text = isVisible ? songPart!.Title : string.Empty;
        PreviousSwipeItemSongPart.Text = isVisible ? songPart!.PartNameFull : string.Empty;
    }

    internal void UpdateNextSwipeItem()
    {
        if (MainViewModel.PlayMode == PlayMode.Playlist && MainViewModel.PlaylistQueue.Count > 0)
        {
            SetNextSwipeItem(isVisible: true, songPart: MainViewModel.PlaylistQueue.Peek());
        }
        else if (MainViewModel.PlayMode == PlayMode.Queue && MainViewModel.SongPartsQueue.Count > 0)
        {
            SetNextSwipeItem(isVisible: true, songPart: MainViewModel.SongPartsQueue.Peek());
        }
        else
        {
            SetNextSwipeItem(isVisible: false, songPart: null);
        }
    }

    private void SetNextSwipeItem(bool isVisible, SongPart? songPart)
    {
        NextSwipeItem.IsVisible = isVisible;
        NextSwipeItemTitle.Text = isVisible ? songPart!.Title : string.Empty;
        NextSwipeItemSongPart.Text = isVisible ? songPart!.PartNameFull : string.Empty;
    }

    private void ViewSongPartDetailsTapped(object sender, TappedEventArgs e)
    {
        if (MainViewModel.CurrentSongPart is null || MainViewModel.CurrentSongPart.Id < 0) { return; }

        ShowDetails?.Invoke(sender, e);
    }
}