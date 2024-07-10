using RpdPlayerApp.Architecture;
using RpdPlayerApp.Models;

namespace RpdPlayerApp.ViewModel;

internal static class MainViewModel
{
    public static SortMode SortMode { get; set; }
    
    public static SongPart? CurrentSongPart { get; set; } = null;

    public static Queue<SongPart> SongPartsQueue { get; set; } = new Queue<SongPart>();

    public static bool IsPlayingPlaylist { get; set; }
    
}
