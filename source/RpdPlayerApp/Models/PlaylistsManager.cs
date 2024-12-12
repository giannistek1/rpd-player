using RpdPlayerApp.ViewModels;

namespace RpdPlayerApp.Models;

internal class PlaylistsManager
{
    internal PlaylistsManager()
    {
            
    }

    internal void AddToPlaylist(string name, SongPart songPartToAdd)
    {
        var playlistExists = MainViewModel.Playlists.AsEnumerable().Where(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        if (playlistExists is null) 
        {
            var playlist = new Playlist(name);
            playlist.SongParts.Add(songPartToAdd);

            MainViewModel.Playlists.Add(playlist);
        }
        else
        {
            playlistExists.SongParts.Add(songPartToAdd);
        }
    }
}
