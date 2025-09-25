using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core.Primitives;
using RpdPlayerApp.Architecture;
using RpdPlayerApp.Enums;
using RpdPlayerApp.Managers;
using RpdPlayerApp.Models;
using RpdPlayerApp.ViewModels;

namespace RpdPlayerApp.Views;

/// <summary> The contentview at the bottom that has a slider. </summary>
public partial class AudioPlayerControl : ContentView
{
    internal EventHandler? Pause;
    internal EventHandler? ShowDetails;
    internal EventHandler? UpdateProgress;

    internal Slider? audioProgressSlider;

    public AudioPlayerControl()
    {
        InitializeComponent();

        AudioManager.PreSongPartMediaElement = LocalAudioMediaElement;
        AudioManager.SongPartMediaElement = AudioMediaElement;
        AudioManager.SongPartMediaElement2 = AudioMediaElement2;
        AudioManager.CurrentPlayer = AudioManager.SongPartMediaElement;

        Loaded += OnLoad;
    }

    private void OnLoad(object? sender, EventArgs e)
    {
        audioProgressSlider = AudioProgressSlider;

        LocalAudioMediaElement.MediaEnded += LocalAudioMediaElement_MediaEnded;

        AudioMediaElement.MediaEnded += AudioMediaElementMediaEnded;
        AudioMediaElement.PositionChanged += AudioMediaElementPositionChanged;

        AudioMediaElement2.MediaEnded += AudioMediaElementMediaEnded;

        AudioProgressSlider.DragStarted += AudioProgressSliderDragStarted;
        AudioProgressSlider.DragCompleted += AudioProgressSliderDragCompleted;

        PlayToggleImageButton.Source = IconManager.PlayIcon;
        AudioManager.OnChange += OnChangeSongPart;
        AudioManager.OnPlay += OnPlayAudio;
        AudioManager.OnPause += OnPauseAudio;
        AudioManager.OnStop += OnStopAudio;

        StartTitleAutoScroll();
    }

    private void ViewSongPartDetailsTapped(object sender, TappedEventArgs e)
    {
        if (AppState.CurrentSongPart is null || AppState.CurrentSongPart.Id < 0) { return; }

        ShowDetails?.Invoke(sender, e);
    }

    private void OnChangeSongPart(object? sender, MyEventArgs e)
    {
        // Update UI.
        AlbumImage.Source = ImageSource.FromUri(new Uri(e.SongPart.AlbumURL));
        NowPlayingLabel.Text = $"{e.SongPart.Title}";
        NowPlayingPartLabel.Text = $"{e.SongPart.PartNameFull}";

        TimeSpan duration = TimeSpan.FromSeconds(e.SongPart.ClipLength);
        DurationLabel.Text = String.Format("{0:mm\\:ss}", duration);
    }

    private void OnPlayAudio(object? sender, EventArgs e) => PlayToggleImageButton.Source = IconManager.PauseIcon;

    private void OnPauseAudio(object? sender, EventArgs e) => PlayToggleImageButton.Source = IconManager.PlayIcon;

    private void OnStopAudio(object? sender, EventArgs e) => PlayToggleImageButton.Source = IconManager.PlayIcon;

    /// <summary> Plays audio from audioURL, updates AudioPlayerControl controls </summary>
    /// <param name="songPart"> </param>
    internal void PlayAudio(SongPart songPart, bool updateCurrentSong = false)
    {
        if (updateCurrentSong)
        {
            AudioManager.ChangeSongPart(songPart);
        }

        AudioManager.PlayAudio(songPart: songPart);

        // This is very slow :(
        //CheckValidUrl(songPart);

        SetNextSongSource();
        UpdateNextSwipeItem(); // Obsolete for now.
    }

    // For updating theme.
    internal void UpdateUI()
    {
        PlayToggleImageButton.Source = AppState.IsCurrentlyPlayingSongPart ? IconManager.PauseIcon : IconManager.PlayIcon;
        PlayToggleBorder.Background = (Color)Application.Current!.Resources["PrimaryButton"];
    }

    #region AudioProgressSlider

    internal void AudioProgressSliderDragStarted(object? sender, EventArgs e) => AudioMediaElement.PositionChanged -= AudioMediaElementPositionChanged;

    private void AudioProgressSliderDragCompleted(object? sender, EventArgs e) => SeekToProgress(sender, e);

    internal void SeekToProgress(object? sender, EventArgs e)
    {
        if (sender is Slider audioSlider)
        {
            AudioMediaElement.SeekTo(TimeSpan.FromSeconds(audioSlider.Value / 100 * AudioMediaElement.Duration.TotalSeconds));

            AudioMediaElement.PositionChanged += AudioMediaElementPositionChanged;
        }
    }

    #endregion AudioProgressSlider

