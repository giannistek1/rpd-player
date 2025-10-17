using RpdPlayerApp.Architecture;
using RpdPlayerApp.DTO;
using RpdPlayerApp.Services;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;


namespace RpdPlayerApp.Repositories;

class FeedbackRepository
{
    private readonly HttpClient _httpClient;

    // TODO: Integrate supabase client.
    private static DateTime _lastRequestTime = DateTime.MinValue;
    private static readonly TimeSpan _cooldown = TimeSpan.FromSeconds(10); // Cooldown period

    public FeedbackRepository()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(Constants.BASE_URL)
        };
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Constants.APIKEY);
        _httpClient.DefaultRequestHeaders.Add("apikey", Constants.APIKEY);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<int> InsertFeedbackAsync(FeedbackDto feedback)
    {
        if (Constants.APIKEY.IsNullOrWhiteSpace()) { return -3; }

        // Enforce cooldown
        var timeSinceLast = DateTime.UtcNow - _lastRequestTime;
        if (timeSinceLast < _cooldown)
        {
            return -2;
        }

        _lastRequestTime = DateTime.UtcNow;

        // The Supabase REST API expects JSON in the request body
        string url = Constants.FEEDBACK_ROUTE;
        var json = JsonSerializer.Serialize(feedback);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
            return 1;
        }
        else
        {
            string respText = await response.Content.ReadAsStringAsync();
            // Log or handle error (status, body)
            DebugService.Instance.AddDebug($"Supabase insert failed: {response.StatusCode} / {respText}");
            return -2;
        }
    }
}
