namespace RpdPlayerApp.Models;

internal class Album
{
    public int Id { get; set; }
    public string ArtistName { get; set; }
    public string Title { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string Language { get; set; }
    public string ImageURL { get; set; }

    public bool ShowAlbumTitle { get; set; } = false;
    public bool ShowAlbumReleaseDate { get; set; } = false;
    public bool ShowLanguage { get; set; } = false;

    public Album(int id = -1, string artistName = "", DateTime releaseDate = new DateTime(), string title = "", string language = "", string imageURL = "")
    {
        Id = id;
        ArtistName = artistName;
        ReleaseDate = releaseDate;
        Title = title;
        Language = language;
        ImageURL = imageURL;
    }
}
