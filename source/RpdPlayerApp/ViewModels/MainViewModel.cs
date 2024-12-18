using RpdPlayerApp.Architecture;
using RpdPlayerApp.Models;
using System.Collections.ObjectModel;

namespace RpdPlayerApp.ViewModels;

internal static class MainViewModel
{
    #region Constants
    // Margin/padding order: Left, Top, Right, and Bottom. LTRB (Clockwise from 9)
    /// <summary>
    /// 7
    /// </summary>
    public static readonly int SongPartPropertyAmount = 7;

    /// <summary>
    /// 7
    /// </summary>
    public static readonly int ArtistPropertyAmount = 7;

    /// <summary>
    /// 5
    /// </summary>
    public static readonly int AlbumPropertyAmount = 5;

    /// <summary>
    /// 5
    /// </summary>
    public static readonly int VideoPropertyAmount = 5;

    internal static readonly DateTime secondGenStartDate = new DateTime(2002, 12, 31);
    internal static readonly DateTime thirdGenStartDate = new DateTime(year: 2012, month: 5, day: 25);
    internal static readonly DateTime fourthGenStartDate = new DateTime(2018, 1, 1);
    internal static readonly DateTime fifthGenStartDate = new DateTime(2023, 1, 1);

    public const string NOT_KPOP = "NOT KPOP";
    public const string FIRST_GENERATION = "First generation";
    public const string SECOND_GENERATION = "Second generation";
    public const string THIRD_GENERATION = "Third generation";
    public const string FOURTH_GENERATION = "Fourth generation";
    public const string FIFTH_GENERATION = "Fifth generation";

    public static readonly string Path = FileSystem.Current.AppDataDirectory;

    #endregion

    #region Modes
    public static SortMode SortMode { get; set; } = SortMode.Artist;
    public static SearchFilterMode SearchFilterMode { get; set; }
    public static string SearchFilterModeText { get; set; }

    public static PlayMode PlayMode { get; set; }
    public static bool UsingVideoMode { get; set; } = false;
    public static bool UsingCloudMode { get; set; } = false;

    // TODO: Enums
    /// <summary>
    /// 0 = off
    /// 1 = autoplay
    /// 2 = shuffle
    /// 3 = repeat
    /// </summary>
    public static byte AutoplayMode { get; set; } = 0;
    /// <summary>
    /// 0 = off
    /// 1 = 3s
    /// 2 = 5s
    /// </summary>
    public static byte TimerMode { get; set; } = 0;

    public static bool UsingAnnouncements { get; set; } = false;

    #endregion

    // TODO: Make non-nullable and check for default values

    public static List<SongPart> SongParts { get; set; } = [];
    public static SongPart CurrentSongPart { get; set; } = new(); 
    public static Queue<SongPart> PlaylistQueue { get; set; } = new();
    public static List<SongPart> SongPartHistory { get; set; } = [];

    public static Queue<SongPart> SongPartsQueue { get; set; } = new();


    public static bool CurrentlyPlaying { get; set; } = false;

    public static ObservableCollection<Playlist> Playlists { get; set; } = [];

    #region Settings
    /// <summary>
    /// 0.0 - 1.0
    /// </summary>
    public static double MainVolume = 1.0;
    public static bool ShouldLoopVideo { get; set; } = true;
    #endregion
}
