using RpdPlayerApp.Architecture;
using RpdPlayerApp.Models;
using System.Text;

namespace RpdPlayerApp.Managers;

internal class PlaylistsManager
{
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

    internal async Task<bool> TryAddSongPartToPlaylist(string playlistName, SongPart songPartToAdd)
    {
        var playlistExists = CacheState.LocalPlaylists?.AsEnumerable().FirstOrDefault(p => p.Name.Equals(playlistName, StringComparison.OrdinalIgnoreCase));
        if (playlistExists is null)
        {
            var playlist = new Playlist(creationDate: DateTime.Now, lastModifiedDate: DateTime.Now, name: playlistName);
            playlist.SongParts.Add(songPartToAdd);

            CacheState.LocalPlaylists?.Add(playlist);

            await SavePlaylistLocally(playlist, playlistName);
            return true;
        }
        else // Update
        {
            if (!SongPartIsInPlaylist(playlistName, songPartToAdd))
            {
                playlistExists.SongParts.Add(songPartToAdd);
                await SavePlaylistLocally(playlistExists, playlistName);
                return true;
            }
            // Song already in playlist.
            return false;
        }
    }

    internal bool SongPartIsInPlaylist(string playlistName, SongPart? songPart)
    {
        var playlist = CacheState.LocalPlaylists.AsEnumerable().FirstOrDefault(p => p.Name.Equals(playlistName, StringComparison.OrdinalIgnoreCase));

        if (playlist is not null && playlist.SongParts.Any(s => s.AudioURL == songPart?.AudioURL))
        {
            return true;
        }

        return false;
    }
}
