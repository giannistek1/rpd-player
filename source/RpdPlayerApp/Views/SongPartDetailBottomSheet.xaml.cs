using CommunityToolkit.Maui.Alerts;
using RpdPlayerApp.Architecture;
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
    }

    private void OnLoad(object? sender, EventArgs e)
    {
        FavoriteImageButton.Source = _playlistsManager.IsInPlaylist(FAVORITES, songPart) ? IconManager.FavoritedIcon : IconManager.FavoriteIcon;

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

    /// <summary> Whenever mode changes dependent. </summary>
    internal void UpdateIcons()
    {
        // TODO: Set image to play once song ends.
        PlayToggleImageButton.Source = MainViewModel.CurrentSongPart.IsPlaying || MainViewModel.IsCurrentlyPlayingTimer ? IconManager.PauseIcon : IconManager.PlayIcon;
        VoiceImageButton.Source = MainViewModel.UsingAnnouncements ? IconManager.VoiceIcon : IconManager.VoiceOffIcon;
        // TODO: Enums
        TimerImageButton.Source = MainViewModel.TimerMode switch
        {
            0 => IconManager.TimerOffIcon,
            1 => IconManager.Timer3Icon,
            2 => IconManager.Timer5Icon,
            _ => IconManager.TimerOffIcon
        };

        AutoplayImageButton.Source = MainViewModel.AutoplayMode switch
        {
            0 => IconManager.OffIcon,
            1 => IconManager.AutoplayIcon,
            2 => IconManager.ShuffleIcon,
            3 => IconManager.RepeatOneIcon,
            _ => IconManager.OffIcon
        };

        MasterVolumeSlider.Value = CommonSettings.MainVolume * 100;
        // Theme change or change song should update favorite icon.
        FavoriteImageButton.Source = _playlistsManager.IsInPlaylist(FAVORITES, songPart) ? IconManager.FavoritedIcon : IconManager.FavoriteIcon;
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
        FavoriteImageButton.Source = _playlistsManager.IsInPlaylist(FAVORITES, songPart) ? IconManager.FavoritedIcon : IconManager.FavoriteIcon;
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
    private void PlayToggleButton_Pressed(object sender, EventArgs e) => PlayToggleSongPart?.Invoke(sender, e);

    private void PreviousButton_Pressed(object sender, EventArgs e) => PreviousSongPart?.Invoke(sender, e);

    private void NextButton_Pressed(object sender, EventArgs e) => NextSongPart?.Invoke(sender, e);

    private void TimerButton_Pressed(object sender, EventArgs e)
    {
        if (MainViewModel.TimerMode < 3)
        {
            MainViewModel.TimerMode++;
        }
        else
        {
            MainViewModel.TimerMode = 0;
        }

        switch (MainViewModel.TimerMode)
        {
            case 0: TimerImageButton.Source = IconManager.TimerOffIcon; break;
            case 1: TimerImageButton.Source = IconManager.Timer3Icon; break;
            case 2: TimerImageButton.Source = IconManager.Timer5Icon; break;
            case 3: TimerImageButton.Source = IconManager.AwardIcon; break;
        }

        AudioManager.SetTimer();
    }

    private void AutoplayButton_Pressed(object sender, EventArgs e)
    {
        if (MainViewModel.AutoplayMode < 3)
        {
            MainViewModel.AutoplayMode++;
        }
        else
        {
            MainViewModel.AutoplayMode = 0;
        }

        switch (MainViewModel.AutoplayMode)
        {
            case 0: AutoplayImageButton.Source = IconManager.OffIcon; break;
            case 1: AutoplayImageButton.Source = IconManager.AutoplayIcon; break;
            case 2: AutoplayImageButton.Source = IconManager.ShuffleIcon; break;
            case 3: AutoplayImageButton.Source = IconManager.RepeatOneIcon; break;
        }

        //Toast.Make($"Autoplay: {MainViewModel.AutoplayMode}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
    }

    private void VoiceButton_Pressed(object sender, EventArgs e)
    {
        MainViewModel.UsingAnnouncements = !MainViewModel.UsingAnnouncements;

        VoiceImageButton.Source = MainViewModel.UsingAnnouncements ? IconManager.VoiceIcon : IconManager.VoiceOffIcon;

        if (MainViewModel.UsingAnnouncements)
        {
            HelperClass.ShowToast("Using voiced announcements.");
        }
    }

    private void FavoriteButton_Pressed(object sender, EventArgs e)
    {
        // TODO: Unfavorite?
        bool success = _playlistsManager.TryAddToPlaylist(FAVORITES, songPart!);
        if (!success) { return; }

        FavoriteImageButton.Source = IconManager.FavoritedIcon;

        // Update libraryview
        FavoriteSongPart?.Invoke(sender, e);

        HelperClass.ShowToast($"Favorited: {songPart?.Title}");
    }

    private void MasterVolumeSlider_ValueChanged(object sender, Syncfusion.Maui.Sliders.SliderValueChangedEventArgs e)
    {
        CommonSettings.MainVolume = e.NewValue / 100;
        Preferences.Set(CommonSettings.MAIN_VOLUME, CommonSettings.MainVolume);
    }

    private void CloseImageButton_Pressed(object sender, EventArgs e) => Close?.Invoke(sender, e);

    private void VolumeImageButton_Pressed(object sender, EventArgs e)
    {
        CommonSettings.IsVolumeMuted = !CommonSettings.IsVolumeMuted;
        VolumeImageButton.Source = CommonSettings.IsVolumeMuted ? IconManager.NoSoundIcon : IconManager.SoundIcon;
        AudioManager.SetMute();
    }

    private void ForwardImageButton_Pressed(object sender, EventArgs e)
    {

    }

    private void ReplayImageButton_Pressed(object sender, EventArgs e)
    {

    }
}