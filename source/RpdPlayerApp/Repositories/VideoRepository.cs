using RpdPlayerApp.Architecture;
using RpdPlayerApp.Models;
using System.Text.RegularExpressions;

namespace RpdPlayerApp.Repositories;

internal class VideoRepository
{
    public static List<Video> Videos = [];

    public static bool GetVideos() => InitVideos(GetStringFromUrl());
    public static bool InitVideos(string videosText)
    {
        // pattern = any number of arbitrary characters between square brackets.
        var pattern = @"\{(.*?)\}";
        var matches = Regex.Matches(videosText, pattern);

        // artist, album, title, partname, (and number), url
        // matches = 5 * 200 = 1000 
        // 0 1 2 3 4 Video 1
        // 5 6 7 8 9 Video 2

        for (int i = 0; i < matches.Count / Constants.VideoPropertyAmount; i++)
        {
            int n = Constants.VideoPropertyAmount * i; // i = Video number

            try
            {
                string artistName = matches[n + 0].Groups[1].Value;
                string albumTitle = matches[n + 1].Groups[1].Value;

                Video video = new(
                    id: i,
                    artistName: artistName,
                    albumTitle: albumTitle,
                    title: matches[n + 2].Groups[1].Value,
                    partNameShort: $"{matches[n + 3].Groups[1].Value}",
                    partNameNumber: matches[n + 4].Groups[1].Value
                );
                Videos.Add(video);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureMessage($"ERROR: {typeof(Video).Name}, video number: {i + 1}, {ex.Message}");
                General.ShowToast($"ERROR: InitVideo video {i + 1}. {ex.Message}");
            }

        }

        Videos = [.. Videos.OrderBy(s => s.ArtistName)];

        return Videos.Count > 0;
    }

    internal static bool VideoExists(string artistName, string title, string partNameShort, string partNameNumber) => Videos.Exists(a => a.ArtistName.Equals(artistName, StringComparison.OrdinalIgnoreCase) &&
                                                                                                                                                  a.Title.Equals(title, StringComparison.OrdinalIgnoreCase) &&
                                                                                                                                                  a.PartNameShort.Equals(partNameShort, StringComparison.OrdinalIgnoreCase) &&
                                                                                                                                                  a.PartNameNumber.Equals(partNameNumber, StringComparison.OrdinalIgnoreCase));

    private static string GetStringFromUrl()
    {
        if (!General.HasInternetConnection()) { return string.Empty; }

        string videosAsText = string.Empty;

        using (HttpClient client = new())
        {
            videosAsText = client.GetStringAsync(Constants.VIDEOS_SOURCE_URL).Result;
        }
        return videosAsText;
    }
}
