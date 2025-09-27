using RpdPlayerApp.Architecture;
using RpdPlayerApp.Enums;
using RpdPlayerApp.Managers;
using RpdPlayerApp.Models;
using RpdPlayerApp.ViewModels;
using The49.Maui.BottomSheet;

namespace RpdPlayerApp.Views;

public partial class SongPartDetailBottomSheet
{
    internal SongPart? songPart = null;
    internal bool isShown = false;

    internal EventHandler? PlayToggleSongPart;
    internal EventHandler? PreviousSongPart;
    internal EventHandler? NextSongPart;
    internal EventHandler? FavoriteSongPart;
    internal EventHandler? Close;

    internal AudioPlayerControl? AudioPlayerControl { get; set; }

    private const string FAVORITES = "Favorites";
    private readonly PlaylistsManager _playlistsManager;

    public SongPartDetailBottomSheet()
    {
        InitializeComponent();
        _playlistsManager = new();

        Loaded += OnLoad;
        Dismissed += OnDismissed;

        AudioManager.OnChange += OnChangeSongPart;
        AudioManager.OnPlay += OnPlayAudio;
        AudioManager.OnPause += OnPauseAudio;
        AudioManager.OnStop += OnStopAudio;

        StartAlbumAutoScroll();
    }

    private void OnLoad(object? sender, EventArgs e)
    {
        FavoriteImageButton.Source = _playlistsManager.SongPartIsInPlaylist(FAVORITES, songPart) ? IconManager.FavoritedIcon : IconManager.FavoriteIcon;

        AudioProgressSlider.DragStarted += AudioProgressSlider_DragStarted;
        AudioProgressSlider.DragCompleted += AudioProgressSlider_DragCompleted;
    }

    private void OnDismissed(object? sender, DismissOrigin e) => isShown = false;

    private void OnChangeSongPart(object? sender, MyEventArgs e) => UpdateSongDetails();

    private void OnPlayAudio(object? sender, EventArgs e) => PlayToggleImageButton.Source = IconManager.PauseIcon;

    private void OnPauseAudio(object? sender, EventArgs e) => PlayToggleImageButton.Source = IconManager.PlayIcon;

    private void OnStopAudio(object? sender, EventArgs e) => PlayToggleImageButton.Source = IconManager.PlayIcon;

    private void AudioProgressSlider_DragStarted(object? sender, EventArgs e) => AudioPlayerControl!.AudioProgressSliderDragStarted(sender, e);

    private void AudioProgressSlider_DragCompleted(object? sender, EventArgs e) => AudioPlayerControl?.SeekToProgress(sender, e);

    /// <summary> Depends on whenever play mode changes. </summary>
    internal void UpdateIcons()
    {
        // TODO: Set image to play once song ends.
        //PlayToggleImageButton.Source = AppState.CurrentSongPart.IsPlaying || AppState.IsCurrentlyPlayingTimer ? IconManager.PauseIcon : IconManager.PlayIcon;
        PlayToggleImageButton.Source = (AppState.CurrentlyPlayingState == CurrentlyPlayingStateEnum.SongPart
                                    || AppState.CurrentlyPlayingState == CurrentlyPlayingStateEnum.Countdown
                                    || AppState.CurrentlyPlayingState == CurrentlyPlayingStateEnum.Announcement)
                                    ? IconManager.PauseIcon : IconManager.PlayIcon;

        VoiceImageButton.Source = AppState.UsingAnnouncements ? IconManager.VoiceIcon : IconManager.VoiceOffIcon;
        // TODO: Enums
        TimerImageButton.Source = AppState.CountdownMode switch
        {
            0 => IconManager.TimerOffIcon,
            1 => IconManager.Timer3Icon,
            2 => IconManager.Timer5Icon,
            _ => IconManager.TimerOffIcon
        };

        AutoplayImageButton.Source = AppState.AutoplayMode switch
        {
            AutoplayModeEnum.Off => IconManager.OffIcon,
            AutoplayModeEnum.Autoplay => IconManager.AutoplayIcon,
            AutoplayModeEnum.Shuffle => IconManager.ShuffleIcon,
            AutoplayModeEnum.RepeatOne => IconManager.RepeatOneIcon,
            _ => IconManager.OffIcon
        };

        MasterVolumeSlider.Value = CommonSettings.MainVolume * 100;
        // Theme change or change song should update favorite icon.
        FavoriteImageButton.Source = _playlistsManager.SongPartIsInPlaylist(FAVORITES, songPart) ? IconManager.FavoritedIcon : IconManager.FavoriteIcon;
    }

