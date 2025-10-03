using Newtonsoft.Json;
using RpdPlayerApp.Items;
using RpdPlayerApp.Models;
using RpdPlayerApp.Repositories;

namespace RpdPlayerApp.Managers;

internal static class NewsManager
{
    internal const string SONGPARTS = "SONGPARTS";

    internal static List<NewsItem> SongPartsDifference { get; set; } = [];

    /// <summary>Create news items from all song parts </summary>
    /// <returns>List of news items</returns>
    internal static List<NewsItem> CreateNewsItemsFromSongParts() => [.. SongPartRepository.SongParts.Select(s => new NewsItem
    {
        Title = s.Title,
        Artist = s.ArtistName,
        Part = s.PartNameFull,
        AudioUrl = s.AudioURL,
        HasVideo = s.HasVideo
    })];

    internal static async Task SaveNews()
    {
        var newsItems = SongPartRepository.SongParts.Select(s => new NewsItem
        {
            Title = s.Title,
            Artist = s.ArtistName,
            Part = s.PartNameFull,
            AudioUrl = s.AudioURL,
            HasVideo = s.HasVideo
        }).ToList();

        var jsonSongParts = JsonConvert.SerializeObject(newsItems);
        await FileManager.SaveJsonToFileAsync($"{SONGPARTS}", jsonSongParts);
    }
}
