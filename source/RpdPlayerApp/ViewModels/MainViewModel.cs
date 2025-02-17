using RpdPlayerApp.Enums;
using RpdPlayerApp.Items;
using RpdPlayerApp.Models;
using System.Collections.ObjectModel;

namespace RpdPlayerApp.ViewModels;
/// <summary>
/// The "main" manager.
/// </summary>
internal static class MainViewModel
{
    #region Constants
    // TODO: In data class


    // Margin/padding order: Left, Top, Right, and Bottom. LTRB (Clockwise from 9)
    /// <summary>
    /// 7
    /// </summary>
    internal static readonly int SongPartPropertyAmount = 7;

    /// <summary>
    /// 7
    /// </summary>
    internal static readonly int ArtistPropertyAmount = 7;

    /// <summary>
    /// 5
    /// </summary>
    internal static readonly int AlbumPropertyAmount = 5;

    /// <summary>
    /// 5
    /// </summary>
    internal static readonly int VideoPropertyAmount = 5;

    internal static readonly DateTime secondGenStartDate = new(2002, 12, 31, 0, 0,0, DateTimeKind.Utc);
    internal static readonly DateTime thirdGenStartDate = new(year: 2012, month: 2, day: 11, 0,0,0, DateTimeKind.Utc);
    internal static readonly DateTime fourthGenStartDate = new(2018, 1, 1,0,0,0, DateTimeKind.Utc);
    internal static readonly DateTime fifthGenStartDate = new(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    internal const string GenreKpop = "KR";
    internal const string NOT_KPOP = "NOT KPOP";
    internal const string FIRST_GENERATION = "First generation";
    internal const string SECOND_GENERATION = "Second generation";
    internal const string THIRD_GENERATION = "Third generation";
    internal const string FOURTH_GENERATION = "Fourth generation";
    internal const string FIFTH_GENERATION = "Fifth generation";

    internal static readonly string Path = FileSystem.Current.AppDataDirectory;

    #endregion

    #region Modes
    internal static SortMode SortMode { get; set; } = SortMode.Artist;
    internal static SearchFilterMode SearchFilterMode { get; set; }
    internal static string SearchFilterModeText { get; set; } = string.Empty;
    /// <summary>
    /// Queue or Playlist.
    /// </summary>
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
    /// <summary>
    /// Song to play after countdown.
    /// </summary>
    internal static SongPart SongToPlay { get; set; } = new(); 
    internal static SongPart CurrentSongPart { get; set; } = new(); 
    internal static Queue<SongPart> PlaylistQueue { get; set; } = new();
    internal static List<SongPart> SongPartHistory { get; set; } = [];

    internal static Queue<SongPart> SongPartsQueue { get; set; } = new();

    internal static bool IsCurrentlyPlayingSongPart { get; set; } = false;
    internal static bool IsCurrentlyPlayingTimer { get; set; } = false;

    internal static ObservableCollection<Playlist> Playlists { get; set; } = [];

    internal static List<string> AllCompanies { get; set; } = [];
    internal static List<string> HybeCompanies { get; set; } = ["HYBE Labels", "Big Hit Entertainment", "Source Music", "Pledis Entertainment"];
    internal static List<string> SMCompanies { get; set; } = ["SM Entertainment", "Label V", "Mystic Story"];
    internal static List<string> YGCompanies { get; set; } = ["YG Entertainment", "The Black Label"];
    // IST -> Went through a lot of renaming: A Cube -> Play A -> PLay M
    internal static List<string> KakaoCompanies { get; set; } = ["IST Entertainment", "Starship Entertainment", "EDAM Entertainment", "Bluedot Entertainment", "High Up Entertainment", "Antenna", "FLEX M"];
    // Wake One -> was MMO Entertainment
    internal static List<string> CjenmCompanies { get; set; } = ["AOMG", "B2M Entertainment", "Jellyfish Entertainment", "Wake One", "Stone Music Entertainment", "Swing Entertainment"];
    internal static List<string> RbwCompanies { get; set; } = ["RBW Entertainment", "WM Entertainment", "DSP Media"];

    // YMC Entertainment is owned by Dream T Entertainment

    internal static List<NewsItem> SongPartsDifference { get; set; } = [];
}
