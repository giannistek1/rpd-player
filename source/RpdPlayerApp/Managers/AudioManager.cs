using CommunityToolkit.Maui.Views;
using RpdPlayerApp.Architecture;
using RpdPlayerApp.Models;
using RpdPlayerApp.ViewModels;
using RpdPlayerApp.Views;

namespace RpdPlayerApp.Managers;


internal static class AudioManager
{
    /// <summary>
    /// For countdowns and announcements.
    /// </summary>
    internal static MediaElement? PreSongPartMediaElement { get; set; }

    /// <summary>
    /// For the songparts.
    /// </summary>
    internal static MediaElement? SongPartMediaElement { get; set; }
    internal static SongPartDetailBottomSheet? DetailBottomSheet { get; set; }

    internal static EventHandler? OnPlay { get; set; }
    internal static EventHandler? OnPause { get; set; }
    internal static EventHandler? OnStop { get; set; }

    internal static event EventHandler<MyEventArgs>? OnChange;

    /// <summary>
    /// Sets media source and updates UI with invoke.
    /// </summary>
    internal static void ChangeSongPart(SongPart songPart)
    {
        DetailBottomSheet!.songPart = songPart;
        SongPartMediaElement!.Source = MediaSource.FromUri(songPart.AudioURL);
        OnChange?.Invoke(null, new MyEventArgs(songPart));
    }
    /// <summary>
    /// Precondition: Internet connection. Use ChangeSongPart first or change the source. <br />
    /// Plays current song and alerts subscribers.
    /// </summary>
    /// <param name="songPart"></param>
    internal static void PlayAudio(SongPart songPart)
    {
        if (!HelperClass.HasInternetConnection()) { return; }

        // Update variables
        songPart.IsPlaying = true;
        MainViewModel.IsCurrentlyPlayingSongPart = true;

        OnPlay?.Invoke(null, EventArgs.Empty);

        SongPartMediaElement!.Play();
        TimerManager.songPart = songPart;
        TimerManager.StartInfiniteScaleYAnimationWithTimer();
    }

    internal static void PauseAudio()
    {
        PreSongPartMediaElement!.Pause();
        SongPartMediaElement!.Pause();
        MainViewModel.CurrentSongPart.IsPlaying = false;

        OnPause?.Invoke(null, EventArgs.Empty);
    }

    internal static void StopAudio()
    {
        if (MainViewModel.CurrentSongPart.Id >= 0) 
        {
            SongPartMediaElement!.Stop();
            SongPartMediaElement.SeekTo(new TimeSpan(0));

            MainViewModel.CurrentSongPart.IsPlaying = false;
            MainViewModel.IsCurrentlyPlayingSongPart = false;

            OnStop?.Invoke(null, EventArgs.Empty);
        } 
    }

    internal static void SetTimer()
    {
        switch (MainViewModel.TimerMode)
        {
            case 1: PreSongPartMediaElement!.Source = MediaSource.FromResource("countdown-short.mp3"); break;
            case 2: PreSongPartMediaElement!.Source = MediaSource.FromResource("countdown-long.mp3"); break;
            case 3: PreSongPartMediaElement!.Source = MediaSource.FromResource("countdown-kart.mp3"); break;
        }
    }

    internal static void PlayTimer()
    {
        MainViewModel.IsCurrentlyPlayingSongPart = false;
        MainViewModel.IsCurrentlyPlayingTimer = true;

        OnPlay?.Invoke(null, EventArgs.Empty);

        PreSongPartMediaElement!.Play();
    }
}
