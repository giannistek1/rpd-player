using RpdPlayerApp.Architecture;
using RpdPlayerApp.Models;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;

namespace RpdPlayerApp.Repositories;

internal static class ArtistRepository
{
    public readonly static ObservableCollection<Artist> Artists = [];

    public static bool GetArtists() => InitArtists(GetStringFromUrl());

    public static bool InitArtists(string artistsText)
    {
        // pattern = any number of arbitrary characters between square brackets.
        var pattern = @"\{(.*?)\}";
        var matches = Regex.Matches(artistsText, pattern);

        // artist, alt names, debut date, grouptype, members, company, imageurl
        // matches = 7 * 165 = 4000 
        // 0 1 2 3  4  5  6  Artist 1
        // 7 8 9 10 11 12 13  Artist 2

        for (int i = 0; i < matches.Count / Constants.ArtistPropertyAmount; i++)
        {
            int n = Constants.ArtistPropertyAmount * i; // i = Artist number

            try
            {
                Enum.TryParse(matches[n + 3].Groups[1].Value, out GroupType groupType);

                Artists.Add(new Artist(id: i, name: matches[n + 0].Groups[1].Value,
                    altNames: matches[n + 1].Groups[1].Value,
                    debutDate: DateTime.ParseExact(matches[n + 2].Groups[1].Value, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    groupType: groupType,
                    memberCount: Convert.ToInt16(matches[n + 4].Groups[1].Value),
                    company: matches[n + 5].Groups[1].Value,
                    imageUrl: matches[n + 6].Groups[1].Value));
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureMessage($"Error: {typeof(ArtistRepository).Name}, artist {i + 1}, {ex.Message}");
                General.ShowToast($"ERROR: InitArtist, artist {i + 1}, {ex.Message}");
            }

        }

        return Artists.Count > 0;
    }
    private static string GetStringFromUrl()
    {
        if (!General.HasInternetConnection()) { return string.Empty; }

        string artistsAsText = string.Empty;

        using (HttpClient client = new())
        {
            artistsAsText = client.GetStringAsync(Constants.ARTISTS_SOURCE_URL).Result;
        }
        return artistsAsText;
    }


    internal static Artist MatchArtist(string artistName) => Artists.FirstOrDefault(a => a.Name.Equals(artistName, StringComparison.OrdinalIgnoreCase), new());
    internal static List<Artist> GetTopArtistsForGen(GenType generation, int count = 5)
        => Artists.Where(a => a.IsKpopArtist && a.Gen == generation)
                  .OrderByDescending(a => a.SongPartCount)
                  .Take(count)
                  .ToList();

    internal static List<Artist> GetTopArtistsForCompanies(List<string> companies, int count = 5)
        => Artists.Where(a => companies.Contains(a.Company))
                  .OrderByDescending(a => a.SongPartCount)
                  .Take(count)
                  .ToList();

    internal static Artist GetRandomArtist()
    {
        if (Artists.Count == 0)
        {
            return new Artist();
        }
        return Artists[General.Rng.Next(Artists.Count)];
    }
    internal static Artist GetRandomArtist(List<Artist> artistList)
    {
        if (artistList.Count == 0)
        {
            return new Artist();
        }
        return artistList[General.Rng.Next(artistList.Count)];
    }

}
