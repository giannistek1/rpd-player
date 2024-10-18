namespace RpdPlayerApp.Models;

internal class Album
{
    public int Id { get; set; }
    public string ArtistName { get; set; }
    public string Title { get; set; }
    public DateTime ReleaseDate { get; set; }
    // TODO: Change KR, CH, TH, EN etc to Genre
    public string GenreShort { get; set; }
    public string GenreFull { get; set; }
    public string ImageURL { get; set; }

    public bool ShowAlbumTitle { get; set; } = false;
    public bool ShowAlbumReleaseDate { get; set; } = false;
    public bool ShowGenreShort { get; set; } = false;

    public Album(int id = -1, string artistName = "", DateTime releaseDate = new DateTime(), string title = "", string genreShort = "", string imageURL = "")
    {
        Id = id;
        ArtistName = artistName;
        ReleaseDate = releaseDate;
        Title = title;
        GenreShort = genreShort;
        GenreFull = GetGenre();
        ImageURL = imageURL;
    }

    private string GetGenre()
    {
        return GenreShort switch
        {
            "KR" => "K-Pop",
            "JP" => "J-Pop",
            "EN" => "Pop",
            "CH" => "C-pop",
            "TH" => "T-pop",
            "NL" => "NL-pop",

            _ => "Unknown"
        };
    }
}
