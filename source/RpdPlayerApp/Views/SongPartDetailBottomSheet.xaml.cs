using CommunityToolkit.Maui.Alerts;
using RpdPlayerApp.Models;
using RpdPlayerApp.ViewModels;
using The49.Maui.BottomSheet;
using UraniumUI.Icons.MaterialSymbols;

namespace RpdPlayerApp.Views;

public partial class SongPartDetailBottomSheet
{
    private FontImageSource _pauseIcon = new();
    private FontImageSource _playIcon = new();

    private FontImageSource _offIcon = new();
    private FontImageSource _autoplayIcon = new();
    private FontImageSource _shuffleIcon = new();
    private FontImageSource _repeatOneIcon = new();

    private FontImageSource _timerOffIcon = new();
    private FontImageSource _timer3Icon = new();
    private FontImageSource _timer5Icon = new();

    private FontImageSource _voiceOffIcon = new();
    private FontImageSource _voiceOnIcon = new();

    internal SongPart? songPart = null;
    internal bool isShown = false;

    internal EventHandler? PlayToggleSongPart;
    internal EventHandler? PreviousSong;
    internal EventHandler? NextSong;


	public SongPartDetailBottomSheet()
	{
		InitializeComponent();

        this.Dismissed += OnDismissed;
        this.Shown += OnShown;

        _pauseIcon = new FontImageSource
        {
            FontFamily = "MaterialOutlined",
            Glyph = MaterialOutlined.Pause,
            Color = (Color)Application.Current!.Resources["IconColor"] // TODO: Sucks because this only gets set once.
        };
        _playIcon = new FontImageSource
        {
            FontFamily = "MaterialOutlined",
            Glyph = MaterialOutlined.Play_arrow,
            Color = (Color)Application.Current!.Resources["IconColor"] // TODO: Sucks because this only gets set once.
        };


        _offIcon = new FontImageSource
        {
            FontFamily = "MaterialOutlined",
            Glyph = MaterialOutlined.Block,
            Color = (Color)Application.Current!.Resources["IconColor"] // TODO: Sucks because this only gets set once.
        };
        _autoplayIcon = new FontImageSource
        {
            FontFamily = "MaterialOutlined",
            Glyph = MaterialOutlined.Autoplay,
            Color = (Color)Application.Current!.Resources["IconColor"] // TODO: Sucks because this only gets set once.
        };
        _shuffleIcon = new FontImageSource
        {
            FontFamily = "MaterialOutlined",
            Glyph = MaterialOutlined.Shuffle,
            Color = (Color)Application.Current!.Resources["IconColor"] // TODO: Sucks because this only gets set once.
        };
        _repeatOneIcon = new FontImageSource
        {
            FontFamily = "MaterialOutlined",
            Glyph = MaterialOutlined.Repeat_one,
            Color = (Color)Application.Current!.Resources["IconColor"] // TODO: Sucks because this only gets set once.
        };


        _timerOffIcon = new FontImageSource
        {
            FontFamily = "MaterialOutlined",
            Glyph = MaterialOutlined.Timer_off,
            Color = (Color)Application.Current!.Resources["IconColor"] // TODO: Sucks because this only gets set once.
        };
        _timer3Icon = new FontImageSource
        {
            FontFamily = "MaterialOutlined",
            Glyph = MaterialOutlined.Timer_3_alt_1,
            Color = (Color)Application.Current!.Resources["IconColor"] // TODO: Sucks because this only gets set once.
        };
        _timer5Icon = new FontImageSource
        {
            FontFamily = "MaterialOutlined",
            Glyph = MaterialOutlined.Timer_5,
            Color = (Color)Application.Current!.Resources["IconColor"] // TODO: Sucks because this only gets set once.
        };

        _voiceOffIcon = new FontImageSource
        {
            FontFamily = "MaterialSharp",
            Glyph = MaterialSharp.Voice_selection_off,
            Color = (Color)Application.Current!.Resources["IconColor"] // TODO: Sucks because this only gets set once.
        };
        _voiceOnIcon = new FontImageSource
        {
            FontFamily = "MaterialSharp",
            Glyph = MaterialSharp.Voice_selection,
            Color = (Color)Application.Current!.Resources["IconColor"] // TODO: Sucks because this only gets set once.
        };
    }

    private void OnShown(object? sender, EventArgs e)
    {
        // TODO: Set image to play once song ends
        PlayToggleImage.Source = MainViewModel.CurrentSongPart.IsPlaying ? _pauseIcon : _playIcon;

        MasterVolumeSlider.Value = MainViewModel.MainVolume * 100;
    }

    private void OnDismissed(object? sender, DismissOrigin e)
    {
        isShown = false;
    }

    internal void UpdateUI()
    {
        if (songPart is null) { return; }

        AlbumImage.Source = ImageSource.FromUri(new Uri(songPart.AlbumURL));

        AlbumLabel.Text = $"{songPart.AlbumTitle} ";
        ReleaseDateLabel.Text = $"{songPart.Album.ReleaseDate:d}";
        GenreLabel.Text = $"{songPart.Album.GenreFull}";

        SongTitleLabel.Text = $"{songPart.Title}";
        SongPartLabel.Text = $"{songPart.PartNameFull}";
        ArtistLabel.Text = songPart.ArtistName;

        TimeSpan duration = TimeSpan.FromSeconds(songPart.ClipLength);

        DurationLabel.Text = string.Format("{0:mm\\:ss}", duration);
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

    private void PreviousButton_Pressed(object sender, EventArgs e)
    {
        PreviousSong?.Invoke(sender, e);
    }

    private void PlayToggleButton_Pressed(object sender, EventArgs e)
    {
        PlayToggleSongPart?.Invoke(sender, e);
        PlayToggleImage.Source = MainViewModel.CurrentSongPart.IsPlaying ? _pauseIcon : _playIcon;
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
            case 0: TimerImage.Source = _timerOffIcon; break;
            case 1: TimerImage.Source = _timer3Icon; break;
            case 2: TimerImage.Source = _timer5Icon; break;
        }

        //Toast.Make($"TimerMode: {MainViewModel.TimerMode}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
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
            case 0: AutoplayImage.Source = _offIcon; break;
            case 1: AutoplayImage.Source = _autoplayIcon; break;
            case 2: AutoplayImage.Source = _shuffleIcon; break;
            case 3: AutoplayImage.Source = _repeatOneIcon; break;
        }

        //Toast.Make($"Autoplay: {MainViewModel.AutoplayMode}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
    }

    private void VoiceButton_Pressed(object sender, EventArgs e)
    {
        MainViewModel.UsingAnnouncements = !MainViewModel.UsingAnnouncements;

        VoiceImage.Source = MainViewModel.UsingAnnouncements ? _voiceOnIcon : _voiceOffIcon;

        //Toast.Make($"Using announcements: {MainViewModel.UsingAnnouncements}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
    }

    private void MasterVolumeSlider_ValueChanged(object sender, Syncfusion.Maui.Sliders.SliderValueChangedEventArgs e)
    {
        MainViewModel.MainVolume = e.NewValue / 100;
    }
}