using Newtonsoft.Json;
using RpdPlayerApp.Architecture;
using RpdPlayerApp.Enums;
using RpdPlayerApp.Repositories;
using System.ComponentModel;

namespace RpdPlayerApp.Models;

internal partial class SongPart : INotifyPropertyChanged
{
    /// <summary> This is an index based on a playlist. For a unique identifier, use the audioURL. </summary>
    public int Id { get; set; }

    public string Title { get; set; }
    public string ArtistName { get; set; }
    public Artist Artist { get; set; }

    /// <summary> PartNameShort (P, C, D, DB, O, T, etc) </summary>
    public string PartNameShort { get; set; }

    public string PartNameShortWithNumber { get; set; }
    public string PartNameNumber { get; set; }
    public string PartNameFull { get; set; }
    public SongPartOrder PartClassification { get; set; }
    public string AlbumTitle { get; set; }
    public Album Album { get; set; }
    public string AlbumURL { get; set; } = string.Empty;

    /// <summary> Unique. </summary>
    public string AudioURL { get; set; }

    public string VideoURL { get; set; }
    public bool HasVideo { get; set; }

    [JsonIgnore]
    public bool ShowClipLength { get; set; } = false;

    public double ClipLength { get; set; }
    public TimeSpan ClipLengthAsTimeSpan { get; set; }

    private bool isPlaying = false;

    /// <summary> Whether the song is currently playing or paused/stopped. </summary>
    [JsonIgnore]
    public bool IsPlaying
    {
        get
        {
            return isPlaying;
        }

        set
        {
            isPlaying = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPlaying)));
        }
    }

    private double playingIconScaleY1 = 1.0;

    [JsonIgnore]
    public double PlayingIconScaleY1
    {
        get
        {
            return playingIconScaleY1;
        }

        set
        {
            playingIconScaleY1 = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PlayingIconScaleY1)));
        }
    }

    private double playingIconTranslationY1 = 1.0;

    [JsonIgnore]
    public double PlayingIconTranslationY1
    {
        get
        {
            return playingIconTranslationY1;
        }

        set
        {
            playingIconTranslationY1 = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PlayingIconTranslationY1)));
        }
    }

    private double playingIconScaleY2 = 0.2;

    [JsonIgnore]
    public double PlayingIconScaleY2
    {
        get
        {
            return playingIconScaleY2;
        }

        set
        {
            playingIconScaleY2 = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PlayingIconScaleY2)));
        }
    }

    private double playingIconTranslationY2 = 1.0;

    [JsonIgnore]
    public double PlayingIconTranslationY2
    {
        get
        {
            return playingIconTranslationY2;
        }

        set
        {
            playingIconTranslationY2 = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PlayingIconTranslationY2)));
        }
    }

    private double playingIconScaleY3 = 0.6;

    [JsonIgnore]
    public double PlayingIconScaleY3
    {
        get
        {
            return playingIconScaleY3;
        }

        set
        {
            playingIconScaleY3 = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PlayingIconScaleY3)));
        }
    }

    private double playingIconTranslationY3 = 1.0;

    [JsonIgnore]
    public double PlayingIconTranslationY3
    {
        get
        {
            return playingIconTranslationY3;
        }

        set
        {
            playingIconTranslationY3 = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PlayingIconTranslationY3)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public SongPart(int id = -1, string artistName = "", string albumTitle = "", string title = "", string partNameShort = "", string partNameNumber = "", double clipLength = 0.0, string audioURL = "", string videoURL = "")
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

    private static SongPartOrder GetSongPartOrder(string partNameShort)
    {
        return partNameShort switch
        {
            "P" or "PDB" => SongPartOrder.Prechorus,
            "C" or "CDB" or "CDBE" or "CE" => SongPartOrder.Chorus,
            "B" or "DB" or "O" or "DBO" or "DBC" or "DBE" => SongPartOrder.Dancebreak,
            "T" => SongPartOrder.Tiktok,
            "E" or "O" => SongPartOrder.Ending,
            "I" => SongPartOrder.Intro,
            "V" => SongPartOrder.Verse,

            "U" or _ => SongPartOrder.Unspecified
        };
    }
}