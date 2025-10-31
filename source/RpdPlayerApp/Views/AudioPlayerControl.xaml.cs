using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core.Primitives;
using RpdPlayerApp.Architecture;
using RpdPlayerApp.Enums;
using RpdPlayerApp.Managers;
using RpdPlayerApp.Models;

namespace RpdPlayerApp.Views;

/// <summary> The contentview at the bottom that has a slider. </summary>
public partial class AudioPlayerControl : ContentView
{
    internal EventHandler? Pause;
    internal EventHandler? ShowDetails;
    internal EventHandler? UpdateProgress;

    internal Slider? AudioSlider;
    internal Slider? PlaylistSlider;

    public AudioPlayerControl()
    {
        InitializeComponent();

        AudioManager.PreSongPartMediaElement = LocalAudioMediaElement;
        AudioManager.SongPartMediaElement = AudioMediaElement;
        AudioManager.SongPartMediaElement2 = AudioMediaElement2;

        AudioManager.CurrentPlayer = AudioManager.SongPartMediaElement;
        AudioManager.NextPlayer = AudioManager.SongPartMediaElement2;

        Loaded += OnLoad;
    }

    private void OnLoad(object? sender, EventArgs e)
    {
        AudioSlider = AudioProgressSlider;

        LocalAudioMediaElement.MediaEnded += LocalAudioMediaElementMediaEnded;

        AudioMediaElement.MediaEnded += AudioMediaElementMediaEnded;
        AudioMediaElement.PositionChanged += AudioMediaElementPositionChanged;

        AudioMediaElement2.MediaEnded += AudioMediaElementMediaEnded;
        AudioMediaElement2.PositionChanged += AudioMediaElementPositionChanged;

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

    // For updating theme.
    internal void UpdateUI()
    {
        PlayToggleImageButton.Source = General.IsOddEnumValue(AppState.CurrentlyPlayingState) ? IconManager.PauseIcon : IconManager.PlayIcon;
        PlayToggleBorder.Background = (Color)Application.Current!.Resources["PrimaryButton"];
    }

    #region AudioProgressSlider

    internal void AudioProgressSliderDragStarted(object? sender, EventArgs e)
    {
        AudioManager.CurrentPlayer!.PositionChanged -= AudioMediaElementPositionChanged;
    }

    private void AudioProgressSliderDragCompleted(object? sender, EventArgs e) => SeekToProgress(sender, e);

    internal void SeekToProgress(object? sender, EventArgs e)
    {
        if (sender is Slider audioSlider)
        {
            AudioManager.CurrentPlayer!.SeekTo(TimeSpan.FromSeconds(audioSlider.Value / 100 * AudioManager.CurrentPlayer!.Duration.TotalSeconds));

            AudioManager.CurrentPlayer.PositionChanged += AudioMediaElementPositionChanged;
        }
    }

    #endregion AudioProgressSlider

    private void AudioMediaElementPositionChanged(object? sender, MediaPositionChangedEventArgs e)
    {
        // Can also use a timer and update value on main thread after time elapsed.
        AudioProgressSlider.Value = e.Position.TotalSeconds / AudioManager.CurrentPlayer!.Duration.TotalSeconds * 100;

        AudioMediaElement.Volume = CommonSettings.MainVolume;
        AudioMediaElement2.Volume = CommonSettings.MainVolume;

        UpdateProgress?.Invoke(sender, e);
    }

    private void AudioMediaElementMediaEnded(object? sender, EventArgs e)
    {
        // Save total activity time.
        CommonSettings.RecalculateTotalActivityTime();
        Preferences.Set(CommonSettings.TOTAL_ACTIVITY_TIME, CommonSettings.TotalActivityTime.ToString());

        AppState.SongPartHistory.Add(AppState.CurrentSongPart);

        AudioManager.PlayNextSong();
    }

    /// <summary> Also (re-)used with SongPartDetailBottomSheet. </summary>
    /// <param name="sender"> </param>
    /// <param name="e"> </param>
    internal void PlayToggleButtonPressed(object sender, EventArgs e) => AudioManager.PlayPause();

    internal void PlayPreviousSongPart(object sender, EventArgs e) => AudioManager.PlayPreviousSong();

    internal void NextButtonPressed(object sender, EventArgs e) => AudioMediaElementMediaEnded(sender, e);

    private void LocalAudioMediaElementMediaEnded(object? sender, EventArgs e)
    {
        if (AppState.CurrentlyPlayingState == CurrentlyPlayingStateValue.Announcement)
        {
            AudioManager.PlayCountdown();
        }
        else
        {
            AudioManager.PlayAudio();
        }
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
            AudioManager.StopAudio();
            Toast.Make($"Media URL of the song is invalid.", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
            SentrySdk.CaptureMessage($"Tried to play Invalid URL: {songPart.AudioURL}");
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