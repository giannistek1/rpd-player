using RpdPlayerApp.Architecture;
using RpdPlayerApp.Models;
using RpdPlayerApp.Services;
using System.Text.Json;

namespace RpdPlayerApp.Managers;

internal static class FileManager
{
    internal static readonly string AppDataDirectory = FileSystem.Current.AppDataDirectory; // Data/user/0 (Data/Data) directory (only accessed via rooted Android)
    //internal static readonly string PlaylistsDirectory = "localplaylists";

    private static readonly string SongPartsFilePath = Path.Combine(FileSystem.AppDataDirectory, "songparts.json");

    internal static string GetPlaylistsPath()
    {
        return AppDataDirectory;
    }

    #region Playlist

    /// <summary> Without the .txt </summary>
    /// <param name="fileName"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    internal static async Task<string> SavePlaylistStringToTextFileAsync(string fileName, string text)
    {
        try
        {
            DebugService.Instance.Debug($"FileManager: Saving playlist to {fileName}.txt");

            // Combine directory with filename.
            var fullPath = Path.Combine(AppDataDirectory, $"{fileName}.txt");

            // Write the string to the file.
            await File.WriteAllTextAsync(fullPath, text);

            General.ShowToast($"{fileName} saved.");
            return fullPath;
        }
        catch (Exception ex)
        {
            DebugService.Instance.Error(ex.Message);
        }

        return string.Empty;
    }

    #endregion

    /// <summary> Without the .txt </summary>
    /// <param name="fileName"></param>
    /// <param name="jsonText"></param>
    /// <returns></returns>
    internal static async Task<string> SaveJsonToFileAsync(string fileName, string jsonText)
    {
        try
        {
            var fullPath = Path.Combine(AppDataDirectory, $"{fileName}.json");

            // Write the JSON text to the file.
            await File.WriteAllTextAsync(fullPath, jsonText);
            return fullPath;
        }
        catch (Exception ex)
        {
            DebugService.Instance.Error(ex.Message);
        }

        return string.Empty;
    }

    public static async Task SaveSongPartsAsync(List<SongPart> songParts)
    {
        if (songParts.Count == 0)
        {
            DebugService.Instance.Debug("FileManager: No songparts to save.");
            return;
        }

        var json = JsonSerializer.Serialize(songParts);
        await File.WriteAllTextAsync(SongPartsFilePath, json);
    }

    public static async Task<List<SongPart>> LoadSongPartsAsync()
    {
        if (!File.Exists(SongPartsFilePath)) return [];

        try
        {
            var json = await File.ReadAllTextAsync(SongPartsFilePath);
            return JsonSerializer.Deserialize<List<SongPart>>(json) ?? [];
        }
        catch (Exception ex)
        {
            DebugService.Instance.Error($"FileManager: {ex.Message}");
            return [];
        }
    }

    public static async Task SaveArtistsAsync(List<Artist> artists)
    {
        if (artists.Count == 0)
        {
            DebugService.Instance.Debug("FileManager: No artists to save.");
            return;
        }

        var json = JsonSerializer.Serialize(artists);
        var path = Path.Combine(FileSystem.AppDataDirectory, "artists.json");
        await File.WriteAllTextAsync(path, json);
    }

    public static async Task<List<Artist>> LoadArtistsAsync()
    {
        var path = Path.Combine(FileSystem.AppDataDirectory, "artists.json");
        if (!File.Exists(path)) { return []; }

        try
        {
            var json = await File.ReadAllTextAsync(path);
            return JsonSerializer.Deserialize<List<Artist>>(json) ?? [];
        }
        catch (Exception ex)
        {
            DebugService.Instance.Error($"FileManager: {ex.Message}");
            return [];
        }
    }

    public static async Task SaveAlbumsAsync(List<Album> albums)
    {
        if (albums.Count == 0)
        {
            DebugService.Instance.Debug("FileManager: No albums to save.");
            return;
        }

        var json = JsonSerializer.Serialize(albums);
        var path = Path.Combine(FileSystem.AppDataDirectory, "albums.json");
        await File.WriteAllTextAsync(path, json);
    }

    public static async Task<List<Album>> LoadAlbumsAsync()
    {
        var path = Path.Combine(FileSystem.AppDataDirectory, "albums.json");
        if (!File.Exists(path)) { return []; }

        try
        {
            var json = await File.ReadAllTextAsync(path);
            return JsonSerializer.Deserialize<List<Album>>(json) ?? [];
        }
        catch (Exception ex)
        {
            DebugService.Instance.Error($"FileManager: {ex.Message}");
            return [];
        }
    }
}