    /// <summary> Song dependent or theme change. </summary>
    internal void UpdateSongDetails()
    {
        if (songPart is null) { return; }

        AlbumImage.Source = ImageSource.FromUri(new Uri(songPart.AlbumURL));

        AlbumLabel.Text = $"{songPart.AlbumTitle} ";
        AlbumLabel.TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"];
        ReleaseDateLabel.Text = $"{songPart.Album.ReleaseDate:d}";
        ReleaseDateLabel.TextColor = (Color)Application.Current!.Resources["SecondaryTextColor"];
        GenreLabel.Text = $"{songPart.Album.GenreFull}";
        GenreLabel.TextColor = (Color)Application.Current!.Resources["SecondaryTextColor"];

        SongTitleLabel.Text = $"{songPart.Title}";
        SongTitleLabel.TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"];
        SongPartLabel.Text = $"{songPart.PartNameFull}";
        SongPartLabel.TextColor = (Color)Application.Current!.Resources["SecondaryTextColor"];
        ArtistLabel.Text = songPart.ArtistName;
        ArtistLabel.TextColor = (Color)Application.Current!.Resources["SecondaryTextColor"];

        TimeSpan duration = TimeSpan.FromSeconds(songPart.ClipLength);

        DurationLabel.Text = string.Format("{0:mm\\:ss}", duration);
        DurationLabel.TextColor = (Color)Application.Current!.Resources["SecondaryTextColor"];

        ProgressLabel.TextColor = (Color)Application.Current!.Resources["SecondaryTextColor"];

        VolumeImageButton.Source = CommonSettings.IsVolumeMuted ? IconManager.NoSoundIcon : IconManager.SoundIcon;
        MasterVolumeSlider.TrackStyle.ActiveFill = (Color)Application.Current!.Resources["Primary"];
        MasterVolumeSlider.ThumbStyle.Fill = (Color)Application.Current!.Resources["Primary"];

        // Theme change or change song should update favorite icon.
        FavoriteImageButton.Source = _playlistsManager.SongPartIsInPlaylist(FAVORITES, songPart) ? IconManager.FavoritedIcon : IconManager.FavoriteIcon;
    }

    internal void UpdateProgress(double value)
    {
        AudioProgressSlider.Value = value;

        if (songPart is not null)
        {
            TimeSpan duration = TimeSpan.FromSeconds(value / 100 * songPart.ClipLength);

            ProgressLabel.Text = string.Format("{0:mm\\:ss}", duration);
        }
        else
        {
            ProgressLabel.Text = "0:00";
        }
    }

    // Let AudioPlayerControl handle the events.
    private void PlayToggleButtonPressed(object sender, EventArgs e) => PlayToggleSongPart?.Invoke(sender, e);

    private void PreviousButtonPressed(object sender, EventArgs e) => PreviousSongPart?.Invoke(sender, e);

    private void NextButtonPressed(object sender, EventArgs e) => NextSongPart?.Invoke(sender, e);

    private void TimerButtonPressed(object sender, EventArgs e)
    {
        // Cycle through timermodes.
        if (AppState.CountdownMode < 3)
        {
            AppState.CountdownMode++;
        }
        else
        {
            AppState.CountdownMode = 0;
        }

        switch (AppState.CountdownMode)
        {
            case 0: TimerImageButton.Source = IconManager.TimerOffIcon; break;
            case 1: TimerImageButton.Source = IconManager.Timer3Icon; break;
            case 2: TimerImageButton.Source = IconManager.Timer5Icon; break;
            case 3: TimerImageButton.Source = IconManager.AwardIcon; break;
        }

        AudioManager.SetTimer();
    }

