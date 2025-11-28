using CommunityToolkit.Maui.Core.Extensions;
using RpdPlayerApp.Architecture;
using RpdPlayerApp.Models;
using RpdPlayerApp.Services;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace RpdPlayerApp.Repositories;

// Title illegal characters: ?, long dash, :, #, ', `, *,
internal static class SongPartRepository
{
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

        for (int i = 0; i < (matches.Count / Constants.SongPartPropertyAmount) - substractedSongParts; i++)
        {
            int n = Constants.SongPartPropertyAmount * i; // i = Songpart number

            try
            {
                string artistName = matches[n + 0].Groups[1].Value;
                string albumTitle = matches[n + 1].Groups[1].Value;
                string videoUrl = matches[n + 6].Groups[1].Value.Replace(".mp3", ".mp4").Replace("rpd-audio", "rpd-videos");

                SongPart songPart = new(
                    artistName: artistName,
                    albumTitle: albumTitle,
                    title: matches[n + 2].Groups[1].Value,
                    partNameShort: $"{matches[n + 3].Groups[1].Value}",
                    partNameNumber: matches[n + 4].Groups[1].Value,
                    clipLength: Convert.ToDouble(matches[n + 5].Groups[1].Value),
                    audioUrl: matches[n + 6].Groups[1].Value,
                    videoUrl: videoUrl
                );

                songPart.Artist!.SongPartCount++;
                // For filtered list.
                songPart.Artist.FilteredTotalCount++;

                songPart.AlbumUrl = songPart.Album is not null ? songPart.Album.ImageUrl : string.Empty;
                SongParts.Add(songPart);
            }
            catch (Exception ex)
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
            songPartsAsText = client.GetStringAsync(Constants.SONGPARTS_SOURCE_URL).Result;
        }
        return songPartsAsText;
    }

    internal static SongPart GetRandomSongPart() => SongParts[General.Rng.Next(SongParts.Count)];

    internal static List<SongPart> GetSongPartsByGeneration(string generation) => SongParts.Where(s => s.Artist?.Generation == generation && s.Album?.GenreShort == Constants.GenreKpop).ToList();


    public static async Task<bool> LoadSongPartsFromFileAsync(string fileName)
    {
        try
        {
            DebugService.Instance.Info("Offline mode: Loading song segments.");

            var filePath = Path.Combine(FileSystem.Current.AppDataDirectory, fileName);

            if (!File.Exists(filePath))
            {
                DebugService.Instance.Warn($"File not found: {filePath}");
                return false;
            }

            var songPartsText = await File.ReadAllTextAsync(filePath);

            // Reuse your existing parsing logic
            return InitSongParts(songPartsText);
        }
        catch (Exception ex)
        {
            DebugService.Instance.Error($"Failed to load song parts offline: {ex.Message}");
            return false;
        }
    }
}
