using RpdPlayerApp.ViewModels;

namespace RpdPlayerApp.Models;

internal class PlaylistsManager
{
    internal PlaylistsManager()
    {
            
    }

    internal void AddToPlaylist(string playlistName, SongPart songPartToAdd)
    {
        var playlistExists = MainViewModel.Playlists.AsEnumerable().FirstOrDefault(p => p.Name.Equals(playlistName, StringComparison.OrdinalIgnoreCase));
        if (playlistExists is null) 
        {
            var playlist = new Playlist(playlistName);
            playlist.SongParts.Add(songPartToAdd);

            MainViewModel.Playlists.Add(playlist);
        }
        else
        {
            playlistExists.SongParts.Add(songPartToAdd);
        }
    }

    internal bool IsInPlaylist(string playlistName, SongPart? songPart)
    {
        var playlist = MainViewModel.Playlists.AsEnumerable().FirstOrDefault(p => p.Name.Equals(playlistName, StringComparison.OrdinalIgnoreCase));

        if (playlist is not null && playlist.SongParts.Any(s => s.AudioURL == songPart.AudioURL))
        {
            return true;
        }

        return false;
    }
}
