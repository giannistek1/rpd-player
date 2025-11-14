using RpdPlayerApp.Models;
using RpdPlayerApp.Repositories;
using RpdPlayerApp.Services;
using System.Text.Json;

namespace RpdPlayerApp.Managers;

internal static class NewsManager
{
    internal const string SONGPARTS = "SONGPARTS";

    internal static List<SongPart> SongPartsDifference { get; set; } = [];

    internal static async Task SaveNews()
    {
        if (SongPartRepository.SongParts.Count == 0)
        {
            DebugService.Instance.Debug("NewsManager: No songparts to save.");
            return;
        }

        string jsonSongParts = JsonSerializer.Serialize(SongPartRepository.SongParts);
        string path = await FileManager.SaveJsonToFileAsync($"{SONGPARTS}", jsonSongParts);

        DebugService.Instance.Debug($"{path} - {jsonSongParts.Length}");
    }
}
