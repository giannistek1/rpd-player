namespace RpdPlayerApp.Models;

internal class Video
{
    public int Id { get; set; }

    public string Title { get; set; }
    public string ArtistName { get; set; }

    /// <summary> PartNameShort (P, C, D, DB, O, T, etc) </summary>
    public string PartNameShort { get; set; }

    public string PartNameNumber { get; set; }
    public string AlbumTitle { get; set; }

    public Video(int id, string artistName, string albumTitle, string title, string partNameShort, string partNameNumber)
    {
        Id = id;
        Title = title;
        ArtistName = artistName;

        // Somehow make part name a two parter? like if CDB 3 = Chorus 3 & Dance Break
        PartNameShort = partNameShort; // C
        PartNameNumber = partNameNumber; // 1

        AlbumTitle = albumTitle;
    }
}