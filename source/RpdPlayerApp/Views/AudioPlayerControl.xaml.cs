using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views;
using RpdPlayerApp.Architecture;
using RpdPlayerApp.Enums;
using RpdPlayerApp.Managers;
using RpdPlayerApp.Models;
using RpdPlayerApp.ViewModels;

namespace RpdPlayerApp.Views;

public partial class AudioPlayerControl : ContentView
{
    internal EventHandler? Pause;
    internal EventHandler? ShowDetails;
    internal EventHandler? UpdateProgress;

    internal Slider? audioProgressSlider;

    public AudioPlayerControl()
    {
        InitializeComponent();
        AudioManager.SongPartMediaElement = AudioMediaElement;
        AudioManager.PreSongPartMediaElement = LocalAudioMediaElement;
        Loaded += OnLoad;
    }

    private void OnLoad(object? sender, EventArgs e)
    {
        audioProgressSlider = AudioProgressSlider;

        AudioMediaElement.MediaEnded += AudioMediaElementMediaEnded;
        AudioMediaElement.PositionChanged += AudioMediaElementPositionChanged;
        LocalAudioMediaElement.MediaEnded += LocalAudioMediaElement_MediaEnded;

        AudioProgressSlider.DragStarted += AudioProgressSliderDragStarted;
        AudioProgressSlider.DragCompleted += AudioProgressSliderDragCompleted;

        PlayToggleImageButton.Source = IconManager.PlayIcon;
        AudioManager.OnChange += OnChangeSongPart;
        AudioManager.OnPlay += OnPlayAudio;
        AudioManager.OnPause += OnPauseAudio;
        AudioManager.OnStop += OnStopAudio;
    }

