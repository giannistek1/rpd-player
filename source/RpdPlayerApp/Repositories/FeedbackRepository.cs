using RpdPlayerApp.Architecture;
using RpdPlayerApp.DTO;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;


namespace RpdPlayerApp.Repositories;

class FeedbackRepository
{
    private readonly HttpClient _httpClient;

    public FeedbackRepository()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(Constants.BASE_URL)
        };
        // Always send API key and required headers
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Constants.APIKEY);
        _httpClient.DefaultRequestHeaders.Add("apikey", Constants.APIKEY);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<bool> InsertFeedbackAsync(FeedbackDto feedback)
    {
        // The Supabase REST API expects JSON in the request body
        string url = "/rest/v1/feedback";
        var json = JsonSerializer.Serialize(feedback);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        // By default this is a POST request
        var response = await _httpClient.PostAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
            // Optionally parse the returned body if Supabase returns the new row.
            return true;
        }
        else
        {
            string respText = await response.Content.ReadAsStringAsync();
            // Log or handle error (status, body)
            System.Diagnostics.Debug.WriteLine($"Supabase insert failed: {response.StatusCode} / {respText}");
            return false;
        }
    }
}
