using RpdPlayerApp.Architecture;
using RpdPlayerApp.Models;
using System.Collections.ObjectModel;

namespace RpdPlayerApp.ViewModel;

internal static class MainViewModel
{
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

    public static SortMode SortMode { get; set; }
    public static SearchFilterMode SearchFilterMode { get; set; }

    public static SongPart? CurrentSongPart { get; set; } = null;

    public static Queue<SongPart> SongPartsQueue { get; set; } = new Queue<SongPart>();

    public static PlayMode PlayMode { get; set; }

    public static bool IsPlayingPlaylist { get; set; }

    public static ObservableCollection<Playlist> Playlists { get; set; } = new ObservableCollection<Playlist>();


}
