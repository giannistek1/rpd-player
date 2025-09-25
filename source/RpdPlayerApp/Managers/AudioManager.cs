using CommunityToolkit.Maui.Views;
using RpdPlayerApp.Architecture;
using RpdPlayerApp.Models;
using RpdPlayerApp.ViewModels;
using RpdPlayerApp.Views;

namespace RpdPlayerApp.Managers;

internal static class AudioManager
{
    /// <summary> For countdowns and announcements. </summary>
    internal static MediaElement? PreSongPartMediaElement { get; set; }

    /// <summary> MediaElement that is currently playing songpart. </summary>
    internal static MediaElement? CurrentPlayer { get; set; }

    /// <summary> First of two the MediaElements for the songparts. </summary>
    internal static MediaElement? SongPartMediaElement { get; set; }
    /// <summary> Seocond of the two MediaElements. To ensure smooth transitioning, a hidden second mediaElement plays the next song. </summary>
    internal static MediaElement? SongPartMediaElement2 { get; set; }

    internal static SongPartDetailBottomSheet? DetailBottomSheet { get; set; }

    internal static EventHandler? OnPlay { get; set; }
    internal static EventHandler? OnPause { get; set; }
    internal static EventHandler? OnStop { get; set; }

    internal static event EventHandler<MyEventArgs>? OnChange;

    /// <summary> Sets media source and updates UI with invoke. </summary>
    internal static void ChangeSongPart(SongPart songPart)
    {
        DetailBottomSheet!.songPart = songPart;
        //if (CurrentPlayer == SongPartMediaElement && !IsFirstSong)
        //{
        //    SongPartMediaElement2!.Source = MediaSource.FromUri(songPart.AudioURL);
        //}
        //else
        //{
        //    SongPartMediaElement!.Source = MediaSource.FromUri(songPart.AudioURL);
        //}
        //SongPartMediaElement!.Source = MediaSource.FromUri(songPart.AudioURL);
        CurrentPlayer!.Source = MediaSource.FromUri(songPart.AudioURL);

        OnChange?.Invoke(null, new MyEventArgs(songPart));
    }

    //internal static bool IsFirstSong { get; set; } = true;
    //internal static bool SwitchPlayers { get; set; } = true;

    //internal static void SwitchCurrentPlayer()
    //{
    //    // Switch players.
    //    if (SwitchPlayers && CurrentPlayer == SongPartMediaElement)
    //    {
    //        CurrentPlayer!.Stop();
    //        CurrentPlayer = SongPartMediaElement2;
    //    }
    //    else if (SwitchPlayers)
    //    {
    //        CurrentPlayer!.Stop();
    //        CurrentPlayer = SongPartMediaElement;
    //    }
    //}

    /// <summary>
    /// Precondition: Internet connection. Use ChangeSongPart first or change the source. <br />
    /// Plays current songpart and alerts subscribers.
    /// </summary>
    /// <param name="songPart"> </param>
    internal static void PlayAudio(SongPart songPart)
    {
        if (!General.HasInternetConnection()) { return; }

        // Update variables.
        songPart.IsPlaying = true;
        AppState.IsCurrentlyPlayingSongPart = true;
        AppState.CurrentSongPart.IsPlaying = true;

        OnPlay?.Invoke(null, EventArgs.Empty);

        //SongPartMediaElement!.Play();
        CurrentPlayer!.Play();

        //DebugService.Instance.AddDebug(msg: $"SwitchPlayers: {SwitchPlayers}");
        DebugService.Instance.AddDebug(msg: $"Player: {(CurrentPlayer == SongPartMediaElement ? "SongPartMediaElement" : "SongPartMediaElement2")}");

        AnimationManager.songPart = songPart;
        AnimationManager.StartInfiniteScaleYAnimationWithTimer();

        //if (IsFirstSong)
        //{
        //    IsFirstSong = false;
        //    AudioManager.SwitchPlayers = true;
        //}
    }

    /// <summary> Pauses whatever audio clip is playing now. </summary>
    internal static void PauseAudio()
    {
        PreSongPartMediaElement!.Pause();
        //SongPartMediaElement!.Pause();
        CurrentPlayer!.Pause();
        AppState.CurrentSongPart.IsPlaying = false;

        OnPause?.Invoke(null, EventArgs.Empty);
    }

    /// <summary> Stops whatever audio clip is playing now. </summary>
    internal static void StopAudio()
    {
        if (AppState.CurrentSongPart.Id >= 0)
        {
            //SongPartMediaElement!.Stop();
            //SongPartMediaElement.SeekTo(new TimeSpan(0));

            CurrentPlayer!.Stop();
            CurrentPlayer.SeekTo(new TimeSpan(0));

            AppState.CurrentSongPart.IsPlaying = false;
            AppState.IsCurrentlyPlayingSongPart = false;

            OnStop?.Invoke(null, EventArgs.Empty);
        }
    }

    internal static void SetTimer() => PreSongPartMediaElement!.Source = GetTimerMediaSource();

    private static MediaSource? GetTimerMediaSource()
    {
        return AppState.TimerMode switch
        {
            1 => MediaSource.FromResource("countdown-short.mp3"),
            2 => MediaSource.FromResource("countdown-long.mp3"),
            3 => MediaSource.FromResource("countdown-kart.mp3"), // Special
            _ => null
        };
    }

    internal static void PlayTimer()
    {
        AppState.IsCurrentlyPlayingSongPart = false;
        AppState.IsCurrentlyPlayingTimer = true;

        OnPlay?.Invoke(null, EventArgs.Empty);

        PreSongPartMediaElement!.Play();
    }
    // TODO: Invoke OnMute to update audio icons.
    //internal static void SetMute() => SongPartMediaElement!.ShouldMute = CommonSettings.IsVolumeMuted;
    internal static void SetMute() => CurrentPlayer!.ShouldMute = CommonSettings.IsVolumeMuted;

    internal static void RestartAudio()
    {
        // TODO: RestartCountdown?
        if (AppState.IsCurrentlyPlayingSongPart && AppState.CurrentSongPart is not null)
        {
            //SongPartMediaElement?.SeekTo(new TimeSpan(0));
            CurrentPlayer?.SeekTo(new TimeSpan(0));
            //SwitchPlayers = false;
            PlayAudio(AppState.CurrentSongPart);
        }
    }

    internal static void MoveAudioProgress(TimeSpan differenceInSeconds)
    {
        //if (SongPartMediaElement is null) { return; }
        if (CurrentPlayer is null) { return; }

        //var newTimeSpan = SongPartMediaElement!.Position.Add(differenceInSeconds);
        //SongPartMediaElement?.SeekTo(newTimeSpan);

        var newTimeSpan = CurrentPlayer!.Position.Add(differenceInSeconds);
        CurrentPlayer?.SeekTo(newTimeSpan);
    }
}