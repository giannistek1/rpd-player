using RpdPlayerApp.Architecture;
using RpdPlayerApp.Models;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;

namespace RpdPlayerApp.Repository;

internal static class AlbumRepository
{
    private const string ALBUMS_TXT_URL = "https://github.com/giannistek1/rpd-albums/blob/main/albums.txt?raw=true";

    public readonly static ObservableCollection<Album> Albums = new ObservableCollection<Album>();

    public static bool GetAlbums() => InitAlbums(GetStringFromURL());
    public static bool InitAlbums(string albumsText)
    {
        // pattern = any number of arbitrary characters between square brackets.
        var pattern = @"\{(.*?)\}";
        var matches = Regex.Matches(albumsText, pattern);

        // artist, albumtitle, releasedate, language, url
        // matches = 5 * 800 = 4000 
        // 0 1 2 3 4  Album 1
        // 5 6 7 8 9  Album 2


        for (int i = 0; i < matches.Count / 5; i++)
        {
            int n = 5 * i; // album number
            Albums.Add(new Album(id: i, artistName: matches[n + 0].Groups[1].Value, releaseDate: DateTime.ParseExact(matches[n + 1].Groups[1].Value, "yyyy-MM-dd", CultureInfo.InvariantCulture), title: matches[n + 2].Groups[1].Value, language: matches[n + 3].Groups[1].Value, imageURL: matches[n + 4].Groups[1].Value));
        }

        return Albums.Count > 0;
    }

    internal static Album MatchAlbum(string artistName, string albumTitle)
    {
        return Albums.FirstOrDefault(a => a.ArtistName.Equals(artistName, StringComparison.OrdinalIgnoreCase) && a.Title.Equals(albumTitle, StringComparison.OrdinalIgnoreCase), new());
    }

    private static string GetStringFromURL()
    {
        if (!HelperClass.HasInternetConnection())
            return string.Empty;

        string albumsAsText = string.Empty;

        using (HttpClient client = new HttpClient())
        {
            albumsAsText = client.GetStringAsync(ALBUMS_TXT_URL).Result;
        }
        return albumsAsText;
    }
}
