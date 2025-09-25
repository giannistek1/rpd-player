using RpdPlayerApp.Enums;
using RpdPlayerApp.Items;
using RpdPlayerApp.Models;
using System.Collections.ObjectModel;

namespace RpdPlayerApp.ViewModels;
/// <summary> The "main" state manager. </summary>
internal static class AppState
{
    // Margin/padding order: Left, Top, Right, and Bottom. LTRB (Clockwise from 9)

    #region Modes
    internal static SortMode SortMode { get; set; } = SortMode.Artist;
    internal static SearchFilterMode SearchFilterMode { get; set; }
    internal static string SearchFilterModeText { get; set; } = string.Empty;
    /// <summary> Queue or Playlist. </summary>
    internal static PlayMode PlayMode { get; set; }
    internal static bool UsingVideoMode { get; set; } = false;
    internal static bool UsingCloudMode { get; set; } = false;

    // TODO: Enums
    /// <summary>
    /// 0 = off,
    /// 1 = autoplay,
    /// 2 = shuffle,
    /// 3 = repeat
    /// </summary>
    internal static byte AutoplayMode { get; set; } = 0;

    /// <summary>
    /// 0 = off,
    /// 1 = 3s SHORT,
    /// 2 = 5s LONG,
    /// 3 = Mario kart
    /// </summary>
    internal static byte TimerMode { get; set; } = 0;

    internal static bool UsingAnnouncements { get; set; } = false;

    #endregion

    internal static List<SongPart> SongParts { get; set; } = [];
    /// <summary> Song to play (after countdown). </summary>
    internal static SongPart CurrentSongPart { get; set; } = new();
    internal static Queue<SongPart> PlaylistQueue { get; set; } = new();
    internal static Queue<SongPart> SongPartsQueue { get; set; } = new();
    internal static List<SongPart> SongPartHistory { get; set; } = [];


    /// <summary> Whether a song part is currently playing. TODO: Maybe make this a state machine: States: Announcement, Countdown, SongPart. </summary>
    internal static bool IsCurrentlyPlayingSongPart { get; set; } = false;
    /// <summary> Whether the countdown is currently playing.  TODO: Maybe make this a state machine: States: Announcement, Countdown, SongPart. </summary>
    internal static bool IsCurrentlyPlayingTimer { get; set; } = false;

    internal static ObservableCollection<Playlist> Playlists { get; set; } = [];

    internal static List<NewsItem> SongPartsDifference { get; set; } = [];
}
