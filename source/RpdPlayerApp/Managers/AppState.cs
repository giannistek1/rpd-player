using RpdPlayerApp.Enums;
using RpdPlayerApp.Models;
using System.Collections.ObjectModel;

namespace RpdPlayerApp.Managers;
/// <summary> The "main" state manager. </summary>
internal static class AppState
{
    // Margin/padding order: Left, Top, Right, and Bottom. LTRB (Clockwise from 9)

    #region Modes
    internal static SortModeValue SortMode { get; set; } = SortModeValue.Artist;
    internal static SearchFilterModeValue SearchFilterMode { get; set; }
    /// <summary> Queue or Playlist. </summary>
    internal static PlayModeValue PlayMode { get; set; }
    internal static bool UsingVideoMode { get; set; } = false;
    internal static bool UsingCloudMode { get; set; } = false;

    /// <summary>
    /// 0 = off,
    /// 1 = autoplay,
    /// 2 = shuffle,
    /// 3 = repeat
    /// </summary>
    internal static AutoplayModeValue AutoplayMode { get; set; } = AutoplayModeValue.AutoplayLoop;

    /// <summary>
    /// 0 = Off,
    /// 1 = 3s SHORT,
    /// 2 = 5s LONG,
    /// 3 = Mario kart
    /// </summary>
    internal static CountdownModeValue CountdownMode { get; set; } = CountdownModeValue.Off;

    /// <summary>
    /// Off = 0,
    /// DancebreakOnly = 1,
    /// Specific = 2,
    /// Artist = 3,
    /// GroupType = 4,
    /// AlwaysSongPart = 5
    /// </summary>
    internal static AnnouncementModeValue AnnouncementMode { get; set; } = 0;

    #endregion

    internal static List<SongPart> SongParts { get; set; } = [];
    /// <summary> Song to play (after countdown). </summary>
    internal static SongPart CurrentSongPart { get; set; } = new();
    internal static SongPart NextSongPart { get; set; } = new();
    internal static Queue<SongPart> PlaylistQueue { get; set; } = new();
    internal static Queue<SongPart> SongPartsQueue { get; set; } = new();
    internal static List<SongPart> SongPartHistory { get; set; } = [];
    /// <summary> To keep track of amount of times going to previous songs </summary>
    internal static int SongPartHistoryIndex { get; set; } = -1;

    internal static CurrentlyPlayingStateValue CurrentlyPlayingState { get; set; } = CurrentlyPlayingStateValue.None;

    internal static ObservableCollection<Playlist> Playlists { get; set; } = [];
}