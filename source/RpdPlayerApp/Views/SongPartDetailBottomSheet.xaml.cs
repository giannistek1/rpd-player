using CommunityToolkit.Maui.Alerts;
using RpdPlayerApp.Architecture;
using RpdPlayerApp.Models;
using RpdPlayerApp.ViewModels;
using The49.Maui.BottomSheet;

namespace RpdPlayerApp.Views;

public partial class SongPartDetailBottomSheet
{
    internal SongPart? songPart = null;
    internal bool isShown = false;

    internal EventHandler? PlayToggleSongPart;
    internal EventHandler? PreviousSong;
    internal EventHandler? NextSong;
    internal EventHandler? UpdateFavorites;
    internal EventHandler? Close;

    private readonly PlaylistsManager _playlistsManager;

    // TODO: Settings class
    private const string MAIN_VOLUME = "MAIN_VOLUME";

    private const string FAVORITES = "Favorites";

    public SongPartDetailBottomSheet()
	{
		InitializeComponent();

        this.Dismissed += OnDismissed;
        this.Shown += OnShown;

        _playlistsManager = new();
        FavoriteImage.Source = IconManager.FavoriteIcon;
    }

    private void OnShown(object? sender, EventArgs e)
    {
        // TODO: Set image to play once song ends.
        PlayToggleImage.Source = MainViewModel.CurrentSongPart.IsPlaying ? IconManager.PauseIcon : IconManager.PlayIcon;

        MasterVolumeSlider.Value = MainViewModel.MainVolume * 100;

        FavoriteImage.Source = _playlistsManager.IsInPlaylist(FAVORITES, songPart) ? IconManager.FavoritedIcon : IconManager.FavoriteIcon;
    }

    private void OnDismissed(object? sender, DismissOrigin e) => isShown = false;

    internal void UpdateUI()
    {
        if (songPart is null) { return; }

        AlbumImage.Source = ImageSource.FromUri(new Uri(songPart.AlbumURL));

        AlbumLabel.Text = $"{songPart.AlbumTitle} ";
        AlbumLabel.TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"];
        ReleaseDateLabel.Text = $"{songPart.Album.ReleaseDate:d}";
        ReleaseDateLabel.TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"];
        GenreLabel.Text = $"{songPart.Album.GenreFull}";
        GenreLabel.TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"];

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

        MasterVolumeSlider.TrackStyle.ActiveFill = (Color)Application.Current!.Resources["Primary"];
        MasterVolumeSlider.ThumbStyle.Fill = (Color)Application.Current!.Resources["Primary"];
    }

    internal void UpdateProgress(double value)
    {
        AudioProgressSlider.Value = value;

        if (songPart is not null)
        {
            TimeSpan duration = TimeSpan.FromSeconds(value/100 * songPart.ClipLength);

            ProgressLabel.Text = string.Format("{0:mm\\:ss}", duration);
        }
        else
        {
            ProgressLabel.Text = "0:00";
        }
    }
    private void PlayToggleButton_Pressed(object sender, EventArgs e)
    {
        PlayToggleSongPart?.Invoke(sender, e);
        PlayToggleImage.Source = MainViewModel.CurrentSongPart.IsPlaying ? IconManager.PauseIcon : IconManager.PlayIcon;
    }

    private void PreviousButton_Pressed(object sender, EventArgs e)
    {
        PreviousSong?.Invoke(sender, e);
    }

    private void NextButton_Pressed(object sender, EventArgs e)
    {
        NextSong?.Invoke(sender, e);
    }

    private void TimerButton_Pressed(object sender, EventArgs e)
    {
        if (MainViewModel.TimerMode < 2)
        {
            MainViewModel.TimerMode++;
        }
        else
        {
            MainViewModel.TimerMode = 0;
        }

        switch (MainViewModel.TimerMode)
        {
            case 0: TimerImage.Source = IconManager.TimerOffIcon; break;
            case 1: TimerImage.Source = IconManager.Timer3Icon; break;
            case 2: TimerImage.Source = IconManager.Timer5Icon; break;
        }

        if (MainViewModel.TimerMode != 0)
        {
            Toast.Make($"Using timer between song parts.", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        }
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

        switch(MainViewModel.AutoplayMode)
        {
            case 0: AutoplayImage.Source = IconManager.OffIcon; break;
            case 1: AutoplayImage.Source = IconManager.AutoplayIcon; break;
            case 2: AutoplayImage.Source = IconManager.ShuffleIcon; break;
            case 3: AutoplayImage.Source = IconManager.RepeatOneIcon; break;
        }

        //Toast.Make($"Autoplay: {MainViewModel.AutoplayMode}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
    }

    private void VoiceButton_Pressed(object sender, EventArgs e)
    {
        MainViewModel.UsingAnnouncements = !MainViewModel.UsingAnnouncements;

        VoiceImage.Source = MainViewModel.UsingAnnouncements ? IconManager.VoiceIcon : IconManager.VoiceOffIcon;

        if (MainViewModel.UsingAnnouncements)
        {
            Toast.Make($"Using voiced announcements.", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        }
    }

    private void FavoriteButton_Pressed(object sender, EventArgs e)
    {
        // TODO: Unfavorite?
        bool success = _playlistsManager.TryAddToPlaylist(FAVORITES, songPart!);
        if (!success) { return; }

        FavoriteImage.Source = IconManager.FavoritedIcon;

        // Update libraryview
        //UpdateFavorites?.Invoke(sender, e);

        Toast.Make($"Favorited: {songPart?.Title}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
    }

    private void MasterVolumeSlider_ValueChanged(object sender, Syncfusion.Maui.Sliders.SliderValueChangedEventArgs e)
    {
        MainViewModel.MainVolume = e.NewValue / 100;
        Preferences.Set(MAIN_VOLUME, MainViewModel.MainVolume);
    }

    private void CloseButton_Pressed(object sender, EventArgs e) => Close?.Invoke(sender, e);
}