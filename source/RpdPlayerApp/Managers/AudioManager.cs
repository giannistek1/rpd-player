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

    #region Events
    internal static EventHandler? OnPlay { get; set; }
    internal static EventHandler? OnPause { get; set; }
    internal static EventHandler? OnStop { get; set; }

    internal static event EventHandler<MyEventArgs>? OnChange;
    #endregion

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

    internal static void SetAnnouncement() => PreSongPartMediaElement!.Source = GetAnnouncementMediaSource();

    private static MediaSource? GetAnnouncementMediaSource()
    {
        return AppState.CountdownMode switch
        {
            //CountdownModeEnum.Short => MediaSource.FromResource("countdown-short.mp3"),
            //CountdownModeEnum.Long => MediaSource.FromResource("countdown-long.mp3"),
            //CountdownModeEnum.Custom => MediaSource.FromResource("countdown-kart.mp3"),
            _ => null
        };
    }

    internal static void SetTimer() => PreSongPartMediaElement!.Source = GetTimerMediaSource();

    private static MediaSource? GetTimerMediaSource()
    {
        return AppState.CountdownMode switch
        {
            CountdownModeEnum.Short => MediaSource.FromResource("countdown-short.mp3"),
            CountdownModeEnum.Long => MediaSource.FromResource("countdown-long.mp3"),
            CountdownModeEnum.Custom => MediaSource.FromResource("countdown-kart.mp3"),
            _ => null
        };
    }

    internal static void PlayCountdown()
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
    internal static void SetMute()
    {
        PreSongPartMediaElement!.ShouldMute = CommonSettings.IsVolumeMuted;
        CurrentPlayer!.ShouldMute = CommonSettings.IsVolumeMuted;
    }

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
            AppState.CurrentlyPlayingState = (AppState.AnnouncementMode > 0) ? CurrentlyPlayingStateEnum.Announcement : CurrentlyPlayingStateEnum.Countdown;
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
    }

    internal static void MoveAudioProgress(TimeSpan differenceInSeconds)
    {
        if (CurrentPlayer is null) { return; }

        var newTimeSpan = CurrentPlayer!.Position.Add(differenceInSeconds);
        CurrentPlayer?.SeekTo(newTimeSpan);
    }



    /// <summary> Change song / Start song </summary>
    /// <param name="songPart"></param>
    internal static void ChangeAndStartSong(SongPart songPart)
    {
        DebugService.Instance.AddDebug(msg: $"ChangeAndStartSong: {(CurrentPlayer == SongPartMediaElement ? "SongPartMediaElement" : "SongPartMediaElement2")}");

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
        SetNextSongAndNextPlayer();

        AnimationManager.songPart = songPart;
        AnimationManager.StartInfiniteScaleYAnimationWithTimer();
    }

    /// <summary> Change song / Next song </summary>
    internal static void PlayNextSong()
    {
        // Reset SongPartHistoryIndex;
        AppState.SongPartHistoryIndex = -1;

        DebugService.Instance.AddDebug(msg: $"PlayNextSong: {(CurrentPlayer == SongPartMediaElement ? "SongPartMediaElement" : "SongPartMediaElement2")}");

        // Check repeat one mode.
        if (AppState.AutoplayMode == AutoplayModeEnum.RepeatOne)
        {
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
            DebugService.Instance.AddDebug(msg: $"PlayNextSong: NextSongPart.AudioURL is empty");
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
        SetNextSongAndNextPlayer();

        //AnimationManager.songPart = songPart;
        //AnimationManager.StartInfiniteScaleYAnimationWithTimer();
    }

    /// <summary> Change song / Previous song </summary>
    internal static void PlayPreviousSong()
    {
        if (AppState.SongPartHistory.Count == 0)
        {
            AppState.CurrentlyPlayingState = CurrentlyPlayingStateEnum.None;
            DebugService.Instance.AddDebug(msg: $"PlayPreviousSong: History is 0");
            return;
        }

        // Initialize index if not set.
        if (AppState.SongPartHistoryIndex == -1)
        {
            AppState.SongPartHistoryIndex = AppState.SongPartHistory.Count - 1;
        }
        else
        {
            AppState.SongPartHistoryIndex = Math.Max(0, AppState.SongPartHistoryIndex - 1);
        }

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
        if (AppState.AnnouncementMode > AnnouncementModeEnum.Off)
        {
            AppState.CurrentlyPlayingState = CurrentlyPlayingStateEnum.Announcement;
            PlayAnnouncement();
            DebugService.Instance.AddDebug(msg: $"Playing announcement.");
        }
        else if (AppState.CountdownMode > CountdownModeEnum.Off)
        {
            PlayCountdown();
            DebugService.Instance.AddDebug(msg: $"Playing countdown.");
        }
        else // No announcement or countdown
        {
            PlayAudio();
            DebugService.Instance.AddDebug(msg: $"Playing only audio.");
        }
    }

    /// <summary> Plays the current audio and updates Play buttons </summary>
    internal static void PlayAudio()
    {
        AppState.CurrentlyPlayingState = CurrentlyPlayingStateEnum.SongPart;
        CurrentPlayer!.Play();
        OnPlay?.Invoke(null, EventArgs.Empty);
    }

    internal static void ChangedAutoplayMode() => SetNextSongAndNextPlayer();
    internal static void AddedToQueue() => SetNextSongAndNextPlayer();

    private static void SetNextSongAndNextPlayer()
    {
        // Choose song
        if (AppState.SongPartsQueue.Count > 0)
        {
            NextPlayer!.Source = MediaSource.FromUri(AppState.SongPartsQueue.Peek().AudioURL);
            AppState.NextSongPart = AppState.SongPartsQueue.Dequeue();
            return;
        }
        else if (AppState.AutoplayMode == AutoplayModeEnum.Autoplay)
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