    private void AudioMediaElementPositionChanged(object? sender, MediaPositionChangedEventArgs e)
    {
        // Can also use a timer and update value on main thread after time elapsed.
        AudioProgressSlider.Value = e.Position.TotalSeconds / AudioMediaElement.Duration.TotalSeconds * 100;

        AudioMediaElement.Volume = CommonSettings.MainVolume;

        UpdateProgress?.Invoke(sender, e);
    }

    private void AudioMediaElementMediaEnded(object? sender, EventArgs e)
    {
        // Save total activity time.
        CommonSettings.RecalculateTotalActivityTime();
        Preferences.Set(CommonSettings.TOTAL_ACTIVITY_TIME, CommonSettings.TotalActivityTime.ToString());

        switch (AppState.PlayMode)
        {
            case PlayMode.Queue:

                AppState.SongPartHistory.Add(AppState.CurrentSongPart);

                // Choose song
                if (AppState.AutoplayMode == 1) // Autoplay
                {
                    int index = AppState.SongParts.FindIndex(s => s.AudioURL == AppState.CurrentSongPart.AudioURL);

                    // Imagine index = 1220, count = 1221
                    if (index + 1 < AppState.SongParts.Count)
                    {
                        AppState.SongPartsQueue.Enqueue(AppState.SongParts[index + 1]);
                    }
                }
                else if (AppState.AutoplayMode == 2) // Shuffle
                {
                    int index = General.Rng.Next(AppState.SongParts.Count);
                    AppState.SongPartsQueue.Enqueue(AppState.SongParts[index]);
                }
                else if (AppState.AutoplayMode == 3) // Repeat one
                {
                    // TOOD: Rewrite with audioplayer.LoopPlayback = true (saves data and performance?)
                    AppState.SongPartsQueue.Enqueue(AppState.CurrentSongPart);
                }

                if (AppState.SongPartsQueue.Count == 0)
                {
                    if (AudioProgressSlider.Value >= AudioProgressSlider.Maximum - 2) { AudioManager.StopAudio(); }
                    return;
                }

                // Next song
                AppState.CurrentSongPart = AppState.SongPartsQueue.Dequeue();
                if (AppState.TimerMode > 0)
                {
                    PlayCountdownAndUpdateCurrentSong();
                }
                else
                {
                    PlayAudio(AppState.CurrentSongPart, updateCurrentSong: true);
                }

                SetPreviousSwipeItem(isVisible: true, songPart: AppState.SongPartHistory[^1]); //^1 means count minus 1

                if (AppState.SongPartsQueue.Count > 0)
                {
                    SetNextSwipeItem(isVisible: true, songPart: AppState.SongPartsQueue.Peek());
                }

                break;

            case PlayMode.Playlist:

                // TODO: When play new song, clear queue, add queue?

                AppState.SongPartHistory.Add(AppState.CurrentSongPart);

                // If queue empty, return
                if (AppState.PlaylistQueue.Count == 0) { return; }

                AppState.CurrentSongPart = AppState.PlaylistQueue.Dequeue();
                if (AppState.TimerMode >= 1)
                {
                    PlayCountdownAndUpdateCurrentSong();
                }
                else
                {
                    PlayAudio(AppState.CurrentSongPart, updateCurrentSong: true);
                }

                SetPreviousSwipeItem(isVisible: true, songPart: AppState.SongPartHistory[^1]);

                if (AppState.PlaylistQueue.Count > 0)
                {
                    SetNextSwipeItem(isVisible: true, songPart: AppState.PlaylistQueue.Peek());
                }

                break;
        }
    }

    /// <summary> Also (re-)used with SongPartDetailBottomSheet. </summary>
    /// <param name="sender"> </param>
    /// <param name="e"> </param>
    internal void PlayToggleButton_Pressed(object sender, EventArgs e)
    {
        if (AppState.TimerMode == 0 || AppState.IsCurrentlyPlayingSongPart)
        {
            // If audio is done playing.
            if (AudioMediaElement.CurrentState == MediaElementState.Stopped && AudioMediaElement.Position >= AudioMediaElement.Duration)
            {
                AudioManager.PlayAudio(AppState.CurrentSongPart);
            }
            // If audio is paused (in middle).
            else if (AudioMediaElement.CurrentState == MediaElementState.Paused || AudioMediaElement.CurrentState == MediaElementState.Stopped)
            {
                AudioManager.PlayAudio(AppState.CurrentSongPart);
            }
            // Else pause.
            else if (AudioMediaElement.CurrentState == MediaElementState.Playing)
            {
                AudioManager.PauseAudio();
            }
        }
        else if (AppState.TimerMode > 0)
        {
            // If audio is done playing.
            if (LocalAudioMediaElement.CurrentState == MediaElementState.Stopped && LocalAudioMediaElement.Position >= LocalAudioMediaElement.Duration)
            {
                AudioManager.PlayTimer();
            }
            // If audio is paused (in middle).
            else if (LocalAudioMediaElement.CurrentState == MediaElementState.Paused || LocalAudioMediaElement.CurrentState == MediaElementState.Stopped)
            {
                AudioManager.PlayTimer();
            }
            // Else pause.
            else if (LocalAudioMediaElement.CurrentState == MediaElementState.Playing)
            {
                AudioManager.PauseAudio();
            }
        }
    }

