using RpdPlayerApp.Models;
using System.Collections.ObjectModel;

namespace RpdPlayerApp.Architecture;

internal static class CacheState
{
    /// <summary> Global IsDirty flag. Triggers refresh playlists for for example SettingsPage. </summary>
    internal static bool IsDirty { get; set; }
    internal static ObservableCollection<Playlist> LocalPlaylists { get; set; } = [];
    internal static ObservableCollection<Playlist> CloudPlaylists { get; set; } = [];
    internal static ObservableCollection<Playlist> PublicPlaylists { get; set; } = [];
}
