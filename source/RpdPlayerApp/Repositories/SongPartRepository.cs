using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core.Extensions;
using RpdPlayerApp.Architecture;
using RpdPlayerApp.Models;
using RpdPlayerApp.ViewModels;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace RpdPlayerApp.Repositories;

// Title illegal characters: ?, long dash, :, #, ', `, *,
internal static class SongPartRepository
{
    private const string SONG_PARTS_TXT_URL = "https://github.com/giannistek1/rpd-audio/blob/main/songparts.txt?raw=true";

    public static ObservableCollection<SongPart> SongParts = [];

    public static bool GetSongParts() => InitSongParts(GetStringFromURL());

    public static bool InitSongParts(string songPartsText)
    {
        // pattern = any number of arbitrary characters between square brackets.
        var pattern = @"\{(.*?)\}";
        var matches = Regex.Matches(songPartsText, pattern);

        // artist, album, title, partname, (and number), url
        // matches = 7 * 800 = 5600 
        // 0 1 2 3 4 5 6  Songpart 1
        // 7 8 9 10 11 12  Songpart 2

        var substractedSongParts = 0;

#if IOS
        substractedSongParts = 0;
#endif

        for (int i = 0; i < (matches.Count / MainViewModel.SongPartPropertyAmount) - substractedSongParts; i++)
        {
            int n = MainViewModel.SongPartPropertyAmount * i; // i = Songpart number

            try
            {
                string artistName = matches[n + 0].Groups[1].Value;
                string albumTitle = matches[n + 1].Groups[1].Value;
                string videoURL = matches[n + 6].Groups[1].Value.Replace(".mp3", ".mp4").Replace("rpd-audio", "rpd-videos");

                SongPart songPart = new(
                    id: i,
                    artistName: artistName,
                    albumTitle: albumTitle,
                    title: matches[n + 2].Groups[1].Value,
                    partNameShort: $"{matches[n + 3].Groups[1].Value}",
                    partNameNumber: matches[n + 4].Groups[1].Value,
                    clipLength: Convert.ToDouble(matches[n + 5].Groups[1].Value),
                    audioURL: matches[n + 6].Groups[1].Value,
                    videoURL: videoURL
                );
                
                songPart.Artist!.SongPartCount++;
                // For filtered list.
                songPart.Artist.FilteredTotalCount++; 

                songPart.AlbumURL = songPart.Album is not null ? songPart.Album.ImageURL : string.Empty;
                SongParts.Add(songPart);
            }
            catch(Exception ex)
            {
                SentrySdk.CaptureMessage($"ERROR: {typeof(SongPartRepository).Name}, songpart {i + 1}, {ex.Message}");
                General.ShowToast($"ERROR: InitSongPart songpart {i + 1}. {ex.Message}");
            }

        }

        SongParts = SongParts.OrderBy(s => s.ArtistName).ThenBy(s => s.Album?.ReleaseDate).ToObservableCollection();

        return SongParts.Count > 0;
    }


    private static string GetStringFromURL()
    {
        NetworkAccess accessType = Connectivity.Current.NetworkAccess;

        string songPartsAsText = string.Empty;

        if (accessType != NetworkAccess.Internet)
        {
            General.ShowToast("No internet connection!");
            return songPartsAsText;
        }

        using (HttpClient client = new())
        {
            songPartsAsText = client.GetStringAsync(SONG_PARTS_TXT_URL).Result;
        }
        return songPartsAsText;
    }

    internal static SongPart GetRandomSongPart() => SongParts[General.Rng.Next(SongParts.Count)];

    internal static List<SongPart> GetSongPartsByGeneration(string generation) => SongParts.Where(s => s.Artist?.Generation == generation && s.Album?.GenreShort == MainViewModel.GenreKpop).ToList();
}