    internal void PlayPreviousSongPart(object sender, EventArgs e)
    {
        if (AppState.SongPartHistory.Count > 0)
        {
            AppState.CurrentSongPart = AppState.SongPartHistory[^1]; // Set current song with 2nd last song. ^ Means from end.

            if (AppState.TimerMode >= 1)
            {
                PlayCountdownAndUpdateCurrentSong();
            }
            else
            {
                PlayAudio(AppState.SongPartHistory[^1], updateCurrentSong: true);
            }
            AppState.SongPartHistory.RemoveAt(AppState.SongPartHistory.Count - 1);

            UpdatePreviousSwipeItem();
        }
    }

    internal void NextButton_Pressed(object sender, EventArgs e) => AudioMediaElementMediaEnded(sender, e);

    internal void PlayCountdownAndUpdateCurrentSong()
    {
        AudioManager.SetTimer();
        AudioManager.PlayTimer();
        AudioManager.ChangeSongPart(AppState.CurrentSongPart);
    }

    private void LocalAudioMediaElement_MediaEnded(object? sender, EventArgs e)
    {
        AppState.IsCurrentlyPlayingTimer = false;

        PlayAudio(AppState.CurrentSongPart);
    }

    internal void StopAudio()
    {
        AudioMediaElement.Stop();
        AudioMediaElement.SeekTo(new TimeSpan(0));

        AppState.CurrentSongPart.IsPlaying = false;
        AppState.IsCurrentlyPlayingSongPart = false;
    }

    private void CheckValidUrl(SongPart songPart)
    {
        using var client = new MyClient();
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

    internal void UpdatePreviousSwipeItem()
    {
        if (AppState.PlayMode == PlayMode.Playlist && AppState.SongPartHistory.Count > 0)
        {
            SetPreviousSwipeItem(isVisible: true, songPart: AppState.SongPartHistory[^1]);
        }
        else if (AppState.PlayMode == PlayMode.Queue && AppState.SongPartHistory.Count > 0)
        {
            SetPreviousSwipeItem(isVisible: true, songPart: AppState.SongPartHistory[^1]);
        }
        else
        {
            SetPreviousSwipeItem(isVisible: false, songPart: null);
        }
    }

    private void SetPreviousSwipeItem(bool isVisible, SongPart? songPart) { }

    internal void UpdateNextSwipeItem()
    {
        if (AppState.PlayMode == PlayMode.Playlist && AppState.PlaylistQueue.Count > 0)
        {
            SetNextSwipeItem(isVisible: true, songPart: AppState.PlaylistQueue.Peek());
        }
        else if (AppState.PlayMode == PlayMode.Queue && AppState.SongPartsQueue.Count > 0)
        {
            SetNextSwipeItem(isVisible: true, songPart: AppState.SongPartsQueue.Peek());
        }
        else
        {
            SetNextSwipeItem(isVisible: false, songPart: null);
        }
    }

    private void SetNextSwipeItem(bool isVisible, SongPart? songPart) { }

    internal void SetNextSongSource()
    {
        if (AppState.PlayMode == PlayMode.Playlist && AppState.PlaylistQueue.Count > 0)
        {
            if (AudioManager.CurrentPlayer == AudioManager.SongPartMediaElement)
            {
                AudioManager.SongPartMediaElement2!.Source = AppState.PlaylistQueue.Peek().AudioURL;
            }
            else
            {
                AudioManager.SongPartMediaElement!.Source = AppState.PlaylistQueue.Peek().AudioURL;
            }
        }
        else if (AppState.PlayMode == PlayMode.Queue && AppState.SongPartsQueue.Count > 0)
        {
            if (AudioManager.CurrentPlayer == AudioManager.SongPartMediaElement)
            {
                AudioManager.SongPartMediaElement2!.Source = AppState.SongPartsQueue.Peek().AudioURL;
            }
            else
            {
                AudioManager.SongPartMediaElement!.Source = AppState.SongPartsQueue.Peek().AudioURL;
            }
        }
        else
        {
            SetNextSwipeItem(isVisible: false, songPart: null);
        }
    }

    private const int TimerInterval = 5000; // Scroll every 5 seconds
    private void StartTitleAutoScroll()
    {
        Dispatcher.StartTimer(TimeSpan.FromMilliseconds(TimerInterval), () =>
        {
            if (TitleScrollView.ScrollX > 0)
            {
                TitleScrollView.ScrollToAsync(0, 0, animated: true);
            }
            else
            {
                TitleScrollView.ScrollToAsync(TitleGrid.Width, 0, animated: true);
            }

            // Return true to keep the timer running
            return true;
        });
    }
}