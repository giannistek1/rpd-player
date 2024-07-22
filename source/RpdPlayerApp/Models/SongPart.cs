﻿using Android.Health.Connect.DataTypes.Units;

namespace RpdPlayerApp.Models;

internal class SongPart
{
    public int Id { get; set; }

    public string Title { get; set; }
    public string ArtistName { get; set; }
    public Artist? Artist { get; set; }

    /// <summary>
    /// PartNameShort (P, C, D, DB, O, T, etc)
    /// </summary>
    public string PartNameShort { get; set; }
    public string PartNameNumber { get; set; }
    public string PartNameFull { get; set; }
    public string AlbumTitle { get; set; }
    public Album? Album { get; set; }
    public string AlbumURL { get; set; } = string.Empty;
    public string AudioURL { get; set; }
    public bool ShowClipLength { get; set; } = false;
    public double ClipLength { get; set; }
    public TimeSpan ClipLengthAsTimeSpan { get; set; }

    public bool IsPlaying { get; set; } = false;

    public SongPart(int id, string artistName, string albumTitle, string title, string partNameShort, string partNameNumber, double clipLength, string audioURL)
    {
        Id = id;
        Title = title;
        ArtistName = artistName;

        // Somehow make part name a two parter? like if CDB 3 = Chorus 3 & Dance Break
        PartNameShort = partNameShort; // C
        PartNameNumber = partNameNumber; // 1
        PartNameFull = $"{GetPartNameLong(partNameShort)} {PartNameNumber}"; // Chorus 1

        AlbumTitle = albumTitle;
        AudioURL = audioURL;
        ClipLength = clipLength;
        ClipLengthAsTimeSpan = TimeSpan.FromSeconds(clipLength);
    }

    public SongPart(string artistName, string albumName, string title, string partNameShort, string audioURL)
    {
        Title = title;
        ArtistName = artistName;
        PartNameShort = partNameShort;
        AlbumTitle = albumName;
        AudioURL = audioURL;
    }

    private string GetPartNameLong(string partNameShort)
    {
        return partNameShort switch
        {
            "B" => "Bridge",
            "C" => "Chorus",
            "CDB" => "Chorus & Dance Break",
            "CE" => "Chorus & Ending",
            "DB" => "Dance break",
            "DBC" => "Dance Break & Chorus",
            "DBE" => "Dance Break & Ending",
            "E" => "Ending",
            "I" => "Intro",
            "O" => "Outro",
            "P" => "Pre-chorus",
            "T" => "Tiktok",
            "V" => "Verse",

            _ => "Unkown song part"
        };
    }
}
