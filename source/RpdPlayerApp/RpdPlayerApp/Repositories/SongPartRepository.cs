using RpdPlayerApp.Models;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace RpdPlayerApp.Repository;

internal static class SongPartRepository
{
    private const string SONG_PARTS_URL = "https://github.com/giannistek1/rpd-audio/blob/main/songparts.txt?raw=true";

    public readonly static ObservableCollection<SongPart> SongParts = new ObservableCollection<SongPart>();
    //public readonly static ObservableCollection<string> UniqueSongs = new ObservableCollection<string>();

    public static bool InitSongParts(string songPartsText)
    {
        // pattern = any number of arbitrary characters between square brackets.
        var pattern = @"\{(.*?)\}";
        var matches = Regex.Matches(songPartsText, pattern);

        // artist, album, title, partname, (and number), url
        // matches = 6 * 800 = 4800 
        // 0 1 2 3 4 5  Songpart 1
        // 6 7 8 9 10 11 Songpart 2
        // 12 13 14 15 16 17 Songpart 2

        for (int i = 0; i < matches.Count / 6; i++)
        {
            int n = 6 * i; // songpart number
            SongParts.Add(new SongPart(id: i, artistName: matches[n + 0].Groups[1].Value, albumName: matches[n + 1].Groups[1].Value, title: matches[n + 2].Groups[1].Value, partNameShort: $"{matches[n + 3].Groups[1].Value}", partNameNumber: matches[n + 4].Groups[1].Value, audioURL: matches[n + 5].Groups[1].Value));
        }

        return SongParts.Count > 0;
    }

    private static string GetStringFromURL()
    {
        NetworkAccess accessType = Connectivity.Current.NetworkAccess;

        string songPartsAsText = string.Empty;

        if (accessType != NetworkAccess.Internet)
        {
            CommunityToolkit.Maui.Alerts.Toast.Make($"No internet connection!", CommunityToolkit.Maui.Core.ToastDuration.Short, 30).Show();
            return songPartsAsText;
        }

        using (HttpClient client = new HttpClient())
        {
            songPartsAsText = client.GetStringAsync(SONG_PARTS_URL).Result;
        }
        return songPartsAsText;
    }

    public static bool GetSongParts()
    {
        return InitSongParts(GetStringFromURL());
    }
}
