using RpdPlayerApp.Architecture;
using RpdPlayerApp.Items;
using RpdPlayerApp.Models;
using RpdPlayerApp.Services;
using RpdPlayerApp.ViewModels;
using System.Text.Json;

namespace RpdPlayerApp.Managers;

internal static class FileManager
{
    internal static readonly string AppDataDirectory = FileSystem.Current.AppDataDirectory; // Data/user/0 (Data/Data) directory (only accessed via rooted Android)
    //internal static readonly string PlaylistsDirectory = "localplaylists";

    internal static string GetPlaylistsPath()
    {
        return AppDataDirectory;
    }

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

    /// <summary> Without the .txt </summary>
    /// <param name="fileName"></param>
    /// <param name="jsonText"></param>
    /// <returns></returns>
    internal static async Task<string> SaveJsonToFileAsync(string fileName, string jsonText)
    {
        try
        {
            var fullPath = Path.Combine(AppDataDirectory, $"{fileName}.txt");

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

    internal static async Task<List<SongPart>?> LoadSongSegmentsFromFilePath(string filePath)
    {
        var fullPath = Path.Combine(AppDataDirectory, filePath);

        try
        {
            if (File.Exists(fullPath))
            {
                // Read the text file content
                string fileContent = await File.ReadAllTextAsync(fullPath);

                //DebugService.Instance.Debug($"{fileContent}");

                // Convert the text content into a list of objects
                var songs = JsonSerializer.Deserialize<List<SongPart>>(fileContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                //DebugService.Instance.Debug($"songs: {songs?.Count}");

                return songs;
            }
        }
        catch (Exception ex)
        {
            DebugService.Instance.Error($"FileManager: {ex.Message}");
        }

        return null;
    }
}
