using Newtonsoft.Json;
using RpdPlayerApp.Models;
using RpdPlayerApp.Repositories;
using RpdPlayerApp.Services;

namespace RpdPlayerApp.Managers;

internal static class NewsManager
{
    internal const string SONGPARTS = "SONGPARTS";

    internal static List<SongPart> SongPartsDifference { get; set; } = [];

    internal static async Task SaveNews()
    {
        string jsonSongParts = JsonConvert.SerializeObject(SongPartRepository.SongParts);
        string path = await FileManager.SaveJsonToFileAsync($"{SONGPARTS}", jsonSongParts);

        DebugService.Instance.Debug($"{path} - {jsonSongParts.Length}");
    }
}
