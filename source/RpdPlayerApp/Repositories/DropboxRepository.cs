using Dropbox.Api.Files;
using Dropbox.Api;
using CommunityToolkit.Maui.Alerts;
using System.Net.Http.Headers;
using System.Text;

namespace RpdPlayerApp.Repositories
{
    internal static class DropboxRepository
    {
        private const string saveAccessToken = "";
        private const string loadAccessToken = "";

        // Playlists
        // Artist, title, part
        // Artist, title, part

        public async static void SavePlaylist(string playlistName)
        {
            using (var dbx = new DropboxClient(saveAccessToken))
            {
                string mainDir = FileSystem.Current.AppDataDirectory;
                string filename = $"{playlistName}.txt";
                string filePath = Path.Combine(mainDir, filename);

                string dropboxPath = $"/{playlistName}.txt";  // Path in Dropbox where the file will be uploaded, should start with /

                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    var updated = await dbx.Files.UploadAsync(
                        dropboxPath,
                        WriteMode.Overwrite.Instance,
                        body: fileStream);

                    //return $"Uploaded {updated.PathDisplay} to Dropbox.";
                }
            }
            //return "Upload failed.";
        }

        public static async Task<string> LoadPlaylist(string playlistName)
        {
            string dropboxFilePath = $"/Apps/rpd-player/myplaylist.txt";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loadAccessToken);
                client.DefaultRequestHeaders.Add("Dropbox-API-Arg", $"{{\"path\":\"{dropboxFilePath}\"}}");

                var request = new HttpRequestMessage(HttpMethod.Post, "https://content.dropboxapi.com/2/files/download");
                request.Content = new StringContent("", Encoding.UTF8, "text/plain");

                HttpResponseMessage response = await client.SendAsync(request);
                var test = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                {
                    //return test;
                }
                else
                {
                    //return "Error: " + test;
                }

                return test;
            }
        }
    }
}