    private void ViewSongPartDetailsTapped(object sender, TappedEventArgs e)
    {
        if (MainViewModel.CurrentSongPart is null || MainViewModel.CurrentSongPart.Id < 0) { return; }

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

    /// <summary>
    /// Plays audio from audioURL, updates AudioPlayerControl controls
    /// </summary>
    /// <param name="songPart"></param>
    internal void PlayAudio(SongPart songPart, bool updateCurrentSong = false)
    {
        if (updateCurrentSong)
        {
            AudioManager.ChangeSongPart(songPart);
        }

        AudioManager.PlayAudio(songPart: songPart);

        // This is very slow :(
        //CheckValidUrl(songPart);

        UpdateNextSwipeItem();
    }


    // For updating theme.
    internal void UpdateUI()
    {
        PlayToggleImageButton.Source = MainViewModel.IsCurrentlyPlayingSongPart ? IconManager.PauseIcon : IconManager.PlayIcon;
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

    #endregion

    private void AudioMediaElementPositionChanged(object? sender, MediaPositionChangedEventArgs e)
    {
        // Can also use a timer and update value on main thread after time elapsed.
        AudioProgressSlider.Value = e.Position.TotalSeconds / AudioMediaElement.Duration.TotalSeconds * 100;

        AudioMediaElement.Volume = CommonSettings.MainVolume;

        UpdateProgress?.Invoke(sender, e);
    }

    private void AudioMediaElementMediaEnded(object? sender, EventArgs e)
    {
        switch (MainViewModel.PlayMode)
        {
            case PlayMode.Queue:

                MainViewModel.SongPartHistory.Add(MainViewModel.CurrentSongPart);
                 
                // Choose song
                if (MainViewModel.AutoplayMode == 1) // Autoplay
                {
                    int index = MainViewModel.SongParts.FindIndex(s => s.AudioURL == MainViewModel.CurrentSongPart.AudioURL);

                    // Imagine index = 1220, count = 1221
                    if (index + 1 < MainViewModel.SongParts.Count)
                    {
                        MainViewModel.SongPartsQueue.Enqueue(MainViewModel.SongParts[index + 1]);
                    }
                }
                else if (MainViewModel.AutoplayMode == 2) // Shuffle
                {
                    int index = HelperClass.Rng.Next(MainViewModel.SongParts.Count);
                    MainViewModel.SongPartsQueue.Enqueue(MainViewModel.SongParts[index]);
                }
                else if (MainViewModel.AutoplayMode == 3) // Repeat one
                {
                    // TOOD: Rewrite with audioplayer.LoopPlayback = true (saves data and performance?)
                    MainViewModel.SongPartsQueue.Enqueue(MainViewModel.CurrentSongPart);
                }

                if (MainViewModel.SongPartsQueue.Count == 0) {
                    if (AudioProgressSlider.Value >= AudioProgressSlider.Maximum - 2) { AudioManager.StopAudio(); }
                    return; 
                }

                // Next song
                MainViewModel.CurrentSongPart = MainViewModel.SongPartsQueue.Dequeue();
                if (MainViewModel.TimerMode > 0)
                {
                    PlayCountdownAndUpdateCurrentSong();
                }
                else
                {
                    PlayAudio(MainViewModel.CurrentSongPart, updateCurrentSong: true);
                }

                SetPreviousSwipeItem(isVisible: true, songPart: MainViewModel.SongPartHistory[^1]); //^1 means count minus 1

                if (MainViewModel.SongPartsQueue.Count > 0)
                {
                    SetNextSwipeItem(isVisible: true, songPart: MainViewModel.SongPartsQueue.Peek());
                }

                break;

            case PlayMode.Playlist:

                // TODO: When play new song, clear queue, add queue?

                MainViewModel.SongPartHistory.Add(MainViewModel.CurrentSongPart);

                // If queue empty, return
                if (MainViewModel.PlaylistQueue.Count == 0) { return; }
 
                MainViewModel.CurrentSongPart = MainViewModel.PlaylistQueue.Dequeue();
                if (MainViewModel.TimerMode >= 1)
                {
                    PlayCountdownAndUpdateCurrentSong();
                }
                else
                {
                    PlayAudio(MainViewModel.CurrentSongPart, updateCurrentSong: true);
                }

                SetPreviousSwipeItem(isVisible: true, songPart: MainViewModel.SongPartHistory[^1]);

                if (MainViewModel.PlaylistQueue.Count > 0)
                {
                    SetNextSwipeItem(isVisible: true, songPart: MainViewModel.PlaylistQueue.Peek());
                }

                break;
        }
    }


    /// <summary>
    /// Also(re)used with SongPartDetailBottomSheet.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    internal void PlayToggleButton_Pressed(object sender, EventArgs e)
    {
        if (MainViewModel.IsCurrentlyPlayingSongPart)
        {
            // If audio is done playing.
            if (AudioMediaElement.CurrentState == MediaElementState.Stopped && AudioMediaElement.Position >= AudioMediaElement.Duration)
            {
                AudioManager.PlayAudio(MainViewModel.CurrentSongPart);
            }
            // If audio is paused (in middle).
            else if (AudioMediaElement.CurrentState == MediaElementState.Paused || AudioMediaElement.CurrentState == MediaElementState.Stopped)
            {
                AudioManager.PlayAudio(MainViewModel.CurrentSongPart);
            }
            // Else pause.
            else if (AudioMediaElement.CurrentState == MediaElementState.Playing)
            {
                AudioManager.PauseAudio();
            }
        }
        else
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
        if (MainViewModel.SongPartHistory.Count > 0)
        {
            MainViewModel.CurrentSongPart = MainViewModel.SongPartHistory[^1]; // Set current song with 2nd last song. ^ Means from end.

            if (MainViewModel.TimerMode >= 1)
            {
                PlayCountdownAndUpdateCurrentSong();
            }
            else
            {
                PlayAudio(MainViewModel.SongPartHistory[^1], updateCurrentSong: true);
            }
            MainViewModel.SongPartHistory.RemoveAt(MainViewModel.SongPartHistory.Count - 1);

            UpdatePreviousSwipeItem();
        }
    }

    internal void NextButton_Pressed(object sender, EventArgs e) => AudioMediaElementMediaEnded(sender, e);

    internal void PlayCountdownAndUpdateCurrentSong()
    {
        switch(MainViewModel.TimerMode)
        {
            case 1: LocalAudioMediaElement.Source = MediaSource.FromResource("countdown-short.mp3"); break;
            case 2: LocalAudioMediaElement.Source = MediaSource.FromResource("countdown-long.mp3"); break;
            case 3: LocalAudioMediaElement.Source = MediaSource.FromResource("countdown-kart.mp3"); break;
        }

        AudioManager.PlayTimer();

        AudioManager.ChangeSongPart(MainViewModel.CurrentSongPart);
    }

    private void LocalAudioMediaElement_MediaEnded(object? sender, EventArgs e)
    {
        MainViewModel.IsCurrentlyPlayingTimer = false;
        PlayAudio(MainViewModel.SongToPlay);
    }

    internal void StopAudio()
    {
        AudioMediaElement.Stop();
        AudioMediaElement.SeekTo(new TimeSpan(0));

        MainViewModel.CurrentSongPart.IsPlaying = false;
        MainViewModel.IsCurrentlyPlayingSongPart = false;
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

    private void NowPlayingSwipeViewSwipeEnded(object sender, SwipeEndedEventArgs e)
    {
        // Next song.
        if (e.SwipeDirection == SwipeDirection.Left && e.IsOpen)
        {
            // TODO: Probably delete, UGLY, needs fadeout or a delay or text invisible, be creative (PROBABLY CHANGE TO CAROUSELVIEW?).

            NowPlayingSwipeView.Close();
            AudioMediaElementMediaEnded(sender, e);
        }
        // Previous song
        else if (e.SwipeDirection == SwipeDirection.Right && e.IsOpen)
        {
            // TODO: Probably delete, UGLY, needs fadeout or a delay or text invisible, be creative (PROBABLY CHANGE TO CAROUSELVIEW?).

            NowPlayingSwipeView.Close();

            // Or PlayNext/Previous bool
            PlayPreviousSongPart(sender, e);
        }
    }

    internal void UpdatePreviousSwipeItem()
    {
        if (MainViewModel.PlayMode == PlayMode.Playlist && MainViewModel.SongPartHistory.Count > 0)
        {
            SetPreviousSwipeItem(isVisible: true, songPart: MainViewModel.SongPartHistory[^1]);
        }
        else if (MainViewModel.PlayMode == PlayMode.Queue && MainViewModel.SongPartHistory.Count > 0)
        {
            SetPreviousSwipeItem(isVisible: true, songPart: MainViewModel.SongPartHistory[^1]);
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
}