    private void AutoplayButtonPressed(object sender, EventArgs e)
    {
        // Cycle through autoplay modes.
        if (AppState.AutoplayMode < AutoplayModeEnum.RepeatOne)
        {
            AppState.AutoplayMode++;
        }
        else
        {
            AppState.AutoplayMode = AutoplayModeEnum.Off;
        }

        switch (AppState.AutoplayMode)
        {
            case AutoplayModeEnum.Off: AutoplayImageButton.Source = IconManager.OffIcon; break;
            case AutoplayModeEnum.Autoplay: AutoplayImageButton.Source = IconManager.AutoplayIcon; break;
            case AutoplayModeEnum.Shuffle: AutoplayImageButton.Source = IconManager.ShuffleIcon; break;
            case AutoplayModeEnum.RepeatOne: AutoplayImageButton.Source = IconManager.RepeatOneIcon; break;
        }

        AudioManager.ChangedAutoplayMode();

        //Toast.Make($"Autoplay: {AppState.AutoplayModeEnum}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
    }

    private void VoiceButtonPressed(object sender, EventArgs e)
    {
        AppState.UsingAnnouncements = !AppState.UsingAnnouncements;

        VoiceImageButton.Source = AppState.UsingAnnouncements ? IconManager.VoiceIcon : IconManager.VoiceOffIcon;

        if (AppState.UsingAnnouncements)
        {
            General.ShowToast("Using voiced announcements.");
        }
    }

    private void FavoriteButtonPressed(object sender, EventArgs e)
    {
        // TODO: Unfavorite?
        bool success = _playlistsManager.TryAddSongPartToPlaylist(FAVORITES, songPart!);
        if (!success) { return; }

        FavoriteImageButton.Source = IconManager.FavoritedIcon;

        // Update libraryview
        FavoriteSongPart?.Invoke(sender, e);

        General.ShowToast($"Favorited: {songPart?.Title}");
    }

    private void MasterVolumeSlider_ValueChanged(object sender, Syncfusion.Maui.Sliders.SliderValueChangedEventArgs e)
    {
        CommonSettings.MainVolume = e.NewValue / 100;
        Preferences.Set(CommonSettings.MAIN_VOLUME, CommonSettings.MainVolume);
    }

    private void CloseImageButtonPressed(object sender, EventArgs e) => Close?.Invoke(sender, e);

    private void VolumeImageButtonPressed(object sender, EventArgs e)
    {
        CommonSettings.IsVolumeMuted = !CommonSettings.IsVolumeMuted;
        VolumeImageButton.Source = CommonSettings.IsVolumeMuted ? IconManager.NoSoundIcon : IconManager.SoundIcon;
        AudioManager.SetMute();
    }

    private void BackwardsImageButtonPressed(object sender, EventArgs e) => AudioManager.MoveAudioProgress(new TimeSpan(hours: 0, minutes: 0, seconds: -5));

    private void ForwardImageButtonPressed(object sender, EventArgs e) => AudioManager.MoveAudioProgress(new TimeSpan(hours: 0, minutes: 0, seconds: 5));

    private void RestartAudioImageButtonPressed(object sender, EventArgs e) => AudioManager.RestartAudio();

    private const int TimerInterval = 5000; // Scroll every 5 seconds
    private void StartAlbumAutoScroll()
    {
        Dispatcher.StartTimer(TimeSpan.FromMilliseconds(TimerInterval), () =>
        {
            if (AlbumScrollView.ScrollX > 0)
            {
                AlbumScrollView.ScrollToAsync(0, 0, animated: true);
            }
            else
            {
                AlbumScrollView.ScrollToAsync(AlbumGrid.Width, 0, animated: true);
            }

            // Return true to keep the timer running.
            return true;
        });
    }
}