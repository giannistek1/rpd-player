using CommunityToolkit.Maui.Views;
using RpdPlayerApp.Architecture;
using RpdPlayerApp.Enums;
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
    internal static MediaElement? NextPlayer { get; set; }

    /// <summary> First of two the MediaElements for the songparts. </summary>
    internal static MediaElement? SongPartMediaElement { get; set; }
    /// <summary> Seocond of the two MediaElements. To ensure smooth transitioning, a hidden second mediaElement plays the next song. </summary>
    internal static MediaElement? SongPartMediaElement2 { get; set; }

    internal static SongPartDetailBottomSheet? DetailBottomSheet { get; set; }

    internal static EventHandler? OnPlay { get; set; }
    internal static EventHandler? OnPause { get; set; }
    internal static EventHandler? OnStop { get; set; }

    internal static event EventHandler<MyEventArgs>? OnChange;

    /// <summary> Stops whatever audio clip is playing now. </summary>
    internal static void StopAudio()
    {
        if (AppState.CurrentSongPart.Id >= 0)
        {
            CurrentPlayer!.Stop();
            CurrentPlayer.SeekTo(new TimeSpan(0));

            AppState.CurrentlyPlayingState = CurrentlyPlayingStateEnum.SongPartPaused;

            OnStop?.Invoke(null, EventArgs.Empty);
        }
    }

    internal static void SetTimer() => PreSongPartMediaElement!.Source = GetTimerMediaSource();

    private static MediaSource? GetTimerMediaSource()
    {
        return AppState.CountdownMode switch
        {
            1 => MediaSource.FromResource("countdown-short.mp3"),
            2 => MediaSource.FromResource("countdown-long.mp3"),
            3 => MediaSource.FromResource("countdown-kart.mp3"), // Custom
            _ => null
        };
    }

    internal static void PlayCountdownNew()
    {
        AppState.CurrentlyPlayingState = CurrentlyPlayingStateEnum.Countdown;

        OnPlay?.Invoke(null, EventArgs.Empty);

        DebugService.Instance.AddDebug(msg: $"Play countdown: {PreSongPartMediaElement!.Source}");
        PreSongPartMediaElement.SeekTo(new TimeSpan(0));
        PreSongPartMediaElement!.Play();
    }

    internal static void PlayAnnouncement()
    {
        AppState.CurrentlyPlayingState = CurrentlyPlayingStateEnum.Announcement;

        OnPlay?.Invoke(null, EventArgs.Empty);

        DebugService.Instance.AddDebug(msg: $"Play announcement: {PreSongPartMediaElement!.Source}");
        PreSongPartMediaElement.SeekTo(new TimeSpan(0));
        PreSongPartMediaElement!.Play();
    }
    // TODO: Invoke OnMute to update audio icons.
    //internal static void SetMute() => SongPartMediaElement!.ShouldMute = CommonSettings.IsVolumeMuted;
    internal static void SetMute() => CurrentPlayer!.ShouldMute = CommonSettings.IsVolumeMuted;

    internal static void RestartAudio()
    {
        if (AppState.CurrentSongPart is null) { return; }

        if (AppState.CurrentlyPlayingState == CurrentlyPlayingStateEnum.Announcement
            || AppState.CurrentlyPlayingState == CurrentlyPlayingStateEnum.AnnouncementPaused
            || AppState.CurrentlyPlayingState == CurrentlyPlayingStateEnum.Countdown
            || AppState.CurrentlyPlayingState == CurrentlyPlayingStateEnum.CountdownPaused)
        {
            PreSongPartMediaElement!.SeekTo(new TimeSpan(0));
            PreSongPartMediaElement.Play();
            AppState.CurrentlyPlayingState = (AppState.UsingAnnouncements) ? CurrentlyPlayingStateEnum.Announcement : CurrentlyPlayingStateEnum.Countdown;
            OnPlay?.Invoke(null, EventArgs.Empty);
            return;
        }
        else if (AppState.CurrentlyPlayingState == CurrentlyPlayingStateEnum.SongPart
            || AppState.CurrentlyPlayingState == CurrentlyPlayingStateEnum.SongPartPaused)
        {
            CurrentPlayer!.SeekTo(new TimeSpan(0));
            CurrentPlayer.Play();
            AppState.CurrentlyPlayingState = CurrentlyPlayingStateEnum.SongPart;
            OnPlay?.Invoke(null, EventArgs.Empty);
            return;
        }

        //if (AppState.IsCurrentlyPlayingSongPart)
        //{
        //    CurrentPlayer?.SeekTo(new TimeSpan(0));
        //    PlayAudio(AppState.CurrentSongPart);
        //}
    }

    internal static void MoveAudioProgress(TimeSpan differenceInSeconds)
    {
        if (CurrentPlayer is null) { return; }

        var newTimeSpan = CurrentPlayer!.Position.Add(differenceInSeconds);
        CurrentPlayer?.SeekTo(newTimeSpan);
    }

    // New methods

    internal static void PlayPause()
    {
        DebugService.Instance.AddDebug(msg: $"PlayPause: {(CurrentPlayer == SongPartMediaElement ? "SongPartMediaElement" : "SongPartMediaElement2")}");

        if (AppState.CurrentlyPlayingState == CurrentlyPlayingStateEnum.Announcement)
        {
            AppState.CurrentlyPlayingState = CurrentlyPlayingStateEnum.AnnouncementPaused;
            PreSongPartMediaElement!.Pause();
            OnPause?.Invoke(null, EventArgs.Empty);
        }
        else if (AppState.CurrentlyPlayingState == CurrentlyPlayingStateEnum.AnnouncementPaused)
        {
            AppState.CurrentlyPlayingState = CurrentlyPlayingStateEnum.Announcement;
            PreSongPartMediaElement!.Play();
            OnPlay?.Invoke(null, EventArgs.Empty);
        }

        else if (AppState.CurrentlyPlayingState == CurrentlyPlayingStateEnum.Countdown)
        {
            AppState.CurrentlyPlayingState = CurrentlyPlayingStateEnum.CountdownPaused;
            PreSongPartMediaElement!.Pause();
            OnPause?.Invoke(null, EventArgs.Empty);
        }
        else if (AppState.CurrentlyPlayingState == CurrentlyPlayingStateEnum.CountdownPaused)
        {
            AppState.CurrentlyPlayingState = CurrentlyPlayingStateEnum.Countdown;
            PreSongPartMediaElement!.Play();
            OnPlay?.Invoke(null, EventArgs.Empty);
        }

        else if (AppState.CurrentlyPlayingState == CurrentlyPlayingStateEnum.SongPart)
        {
            AppState.CurrentlyPlayingState = CurrentlyPlayingStateEnum.SongPartPaused;
            CurrentPlayer!.Pause();
            OnPause?.Invoke(null, EventArgs.Empty);
        }
        else if (AppState.CurrentlyPlayingState == CurrentlyPlayingStateEnum.SongPartPaused || AppState.CurrentlyPlayingState == CurrentlyPlayingStateEnum.None)
        {
            AppState.CurrentlyPlayingState = CurrentlyPlayingStateEnum.SongPart;
            CurrentPlayer!.Play();
            OnPlay?.Invoke(null, EventArgs.Empty);
        }
    }

    /// <summary> Change song / Start song </summary>
    /// <param name="songPart"></param>
    internal static void ChangeAndStartSongNew(SongPart songPart)
    {
        DebugService.Instance.AddDebug(msg: $"ChangeAndStartSongNew: {(CurrentPlayer == SongPartMediaElement ? "SongPartMediaElement" : "SongPartMediaElement2")}");

        if (!General.HasInternetConnection()) { return; }

        // Reset player.
        CurrentPlayer!.SeekTo(new TimeSpan(0));

        // Update current songpart.
        AppState.CurrentSongPart = songPart;

        // Update UI
        OnChange?.Invoke(null, new MyEventArgs(songPart));
        DetailBottomSheet!.songPart = songPart;

        CurrentPlayer.Source = MediaSource.FromUri(songPart.AudioURL);

        PlayAnnouncementCountdownOrSongPart();
        SetOtherAudioPlayer();

        AnimationManager.songPart = songPart;
        AnimationManager.StartInfiniteScaleYAnimationWithTimer();
    }

    /// <summary> Change song / Next song </summary>
    internal static void NextSongNew()
    {
        // Reset SongPartHistoryIndex;
        AppState.SongPartHistoryIndex = -1;

        DebugService.Instance.AddDebug(msg: $"NextSongNew: {(CurrentPlayer == SongPartMediaElement ? "SongPartMediaElement" : "SongPartMediaElement2")}");

        if (AppState.AutoplayMode == AutoplayModeEnum.RepeatOne)
        {
            // Restart current song.
            CurrentPlayer!.SeekTo(new TimeSpan(0));
            PlayAnnouncementCountdownOrSongPart();
            CurrentPlayer.ShouldLoopPlayback = true;
            return;
        }

        // Stop current player.
        CurrentPlayer!.Stop();
        CurrentPlayer.SeekTo(new TimeSpan(0));
        AppState.CurrentlyPlayingState = CurrentlyPlayingStateEnum.None;

        // Check if next song is valid.
        if (string.IsNullOrEmpty(AppState.NextSongPart.AudioURL))
        {
            OnStop?.Invoke(null, EventArgs.Empty);
            DebugService.Instance.AddDebug(msg: $"NextSongNew: NextSongPart.AudioURL is empty");
            return;
        }

        // Change current player.
        CurrentPlayer = (CurrentPlayer == SongPartMediaElement) ? SongPartMediaElement2 : SongPartMediaElement;
        NextPlayer = (CurrentPlayer == SongPartMediaElement2) ? SongPartMediaElement : SongPartMediaElement2;

        // Update current songpart.
        AppState.CurrentSongPart = AppState.NextSongPart;
        DetailBottomSheet!.songPart = AppState.CurrentSongPart;

        // Update UI
        OnChange?.Invoke(null, new MyEventArgs(AppState.CurrentSongPart));

        PlayAnnouncementCountdownOrSongPart();
        SetOtherAudioPlayer();

        //AnimationManager.songPart = songPart;
        //AnimationManager.StartInfiniteScaleYAnimationWithTimer();
    }

    /// <summary> Change song / Previous song </summary>
    internal static void PreviousSongNew()
    {
        if (AppState.SongPartHistory.Count == 0)
        {
            AppState.CurrentlyPlayingState = CurrentlyPlayingStateEnum.None;
            DebugService.Instance.AddDebug(msg: $"PreviousSongNew: History is 0");
            return;
        }

        // Initialize index if not set
        if (AppState.SongPartHistoryIndex == -1)
            AppState.SongPartHistoryIndex = AppState.SongPartHistory.Count - 1;
        else
            AppState.SongPartHistoryIndex = Math.Max(0, AppState.SongPartHistoryIndex - 1);

        // Stop current player.
        CurrentPlayer!.Stop();
        CurrentPlayer.SeekTo(new TimeSpan(0));

        // Set next player with current song.
        NextPlayer!.Source = MediaSource.FromUri(AppState.CurrentSongPart.AudioURL);
        AppState.NextSongPart = AppState.CurrentSongPart;

        // Set Current player to previous song.
        AppState.CurrentSongPart = AppState.SongPartHistory[AppState.SongPartHistoryIndex];
        DetailBottomSheet!.songPart = AppState.CurrentSongPart;
        CurrentPlayer.Source = AppState.SongPartHistory[AppState.SongPartHistoryIndex].AudioURL;

        // Update UI
        OnChange?.Invoke(null, new MyEventArgs(AppState.CurrentSongPart));

        PlayAnnouncementCountdownOrSongPart();

        //AnimationManager.songPart = songPart;
        //AnimationManager.StartInfiniteScaleYAnimationWithTimer();
    }


    private static void PlayAnnouncementCountdownOrSongPart()
    {
        if (AppState.UsingAnnouncements)
        {
            AppState.CurrentlyPlayingState = CurrentlyPlayingStateEnum.Announcement;
            PlayAnnouncement();
            DebugService.Instance.AddDebug(msg: $"Playing announcement.");
        }
        else if (AppState.CountdownMode > 0)
        {
            PlayCountdownNew();
            DebugService.Instance.AddDebug(msg: $"Playing countdown.");
        }
        else // No announcement or countdown
        {
            PlayAudioNew();
            DebugService.Instance.AddDebug(msg: $"Playing only audio.");
        }
    }

    /// <summary> Plays the current audio and updates Play buttons </summary>
    internal static void PlayAudioNew()
    {
        AppState.CurrentlyPlayingState = CurrentlyPlayingStateEnum.SongPart;
        CurrentPlayer!.Play();
        OnPlay?.Invoke(null, EventArgs.Empty);
    }

    internal static void ChangedAutoplayMode() => SetOtherAudioPlayer();

    private static void SetOtherAudioPlayer()
    {
        // Choose song
        if (AppState.AutoplayMode == AutoplayModeEnum.Autoplay)
        {
            if (CurrentPlayer!.ShouldLoopPlayback) { CurrentPlayer.ShouldLoopPlayback = false; }

            int index = AppState.SongParts.FindIndex(s => s.AudioURL == AppState.CurrentSongPart.AudioURL);

            // Imagine index = 1220, count = 1221
            if (index + 1 < AppState.SongParts.Count)
            {
                NextPlayer!.Source = MediaSource.FromUri(AppState.SongParts[index + 1].AudioURL);
                AppState.NextSongPart = AppState.SongParts[index + 1];
            }
        }
        else if (AppState.AutoplayMode == AutoplayModeEnum.Shuffle)
        {
            if (CurrentPlayer!.ShouldLoopPlayback) { CurrentPlayer.ShouldLoopPlayback = false; }

            int index = General.Rng.Next(AppState.SongParts.Count);

            NextPlayer!.Source = MediaSource.FromUri(AppState.SongParts[index + 1].AudioURL);
            AppState.NextSongPart = AppState.SongParts[index + 1];
        }

        //if (AppState.SongPartsQueue.Count == 0)
        //{
        //    if (AudioProgressSlider.Value >= AudioProgressSlider.Maximum - 2)
        //    {
        //        AudioManager.StopAudio();
        //    }
        //    return;
        //}
    }
}