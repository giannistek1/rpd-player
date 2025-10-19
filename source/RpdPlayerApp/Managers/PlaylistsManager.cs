using RpdPlayerApp.Architecture;
using RpdPlayerApp.Enums;
using RpdPlayerApp.Models;
using System.Text;

namespace RpdPlayerApp.Managers;

internal class PlaylistsManager
{
    internal static PlaylistModeValue PlaylistMode { get; set; } = PlaylistModeValue.Local;

    internal PlaylistsManager() { }

    /// <summary> Saves playlist locally and overwrites name. </summary>
    /// <param name="playlist"></param>
    /// <param name="name"></param>
    internal static async Task SavePlaylistLocally(Playlist playlist, string name)
    {
        try
        {
            StringBuilder sb = new();

            // Header should contain: Creation date, last modified date, user, count, length
            sb.AppendLine($"HDR:[{playlist.CreationDate}][{playlist.LastModifiedDate}][{AppState.Username}][{playlist.Count}][{playlist.Length}][{playlist.CountdownMode}]");

            foreach (SongPart songPart in playlist.SongParts)
            {
                sb.AppendLine($"{{{songPart.ArtistName}}}{{{songPart.AlbumTitle}}}{{{songPart.Title}}}{{{songPart.PartNameShort}}}{{{songPart.PartNameNumber}}}{{{songPart.ClipLength}}}{{{songPart.AudioURL}}}");
            }

            await FileManager.SavePlaylistStringToTextFileAsync($"{name}", sb.ToString());

            General.ShowToast($"{name} saved locally!");
        }
        catch (Exception ex)
        {
            General.ShowToast(ex.Message);
        }
    }

    internal async Task<bool> TryAddSongPartToLocalPlaylist(string playlistName, SongPart songPartToAdd)
    {
        // See if local playlist exists.
        var matchingPlaylist = CacheState.LocalPlaylists?.AsEnumerable().FirstOrDefault(p => p.Name.Equals(playlistName, StringComparison.OrdinalIgnoreCase));
        if (matchingPlaylist is null)
        {
            var playlist = new Playlist(creationDate: DateTime.Now, lastModifiedDate: DateTime.Now, name: playlistName);
            playlist.SongParts.Add(songPartToAdd);
            playlist.SetCount();
            playlist.SetLength();

            CacheState.LocalPlaylists?.Add(playlist);

            await SavePlaylistLocally(playlist, playlistName);
            return true;
        }
        else // Update
        {
            if (!SegmentIsInPlaylist(playlistName, playlistMode: PlaylistModeValue.Local, songPartToAdd))
            {
                // TODO: When favoriting, the updated length and count does not reflect in the library view.
                CacheState.LocalPlaylists.Remove(matchingPlaylist);

                matchingPlaylist.SongParts.Add(songPartToAdd);
                matchingPlaylist.SetCount();
                matchingPlaylist.SetLength();

                CacheState.LocalPlaylists.Add(matchingPlaylist);

                await SavePlaylistLocally(matchingPlaylist, playlistName);
                return true;
            }
            // Song already in playlist.
            return false;
        }
    }

    internal bool SegmentIsInPlaylist(string playlistName, PlaylistModeValue playlistMode, SongPart? segment)
    {
        Playlist? playlist = null;
        if (playlistMode == PlaylistModeValue.Local)
        {
            playlist = CacheState.LocalPlaylists!.AsEnumerable().FirstOrDefault(p => p.Name.Equals(playlistName, StringComparison.OrdinalIgnoreCase));
        }
        else if (playlistMode == PlaylistModeValue.Cloud)
        {
            playlist = CacheState.CloudPlaylists!.AsEnumerable().FirstOrDefault(p => p.Name.Equals(playlistName, StringComparison.OrdinalIgnoreCase));
        }

        if (playlist is not null && playlist.SongParts.Any(s => s.AudioURL == segment?.AudioURL))
        {
            return true;
        }

        return false;
    }
}
