using RpdPlayerApp.Architecture;
using RpdPlayerApp.DTO;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace RpdPlayerApp.Repositories;

internal class SongRequestRepository
{
    private readonly HttpClient _httpClient;

    public SongRequestRepository()
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

    public async Task<bool> InsertSongRequestAsync(SongRequestDto request)
    {
        // The Supabase REST API expects JSON in the request body
        string url = Constants.SONGREQUEST_ROUTE;
        var json = JsonSerializer.Serialize(request);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
            return true;
        }
        else
        {
            string respText = await response.Content.ReadAsStringAsync();
            // Log or handle error (status, body)
            DebugService.Instance.AddDebug($"Supabase insert failed: {response.StatusCode} / {respText}");
            return false;
        }
    }
}
