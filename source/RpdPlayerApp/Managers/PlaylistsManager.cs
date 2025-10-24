using RpdPlayerApp.Architecture;
using RpdPlayerApp.Enums;
using RpdPlayerApp.Models;
using RpdPlayerApp.Services;
using System.Text;

namespace RpdPlayerApp.Managers;

/// <summary> Responsible for saving, loading playlists and if segments are in playlists </summary>
internal static class PlaylistsManager
{
    internal static PlaylistModeValue PlaylistMode { get; set; } = PlaylistModeValue.Local;

    /// <summary> Saves playlist locally and overwrites name. </summary>
    /// <param name="playlist"></param>
    /// <param name="name"></param>
    internal static async Task<string> SavePlaylistLocally(Playlist playlist, string name)
    {
        try
        {
            StringBuilder sb = new();

            // Header should contain: Creation date, last modified date, user, count, length
            sb.AppendLine($"HDR:[{playlist.CreationDate}][{playlist.LastModifiedDate}][{AppState.Username}][{playlist.Count}][{playlist.Length}][{playlist.CountdownMode}]");

            foreach (SongPart songPart in playlist.Segments)
            {
                sb.AppendLine($"{{{songPart.ArtistName}}}{{{songPart.AlbumTitle}}}{{{songPart.Title}}}{{{songPart.PartNameShort}}}{{{songPart.PartNameNumber}}}{{{songPart.ClipLength}}}{{{songPart.AudioURL}}}");
            }

            var path = await FileManager.SavePlaylistStringToTextFileAsync($"{name}", sb.ToString());

            General.ShowToast($"{name} saved locally!");

            return path;
        }
        catch (Exception ex)
        {
            General.ShowToast(ex.Message);
            return string.Empty;
        }
    }


    internal static async Task GeneratePlaylistFromSongParts(List<SongPart> availableSegments, int durationInMinutes = 120)
    {
        string name = General.GenerateRandomName();

        List<SongPart> songs = GenerateSegmentsFromSongParts(availableSegments, durationInMinutes);

        Playlist playlist = new Playlist(creationDate: DateTime.Now, lastModifiedDate: DateTime.Now, name: name);

        playlist.Segments = new(songs);
        string path = await SavePlaylistLocally(playlist, name);

        playlist.LocalPath = path;

        CacheState.LocalPlaylists?.Add(playlist);
    }

    /// <summary> Generates random playlist based on songparts until max duration or when songparts run out. </summary>
    private static List<SongPart> GenerateSegmentsFromSongParts(List<SongPart> availableSegments, int durationInMinutes = 120)
    {
        try
        {
            if (availableSegments == null || availableSegments.Count == 0)
                throw new ArgumentException("No song parts available.");

            List<SongPart> playlist = new();
            double totalDurationInSeconds = 0;

            // Shuffle available segments randomly
            List<SongPart> shuffled = availableSegments.OrderBy(_ => General.Rng.Next()).ToList();

            // Keep adding random parts until duration reached or we run out
            foreach (SongPart part in shuffled)
            {
                double durationInSeconds = durationInMinutes * 60;

                if (totalDurationInSeconds + part.ClipLength > durationInSeconds)
                    break;

                playlist.Add(part);
                totalDurationInSeconds += part.ClipLength;
            }

            return playlist;
        }
        catch (Exception ex)
        {
            General.ShowToast($"Error generating playlist: {ex.Message}");
            return new List<SongPart>();
        }
    }

    internal static async Task<bool> TryAddSongPartToLocalPlaylist(string playlistName, SongPart songPartToAdd)
    {
        // See if local playlist exists.
        var matchingPlaylist = CacheState.LocalPlaylists?.AsEnumerable().FirstOrDefault(p => p.Name.Equals(playlistName, StringComparison.OrdinalIgnoreCase));
        if (matchingPlaylist is null)
        {
            var playlist = new Playlist(creationDate: DateTime.Now, lastModifiedDate: DateTime.Now, name: playlistName);
            playlist.Segments.Add(songPartToAdd);

            CacheState.LocalPlaylists?.Add(playlist);

            await SavePlaylistLocally(playlist, playlistName);
            return true;
        }
        else // Update
        {
            if (!SegmentIsInPlaylist(playlistName, playlistMode: PlaylistModeValue.Local, songPartToAdd))
            {
                matchingPlaylist.Segments.Add(songPartToAdd);

                await SavePlaylistLocally(matchingPlaylist, playlistName);
                return true;
            }
            // Song already in playlist.
            return false;
        }
    }

    internal static bool SegmentIsInPlaylist(string playlistName, PlaylistModeValue playlistMode, SongPart? segment)
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

        if (playlist is not null && playlist.Segments.Any(s => s.AudioURL == segment?.AudioURL))
        {
            return true;
        }

        return false;
    }
}
