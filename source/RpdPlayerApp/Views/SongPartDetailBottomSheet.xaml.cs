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
        PlayToggleImageButton.Source = General.IsOddEnumValue(AppState.CurrentlyPlayingState) ? IconManager.PauseIcon : IconManager.PlayIcon;

        // TODO: Expand functionality.
        VoiceImageButton.Source = AppState.AnnouncementMode > AnnouncementModeEnum.Off ? IconManager.VoiceIcon : IconManager.VoiceOffIcon;
        TimerImageButton.Source = AppState.CountdownMode switch
        {
            CountdownModeEnum.Off => IconManager.TimerOffIcon,
            CountdownModeEnum.Short => IconManager.Timer3Icon,
            CountdownModeEnum.Long => IconManager.Timer5Icon,
            CountdownModeEnum.Custom => IconManager.AwardIcon,
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

    private void AutoplayButtonPressed(object sender, EventArgs e)
    {
        // Cycle through autoplay modes.
        if (AppState.AutoplayMode < AutoplayModeEnum.RepeatOne)
        {
            AppState.AutoplayMode++;
        }
        else
        {
            AppState.AutoplayMode = AutoplayModeEnum.Off; // 0
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

    private void TimerButtonPressed(object sender, EventArgs e)
    {
        if (AppState.CurrentlyPlayingState == CurrentlyPlayingStateEnum.Countdown)
        {
            General.ShowToast("Let countdown finish or pause first.");
            return;
        }

        // Cycle through timermodes.
        if (AppState.CountdownMode < CountdownModeEnum.Custom)
        {
            AppState.CountdownMode++;
        }
        else
        {
            AppState.CountdownMode = CountdownModeEnum.Off; // 0
        }

        switch (AppState.CountdownMode)
        {
            case CountdownModeEnum.Off: TimerImageButton.Source = IconManager.TimerOffIcon; break;
            case CountdownModeEnum.Short: TimerImageButton.Source = IconManager.Timer3Icon; break;
            case CountdownModeEnum.Long: TimerImageButton.Source = IconManager.Timer5Icon; break;
            case CountdownModeEnum.Custom: TimerImageButton.Source = IconManager.AwardIcon; break;
        }

        // TODO: Maybe going to change once announcements are added.
        AudioManager.SetCountdown();
    }

    private void VoiceButtonPressed(object sender, EventArgs e)
    {
        // Cycle through announcement modes.
        if (AppState.AnnouncementMode < AnnouncementModeEnum.AlwaysSongPart)
        {
            AppState.AnnouncementMode++;
        }
        else
        {
            AppState.AnnouncementMode = AnnouncementModeEnum.Off; // 0
        }

        switch (AppState.AnnouncementMode)
        {
            case AnnouncementModeEnum.Off: VoiceImageButton.Source = IconManager.OffIcon; break;
            case AnnouncementModeEnum.DancebreakOnly: VoiceImageButton.Source = IconManager.VoiceIcon; break;
            case AnnouncementModeEnum.Specific: VoiceImageButton.Source = IconManager.VoiceIcon; break;
            case AnnouncementModeEnum.Artist: VoiceImageButton.Source = IconManager.VoiceIcon; break;
            case AnnouncementModeEnum.GroupType: VoiceImageButton.Source = IconManager.VoiceIcon; break;
            case AnnouncementModeEnum.AlwaysSongPart: VoiceImageButton.Source = IconManager.VoiceIcon; break;
        }

        if (AppState.AnnouncementMode > 0)
        {
            General.ShowToast("Using voiced announcements.");
        }

        // TODO: 
        //AudioManager.SetAnnouncement();
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