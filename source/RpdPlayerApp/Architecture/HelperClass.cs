using CommunityToolkit.Maui.Alerts;

namespace RpdPlayerApp.Architecture
{
    internal static class HelperClass
    {
        public static bool HasInternetConnection()
        {
            NetworkAccess accessType = Connectivity.Current.NetworkAccess;

            if (accessType != NetworkAccess.Internet)
            {
                Toast.Make($"No internet connection!", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
                return false;
            }
            return true;
        }

        public static string ReadTextFile(string filePath)
        {
            try
            {
                // Ensure the file exists
                if (File.Exists(filePath))
                {
                    // Read the contents of the file
                    string fileContent = File.ReadAllText(filePath);
                    return fileContent;
                }
                else
                {
                    return "File not found.";
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during file reading
                return $"An error occurred: {ex.Message}";
            }
        }
    }
}
