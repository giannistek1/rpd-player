using CommunityToolkit.Maui.Alerts;
using RpdPlayerApp.Architecture;
using RpdPlayerApp.Models;
using RpdPlayerApp.ViewModel;
using System.Text.RegularExpressions;

namespace RpdPlayerApp.Repositories;

internal class VideoRepository
{
    private const string VIDEOS_TXT_URL = "https://github.com/giannistek1/rpd-videos/blob/main/videos.txt?raw=true";

    public static List<Video> Videos = new List<Video>();

    public static bool GetVideos() => InitVideos(GetStringFromURL());
    public static bool InitVideos(string videosText)
    {
        // pattern = any number of arbitrary characters between square brackets.
        var pattern = @"\{(.*?)\}";
        var matches = Regex.Matches(videosText, pattern);

        // artist, album, title, partname, (and number), url
        // matches = 5 * 800 = 5600 
        // 0 1 2 3 4 5 6  Songpart 1
        // 7 8 9 10 11 12  Songpart 2

        for (int i = 0; i < matches.Count / MainViewModel.SongPartPropertyAmount; i++)
        {
            int n = MainViewModel.VideoPropertyAmount * i; // i = Songpart number

            try
            {
                string artistName = matches[n + 0].Groups[1].Value;
                string albumTitle = matches[n + 1].Groups[1].Value;

                Video video = new Video(
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
                Toast.Make($"ERROR: InitVideo video {i + 1}. {ex.Message}", CommunityToolkit.Maui.Core.ToastDuration.Long, 14).Show();
            }

        }

        Videos = Videos.OrderBy(s => s.ArtistName).ToList();

        return Videos.Count > 0;
    }

    internal static bool VideoExists(string artistName, string title, string partNameShort, string partNameNumber) => Videos.Exists(a => a.ArtistName.Equals(artistName, StringComparison.OrdinalIgnoreCase) &&
                                                                                                                                                  a.Title.Equals(title, StringComparison.OrdinalIgnoreCase) &&
                                                                                                                                                  a.PartNameShort.Equals(partNameShort, StringComparison.OrdinalIgnoreCase) &&
                                                                                                                                                  a.PartNameNumber.Equals(partNameNumber, StringComparison.OrdinalIgnoreCase));

    private static string GetStringFromURL()
    {
        if (!HelperClass.HasInternetConnection())
            return string.Empty;

        string videosAsText = string.Empty;

        using (HttpClient client = new HttpClient())
        {
            videosAsText = client.GetStringAsync(VIDEOS_TXT_URL).Result;
        }
        return videosAsText;
    }
}
