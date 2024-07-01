using Dropbox.Api.Files;
using Dropbox.Api;
using CommunityToolkit.Maui.Alerts;
using System.Net.Http.Headers;
using System.Text;

namespace RpdPlayerApp.Repositories
{
    internal static class DropboxRepository
    {
        private const string saveAccessToken = "sl.B4IauGQZf1EKQjXG8YfRpkGrA5sB-g4px25PF8XNvWJN54p4iTSaiO4L9C8b35vtYgiv0Gq6jnnGSS_WiwJ8o81JUYZNGKLaI7n-SmQCD0zlwi5R52NQ3brQeNcgjCzUtZOYzMuIWat8";
        private const string loadAccessToken = "sl.u.AFE2lXnuEKzmnzqeZOCrKzLf4SL3FJvYP07O5oBKaSCol_h0uv_jJzDztxxNcLXpqN4Ju5yEbmPJiixBRNPpMu_0CaZvQCxrR9N-AIkNTDtsSvfyFr-HWMBG5jBxB5hj-Z43TVsunpkG5Ly79_WU_hy8xJ7VidUikQVojkV5usevFK5k0T1_jPPm3cP-5BBxurofk_-bojrlfng7A_GavGZ2tZ_HOj2aJ5oWCjVscdEQZLbSuaVKEDsezmhVeBm-d3-EuPgR0FOYAK6PHniy1e-w1mGCRjgmGB5cUGDuF2XPURKpkpPwEo99XW4JyC6XBXEz2cMmbuQCI1w7tje7vwHnqWipn9bMBbcveqhxxNpf5MP928xqSnE8N6t_MeCXHlWTZGkE_H1tEZ9_CsL2Di7pBaJaPT7MRBcjGGDfesTtpt7gbjln6l7KiY4zKtOBSGkmWZWdltwCKrNWr8FwHemIPQTRN9lpbymABJAvlCP6MMvbASsXRYal668un-MZyLuD1-gPsIp3Fehf008Vk-2pBdyOXm5eGol30jmY3aYP9mJuAemaFvX-DF7h0RVX4qCNEWTwJchK2E5nuk12sc7DgQNjOHLO3OargY1_MI-aeLXLkPNV4yn1dkKt_NmDNiW1nz5kDN2ylDbo_UhMsPpOMfP8LKA4q_VDaNPDlQYltCH3iMQ35aLYzO844ykL74IoqboIBzs2bp99p2c4io3OtVAKZAnbrvSDlapSV9Nl1i_Fj6f-iFur2izTU4nnosQ8aydAUo-DrzdIIE6HQB_FjWp8ecVjb0vIsSeaVbx_CLReUERiY7aafk9tFOjhvXpMmYMTJgVYhobHLgT8-whsj9n50QEfob2Jcyebq4a0gH_gnD2zBWRXUQdKjInIrCrjZZmA8UpT0kRRAM4s9tEVDOMe1IrmxCuVRPZUOJ1Yl0iRfG2SZuJ_P38dwIRTsBifkSOBNoSP59RM1V-6WQbMPsg0w56z9JajPUKgi3BP9RDq6J9guaa3FF_6LSKgpgz7IW1mv3qbJNYDKVB1xiKo_a16KQUWD8KujHcpim8GOLzFWUxL_5dgYc__j-pjcnxAVr5PqcVluD85w_KQBlfkGcc37Hcw18LxAfcnsuPEkT7oj4h-rW7XzLcZac9bSOrBWXBYHNnS7zRRhAPl7tuZWumqAg-gRn6VnwOmldt4MLHMMsvfSKYZUck-0OBZlAx2D2wzP4u0UETdYuEuvbANGmpiDEi4pem-s7ZWUn8YZ6_c5FHSX2zNtu1lpPFVGbE4f9QA8BP83TI_xbWvPG8gueENMgC84HepvzDXE9F0_w";

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
