using CommunityToolkit.Maui.Alerts;
using RpdPlayerApp.Architecture;
using RpdPlayerApp.Models;
using RpdPlayerApp.ViewModel;
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

        // artist, albumtitle, releasedate, genreShort, url
        // matches = 5 * 800 = 4000 
        // 0 1 2 3 4  Album 1
        // 5 6 7 8 9  Album 2

        string artist;
        DateTime date;
        string title;
        string genreShort;
        string imageUrl;

        for (int i = 0; i < matches.Count / MainViewModel.AlbumPropertyAmount; i++)
        {
            int n = MainViewModel.AlbumPropertyAmount * i; // n = property index, i = number

            try
            {
                artist = matches[n + 0].Groups[1].Value;
                date = DateTime.ParseExact(matches[n + 1].Groups[1].Value, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                title = matches[n + 2].Groups[1].Value;
                genreShort = matches[n + 3].Groups[1].Value;
                imageUrl = matches[n + 4].Groups[1].Value;

                Albums.Add(new Album(id: i, artistName: artist, releaseDate: date, title: title, genreShort: genreShort, imageURL: imageUrl));
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureMessage($"ERROR album: {typeof(SongPartRepository).Name}, album {i+1}, {ex.Message}"); // i was the last number that worked
                Toast.Make($"ERROR: InitAlbums, album {i+1}. {ex.Message}", CommunityToolkit.Maui.Core.ToastDuration.Long, 14).Show();
            }
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
