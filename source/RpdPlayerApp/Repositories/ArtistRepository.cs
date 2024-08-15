using RpdPlayerApp.Architecture;
using RpdPlayerApp.Models;
using RpdPlayerApp.ViewModel;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;

namespace RpdPlayerApp.Repository;

internal static class ArtistRepository
{
    private const string ARTISTS_TXT_URL = "https://github.com/giannistek1/rpd-artists/blob/main/artists.txt?raw=true";

    public readonly static ObservableCollection<Artist> Artists = new ObservableCollection<Artist>();

    public static bool GetArtists() => InitArtists(GetStringFromURL());

    public static bool InitArtists(string artistsText)
    {
        // pattern = any number of arbitrary characters between square brackets.
        var pattern = @"\{(.*?)\}";
        var matches = Regex.Matches(artistsText, pattern);

        // artist, alt names, debut date, grouptype, members, company, imageurl
        // matches = 7 * 165 = 4000 
        // 0 1 2 3  4  5  6  Artist 1
        // 7 8 9 10 11 12 13  Artist 2


        for (int i = 0; i < matches.Count / MainViewModel.SongPartPropertyAmount; i++)
        {
            int n = MainViewModel.ArtistPropertyAmount * i; // artist number

            try
            {
                Enum.TryParse(matches[n + 3].Groups[1].Value, out GroupType groupType);

                Artists.Add(new Artist(id: i, name: matches[n + 0].Groups[1].Value,
                    altName: matches[n + 1].Groups[1].Value,
                    debutDate: DateTime.ParseExact(matches[n + 2].Groups[1].Value, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    groupType,
                    memberCount: Convert.ToInt16(matches[n + 4].Groups[1].Value),
                    company: matches[n + 5].Groups[1].Value,
                    imageURL: matches[n + 6].Groups[1].Value));
            }
            catch(Exception ex)
            {
                SentrySdk.CaptureMessage($"Error: {typeof(ArtistRepository).Name}: Artist number: {n}");
            }

        }

        return Artists.Count > 0;
    }

    internal static Artist? MatchArtist(string artistName)
    {
        return Artists.FirstOrDefault(a => a.Name.ToLower().Equals(artistName.ToLower()));
    }

    private static string GetStringFromURL()
    {
        if (!HelperClass.HasInternetConnection())
            return string.Empty;

        string artistsAsText = string.Empty;

        using (HttpClient client = new HttpClient())
        {
            artistsAsText = client.GetStringAsync(ARTISTS_TXT_URL).Result;
        }
        return artistsAsText;
    }
}
