using RpdPlayerApp.Architecture;
using RpdPlayerApp.Models;
using System.Collections.ObjectModel;

namespace RpdPlayerApp.ViewModel;

internal static class MainViewModel
{
    /// <summary>
    /// 7
    /// </summary>
    public static readonly int SongPartPropertyAmount = 7;

    public static readonly string Path = FileSystem.Current.AppDataDirectory;

    public static SortMode SortMode { get; set; }
    
    public static SongPart? CurrentSongPart { get; set; } = null;

    public static Queue<SongPart> SongPartsQueue { get; set; } = new Queue<SongPart>();

    public static PlayMode PlayMode { get; set; }

    public static bool IsPlayingPlaylist { get; set; }

    public static ObservableCollection<Playlist> Playlists { get; set; } = new ObservableCollection<Playlist>();


}
