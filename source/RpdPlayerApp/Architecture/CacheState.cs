using RpdPlayerApp.Models;
using System.Collections.ObjectModel;

namespace RpdPlayerApp.Architecture;

internal static class CacheState
{
    internal static ObservableCollection<Playlist>? LocalPlaylists { get; set; } = null;
    internal static ObservableCollection<Playlist>? CloudPlaylists { get; set; } = null;
    internal static ObservableCollection<Playlist>? PublicPlaylists { get; set; } = null;
}
