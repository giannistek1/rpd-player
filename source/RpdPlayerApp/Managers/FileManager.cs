using RpdPlayerApp.Architecture;
using RpdPlayerApp.Items;
using RpdPlayerApp.ViewModels;
using System.Text.Json;

namespace RpdPlayerApp.Managers;

internal static class FileManager
{
    internal static readonly string AppDataDirectory = FileSystem.Current.AppDataDirectory;
    //internal static readonly string PlaylistsDirectory = "localplaylists";
    internal static string GetPlaylistsPath()
    {
        return AppDataDirectory;
    }
    internal static async Task<string> SavePlaylistJsonToFileAsync(string fileName, string jsonText)
    {
        try
        {
            // Combine directory with filename.
            var fullPath = Path.Combine(AppDataDirectory, $"{fileName}.txt");

            // Write the JSON text to the file.
            await File.WriteAllTextAsync(fullPath, jsonText);

            HelperClass.ShowToast($"{fileName} saved.");
            return fullPath;
        }
        catch (Exception ex) { HelperClass.ShowToast(ex.Message); }

        return string.Empty;
    }

    internal static async Task<string> SaveJsonToFileAsync(string fileName, string jsonText)
    {
        try
        {
            var fullPath = Path.Combine(AppDataDirectory, $"{fileName}.txt");

            // Write the JSON text to the file.
            await File.WriteAllTextAsync(fullPath, jsonText);

            HelperClass.ShowToast($"{fileName} saved.");
            return fullPath;
        }
        catch (Exception ex) { HelperClass.ShowToast(ex.Message); }

        return string.Empty;
    }

    internal static async Task<List<NewsItem>?> LoadNewsItemsFromFilePath(string filePath)
    {
        var fullPath = Path.Combine(AppDataDirectory, filePath);
        
        if (File.Exists(fullPath))
        {
            // Read the text file content
            string fileContent = await File.ReadAllTextAsync(fullPath);

            // Convert the text content into a list of objects
            var people = JsonSerializer.Deserialize<List<NewsItem>>(fileContent);

            return people;
        }

        return null;
    }
}
