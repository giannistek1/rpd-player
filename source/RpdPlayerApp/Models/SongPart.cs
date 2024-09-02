using RpdPlayerApp.Architecture;
using RpdPlayerApp.Repositories;
using System.ComponentModel;

namespace RpdPlayerApp.Models;

internal class SongPart : INotifyPropertyChanged
{
    public int Id { get; set; }

    public string Title { get; set; }
    public string ArtistName { get; set; }
    public Artist Artist { get; set; }

    /// <summary>
    /// PartNameShort (P, C, D, DB, O, T, etc)
    /// </summary>
    public string PartNameShort { get; set; }
    public string PartNameNumber { get; set; } = String.Empty;
    public string PartNameFull { get; set; } = String.Empty;
    public SongPartOrder PartClassification { get; set; } = SongPartOrder.Unspecified;
    public string AlbumTitle { get; set; }
    public Album Album { get; set; }
    public string AlbumURL { get; set; } = string.Empty;
    public string AudioURL { get; set; }
    public string VideoURL { get; set; }
    public bool HasVideo { get; set; }
    public bool ShowClipLength { get; set; } = false;
    public double ClipLength { get; set; }
    public TimeSpan ClipLengthAsTimeSpan { get; set; }

    private bool isPlaying = false;
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
    public event PropertyChangedEventHandler PropertyChanged;

    public SongPart(int id, string artistName, string albumTitle, string title, string partNameShort, string partNameNumber, double clipLength, string audioURL, string videoURL)
    {
        Id = id;
        Title = title;
        ArtistName = artistName;

        PartNameShort = partNameShort; // C
        PartNameNumber = partNameNumber; // 1
        PartNameFull = GetPartNameLong(partNameShort, partNameNumber);
        PartClassification = GetSongPartOrder(partNameShort);

        AlbumTitle = albumTitle;
        AudioURL = audioURL;
        ClipLength = clipLength;
        ClipLengthAsTimeSpan = TimeSpan.FromSeconds(clipLength);
        VideoURL = videoURL;
        HasVideo = VideoRepository.VideoExists(artistName: artistName, title: title, partNameShort: partNameShort, partNameNumber: partNameNumber);
    }

    private string GetPartNameLong(string partNameShort, string partNumber)
    {
        return partNameShort switch
        {
            "B" => $"Bridge {partNumber}",
            "C" => $"Chorus {partNumber}",
            "CDB" => $"Chorus {partNumber} & Dance Break",
            "CE" => $"Chorus {partNumber} & Ending",
            "DB" => $"Dance break {partNumber}",

            // Consider renaming your DBC songpart to DBE cuz dance break (3) and chorus (3) is more confusing than dance break 1 and ending
            //"DBC" => $"Dance Break {partNumber} & Chorus", 
            "DBE" => $"Dance Break {partNumber} & Ending",

            // No songpart yet
            "DBO" => $"Dance Break {partNumber} & Outro", 

            "E" => $"Ending {partNumber}",
            "I" => $"Intro {partNumber}",
            "O" => $"Outro {partNumber}",
            "P" => $"Pre-chorus {partNumber}",
            "PDB" => $"Pre-chorus {partNumber} & Dance Break",
            "T" => $"Tiktok {partNumber}",
            "V" => $"Verse {partNumber}",

            _ => "Unkown song part"
        };
    }

    private SongPartOrder GetSongPartOrder(string partNameShort)
    {
        return partNameShort switch
        {
            "P" or "PDB" => SongPartOrder.Prechorus,
            "C" or "CDB" or "CE" => SongPartOrder.Chorus,
            "B" or "DB" or "O" or "DBO" or "DBE" => SongPartOrder.Dancebreak,
            "T" => SongPartOrder.Tiktok,
            "E" or "O" => SongPartOrder.Ending,
            "I" => SongPartOrder.Intro,
            "V" => SongPartOrder.Verse,

            "U" or _ => SongPartOrder.Unspecified
        };
    }
}
