using CommunityToolkit.Maui.Alerts;
using RpdPlayerApp.Architecture;
using RpdPlayerApp.Models;
using RpdPlayerApp.ViewModels;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;

namespace RpdPlayerApp.Repositories;

internal static class ArtistRepository
{
    private const string ARTISTS_TXT_URL = "https://github.com/giannistek1/rpd-artists/blob/main/artists.txt?raw=true";

    public readonly static ObservableCollection<Artist> Artists = [];

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

        for (int i = 0; i < matches.Count / MainViewModel.ArtistPropertyAmount; i++)
        {
            int n = MainViewModel.ArtistPropertyAmount * i; // i = Artist number

            try
            {
                Enum.TryParse(matches[n + 3].Groups[1].Value, out GroupType groupType);

                Artists.Add(new Artist(id: i, name: matches[n + 0].Groups[1].Value,
                    altName: matches[n + 1].Groups[1].Value,
                    debutDate: DateTime.ParseExact(matches[n + 2].Groups[1].Value, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    groupType: groupType,
                    memberCount: Convert.ToInt16(matches[n + 4].Groups[1].Value),
                    company: matches[n + 5].Groups[1].Value,
                    imageURL: matches[n + 6].Groups[1].Value));
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureMessage($"Error: {typeof(ArtistRepository).Name}, artist {i + 1}, {ex.Message}");
                HelperClass.ShowToast($"ERRROR: InitArtist, artist {i + 1}, {ex.Message}");
            }

        }

        return Artists.Count > 0;
    }
    private static string GetStringFromURL()
    {
        if (!HelperClass.HasInternetConnection()) { return string.Empty; }

        string artistsAsText = string.Empty;

        using (HttpClient client = new())
        {
            artistsAsText = client.GetStringAsync(ARTISTS_TXT_URL).Result;
        }
        return artistsAsText;
    }


    internal static Artist MatchArtist(string artistName) => Artists.FirstOrDefault(a => a.Name.Equals(artistName, StringComparison.OrdinalIgnoreCase))!;
    internal static List<Artist> GetTopArtistsForGen(Gen generation, int count = 5) 
        => Artists.Where(a => a.IsKpopArtist && a.Gen == generation)
                  .OrderByDescending(a => a.SongPartCount)
                  .Take(count)
                  .ToList();

    internal static List<Artist> GetTopArtistsForCompanies(List<string> companies, int count = 5)
        => Artists.Where(a => companies.Contains(a.Company))
                  .OrderByDescending(a => a.SongPartCount)
                  .Take(count)
                  .ToList();

    internal static Artist GetRandomArtist() => Artists[HelperClass.Rng.Next(Artists.Count)];
    internal static Artist GetRandomArtist(List<Artist> artistList) => artistList[HelperClass.Rng.Next(artistList.Count)];

}
