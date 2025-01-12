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

    internal static readonly DateTime secondGenStartDate = new(2002, 12, 31, 0, 0,0, DateTimeKind.Utc);
    internal static readonly DateTime thirdGenStartDate = new(year: 2012, month: 5, day: 25, 0,0,0, DateTimeKind.Utc);
    internal static readonly DateTime fourthGenStartDate = new(2018, 1, 1,0,0,0, DateTimeKind.Utc);
    internal static readonly DateTime fifthGenStartDate = new(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc);

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
    public static string SearchFilterModeText { get; set; } = string.Empty;

    public static PlayMode PlayMode { get; set; }
    public static bool UsingVideoMode { get; set; } = false;
    public static bool UsingCloudMode { get; set; } = false;

    // TODO: Enums
    /// <summary>
    /// 0 = off,
    /// 1 = autoplay,
    /// 2 = shuffle,
    /// 3 = repeat
    /// </summary>
    public static byte AutoplayMode { get; set; } = 0;
    /// <summary>
    /// 0 = off,
    /// 1 = 3s,
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

    public static List<string> Companies { get; set; } = [];
    public static List<string> HybeCompanies { get; set; } = ["HYBE Labels", "Big Hit Entertainment", "Source Music", "Pledis Entertainment"];
    public static List<string> SMCompanies { get; set; } = ["SM Entertainment", "Label V", "Mystic Story"];
    public static List<string> YGCompanies { get; set; } = ["YG Entertainment", "The Black Label"];
    // IST -> Went through a lot of renaming: A Cube -> Play A -> PLay M
    public static List<string> KakaoCompanies { get; set; } = ["IST Entertainment", "Starship Entertainment", "EDAM Entertainment", "Bluedot Entertainment", "High Up Entertainment", "Antenna", "FLEX M"];
    // Wake One -> was MMO Entertainment
    public static List<string> CjenmCompanies { get; set; } = ["AOMG", "B2M Entertainment", "Jellyfish Entertainment", "Wake One", "Stone Music Entertainment", "Swing Entertainment"]; 

    #region Settings
    /// <summary>
    /// 0.0 - 1.0 as Double
    /// </summary>
    public static double MainVolume = 1.0;
    public static bool ShouldLoopVideo { get; set; } = true;
    #endregion
}
