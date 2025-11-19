using CommunityToolkit.Mvvm.ComponentModel;
using RpdPlayerApp.Architecture;
using RpdPlayerApp.Enums;
using RpdPlayerApp.Repositories;
using System.Text.Json.Serialization;

namespace RpdPlayerApp.Models;
/// <remarks> JSON usage is because of newsitems. Maybe can be reworked? </remarks>
internal partial class SongPart : ObservableObject
{
    /// <summary> Order index for playlist. For a unique identifier, use the audioURL. </summary>
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    [ObservableProperty]
    private int _id;

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private string _artistName;

    [property: JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public Artist Artist { get; set; }

    /// <summary> e.g. P, C, D, DB, O, T, etc </summary>
    [ObservableProperty]
    private string _partNameShort;

    /// <summary> e.g. P1, C2, etc </summary>
    [ObservableProperty]
    private string _partNameShortWithNumber;

    /// <summary> e.g. 1, 2, 3, etc </summary>
    [ObservableProperty]
    private string _partNameNumber;

    /// <summary> e.g. Pre-chorus 1, Chorus 2, etc </summary>
    [ObservableProperty]
    private string _partNameFull;

    [ObservableProperty]
    private SongSegmentOrderValue _partClassification;

    [ObservableProperty]
    private string _albumTitle;

    [property: JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public Album Album { get; set; }

    [ObservableProperty]
    private string _albumURL = string.Empty;

    /// <summary> Unique. </summary>
    [ObservableProperty]
    private string _audioURL;

    [ObservableProperty]
    private string _videoURL;

    /// <summary> Based on match with VideoRepo list. </summary>
    [ObservableProperty]
    private bool _hasVideo;

    /// <summary> For news </summary>
    [ObservableProperty]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    private bool _newVideoAvailable = false;

    [ObservableProperty]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    private bool _showClipLength = false;

    /// <summary> Cliplength in seconds. </summary>
    [ObservableProperty]
    private double _clipLength;

    [ObservableProperty]
    private TimeSpan _clipLengthAsTimeSpan;

    [ObservableProperty]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    private TimeSpan _playlistStartTime = TimeSpan.MinValue;

    /// <summary> Whether the song is currently playing or paused/stopped. </summary>
    [ObservableProperty]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    private bool isPlaying = false;

    [ObservableProperty]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    private double playingIconScaleY1 = 1.0;

    [ObservableProperty]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    private double _playingIconTranslationY1 = 1.0;

    [ObservableProperty]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    private double playingIconScaleY2 = 0.2;

    [ObservableProperty]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    private double _playingIconTranslationY2 = 1.0;

    [ObservableProperty]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    private double _playingIconScaleY3 = 0.6;

    [ObservableProperty]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    private double _playingIconTranslationY3 = 1.0;

    [JsonConstructor]
    public SongPart() { }

    public SongPart(int id = -1, string artistName = "", string albumTitle = "", string title = "", string partNameShort = "", string partNameNumber = "", double clipLength = 0.0, string audioURL = "", string videoURL = "", TimeSpan? playlistStartTime = null)
    {
        Id = id;
        Title = title;
        ArtistName = artistName;

        PartNameShort = partNameShort; // C
        PartNameNumber = partNameNumber; // 1
        PartNameShortWithNumber = $"{partNameShort}{partNameNumber}";
        PartNameFull = GetPartNameLong(partNameShort, partNameNumber);
        PartClassification = GetSongPartOrder(partNameShort);

        AlbumTitle = albumTitle;
        AudioURL = audioURL;
        ClipLength = clipLength;
        ClipLengthAsTimeSpan = TimeSpan.FromSeconds(clipLength);
        VideoURL = videoURL;

        HasVideo = VideoRepository.VideoExists(artistName: artistName, title: title, partNameShort: partNameShort, partNameNumber: partNameNumber);

        Album = AlbumRepository.MatchAlbum(artistName: artistName, albumTitle: albumTitle);
        Artist = ArtistRepository.MatchArtist(artistName: artistName);

        if (Album.GenreShort == Constants.GenreKpop)
        {
            Artist.IsKpopArtist = true;
            Artist.DecideGeneration();
        }

        if (playlistStartTime is not null)
        {
            PlaylistStartTime = playlistStartTime.Value;
        }
    }

    private static string GetPartNameLong(string partNameShort, string partNumber)
    {
        return partNameShort switch
        {
            "P" => $"Pre-chorus {partNumber}",
            "PDB" => $"Pre-chorus {partNumber} & Dance Break",

            "C" => $"Chorus {partNumber}",
            "CDB" => $"Chorus {partNumber} & Dance Break",
            "CDBE" => $"Chorus {partNumber}, Dance Break & Ending",
            "CE" => $"Chorus {partNumber} & Ending",

            "B" => $"Bridge {partNumber}",
            "DB" => $"Dance break {partNumber}",
            "DBC" => $"Dance Break {partNumber} & Chorus",
            "DBE" => $"Dance Break {partNumber} & Ending",

            // No songpart yet.
            "DBO" => $"Dance Break {partNumber} & Outro",

            "E" => $"Ending {partNumber}",
            "I" => $"Intro {partNumber}",
            "O" => $"Outro {partNumber}",
            "T" => $"Tiktok {partNumber}",
            "V" => $"Verse {partNumber}",

            _ => "Unknown song part"
        };
    }

    private static SongSegmentOrderValue GetSongPartOrder(string partNameShort)
    {
        return partNameShort switch
        {
            "P" or "PDB" => SongSegmentOrderValue.Prechorus,
            "C" or "CDB" or "CDBE" or "CE" => SongSegmentOrderValue.Chorus,
            "B" or "DB" or "O" or "DBO" or "DBC" or "DBE" => SongSegmentOrderValue.Dancebreak,
            "T" => SongSegmentOrderValue.Tiktok,
            "E" or "O" => SongSegmentOrderValue.Ending,
            "I" => SongSegmentOrderValue.Intro,
            "V" => SongSegmentOrderValue.Verse,

            "U" or _ => SongSegmentOrderValue.Unspecified
        };
    }
}