using RpdPlayerApp.Models;
using RpdPlayerApp.Repositories;
using RpdPlayerApp.Services;
using System.Text.Json;

namespace RpdPlayerApp.Managers;

internal static class NewsManager
{
    internal const string SONGPARTS = "SONGPARTS";

    internal static List<SongPart> SongPartsDifference { get; set; } = [];
}
