using RpdPlayerApp.Models;

namespace RpdPlayerApp.Managers;

internal static class NewsManager
{
    internal const string SONGPARTS = "SONGPARTS";
    internal static bool IsTestMode { get; set; } = false;
    internal static List<SongPart> SongPartsDifference { get; set; } = [];
}
