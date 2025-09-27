namespace RpdPlayerApp.Enums;

public enum CurrentlyPlayingStateEnum
{
    /// <summary> AKA Stopped </summary>
    None = 0,
    Announcement = 1,
    AnnouncementPaused = 2,
    Countdown = 3,
    CountdownPaused = 4,
    SongPart = 5,
    SongPartPaused = 6
}
