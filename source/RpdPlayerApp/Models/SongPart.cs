namespace RpdPlayerApp.Models;

internal class SongPart
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
    public string AlbumTitle { get; set; }
    public Album Album { get; set; }
    public string AlbumURL { get; set; } = string.Empty;
    public string AudioURL { get; set; }
    public string VideoURL { get; set; }
    public bool ShowClipLength { get; set; } = false;
    public double ClipLength { get; set; }
    public TimeSpan ClipLengthAsTimeSpan { get; set; }

    public bool IsPlaying { get; set; } = false;
     
    public SongPart(int id, string artistName, string albumTitle, string title, string partNameShort, string partNameNumber, double clipLength, string audioURL, string videoURL)
    {
        Id = id;
        Title = title;
        ArtistName = artistName;

        // Somehow make part name a two parter? like if CDB 3 = Chorus 3 & Dance Break
        PartNameShort = partNameShort; // C
        PartNameNumber = partNameNumber; // 1
        PartNameFull = GetPartNameLong(partNameShort, partNameNumber);

        AlbumTitle = albumTitle;
        AudioURL = audioURL;
        ClipLength = clipLength;
        ClipLengthAsTimeSpan = TimeSpan.FromSeconds(clipLength);
        VideoURL = videoURL;
    }

    public SongPart(string artistName, string albumName, string title, string partNameShort, string audioURL, string videoURL)
    {
        Title = title;
        ArtistName = artistName;
        PartNameShort = partNameShort;
        AlbumTitle = albumName;
        AudioURL = audioURL;
        VideoURL = videoURL;
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
}
