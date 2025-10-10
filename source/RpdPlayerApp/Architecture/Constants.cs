namespace RpdPlayerApp.Architecture;

internal static class Constants
{
    internal const string SONGPARTS_SOURCE_URL = "https://github.com/giannistek1/rpd-audio/blob/main/songparts.txt?raw=true";
    internal const string VIDEOS_SOURCE_URL = "https://github.com/giannistek1/rpd-videos/blob/main/videos.txt?raw=true";
    internal const string ARTISTS_SOURCE_URL = "https://github.com/giannistek1/rpd-artists/blob/main/artists.txt?raw=true";
    internal const string ALBUMS_SOURCE_URL = "https://github.com/giannistek1/rpd-albums/blob/main/albums.txt?raw=true";

    internal const string BASE_URL = "https://faodcajoizngcccweqsa.supabase.co";
    internal const string APIKEY = "";
    internal const string SONGREQUEST_ROUTE = "/rest/v1/song_request";
    internal const string FEEDBACK_ROUTE = "/rest/v1/feedback";
    internal const string PLAYLIST_URL = "/rest/v1/playlist";

    /// <summary> 7 </summary>
    internal static readonly int SongPartPropertyAmount = 7;

    /// <summary> 7 </summary>
    internal static readonly int ArtistPropertyAmount = 7;

    /// <summary> 5 </summary>
    internal static readonly int AlbumPropertyAmount = 5;

    /// <summary> 5 </summary>
    internal static readonly int VideoPropertyAmount = 5;

    internal static int HISTORY_LIMIT = 100;

    internal static readonly DateTime secondGenStartDate = new(2002, 12, 31, 0, 0, 0, DateTimeKind.Utc);
    internal static readonly DateTime thirdGenStartDate = new(year: 2012, month: 2, day: 11, 0, 0, 0, DateTimeKind.Utc);
    internal static readonly DateTime fourthGenStartDate = new(2018, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    internal static readonly DateTime fifthGenStartDate = new(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    internal const string GenreKpop = "KR";
    internal const string NOT_KPOP = "NOT KPOP";
    internal const string FIRST_GENERATION = "First generation";
    internal const string SECOND_GENERATION = "Second generation";
    internal const string THIRD_GENERATION = "Third generation";
    internal const string FOURTH_GENERATION = "Fourth generation";
    internal const string FIFTH_GENERATION = "Fifth generation";

    // Not a constant at startup.
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

}
