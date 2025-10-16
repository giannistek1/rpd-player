using RpdPlayerApp.Models;
using System.Collections.ObjectModel;

namespace RpdPlayerApp.Architecture;

internal class CacheState
{
    internal static ObservableCollection<Playlist>? LocalPlaylists { get; set; } = null;
    internal static ObservableCollection<Playlist>? CloudPlaylists { get; set; } = null;
    internal static ObservableCollection<Playlist>? PublicPlaylists { get; set; } = null;
}
