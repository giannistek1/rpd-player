using RpdPlayerApp.Models;

namespace RpdPlayerApp.Managers;

internal class PlaylistsManager
{
    internal PlaylistsManager() { }

    internal bool TryAddSongPartToPlaylist(string playlistName, SongPart songPartToAdd)
    {
        var playlistExists = AppState.Playlists.AsEnumerable().FirstOrDefault(p => p.Name.Equals(playlistName, StringComparison.OrdinalIgnoreCase));
        if (playlistExists is null)
        {
            var playlist = new Playlist(creationDate: DateTime.Now, name: playlistName);
            playlist.SongParts.Add(songPartToAdd);

            AppState.Playlists.Add(playlist);
            return true;
        }
        else
        {
            if (!SongPartIsInPlaylist(playlistName, songPartToAdd))
            {
                playlistExists.SongParts.Add(songPartToAdd);
                return true;
            }
            // Song already in playlist.
            return false;
        }
    }

    internal bool SongPartIsInPlaylist(string playlistName, SongPart? songPart)
    {
        var playlist = AppState.Playlists.AsEnumerable().FirstOrDefault(p => p.Name.Equals(playlistName, StringComparison.OrdinalIgnoreCase));

        if (playlist is not null && playlist.SongParts.Any(s => s.AudioURL == songPart?.AudioURL))
        {
            return true;
        }

        return false;
    }
}
