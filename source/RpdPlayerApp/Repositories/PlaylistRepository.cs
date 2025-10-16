using RpdPlayerApp.Architecture;
using RpdPlayerApp.DTO;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace RpdPlayerApp.Repositories;

internal class PlaylistRepository
{
    private readonly HttpClient _httpClient;

    private static DateTime _lastRequestTime = DateTime.MinValue;
    private static readonly TimeSpan _cooldown = TimeSpan.FromSeconds(10); // Cooldown period

    public PlaylistRepository()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(Constants.BASE_URL)
        };
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Constants.APIKEY);
        _httpClient.DefaultRequestHeaders.Add("apikey", Constants.APIKEY);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<List<PlaylistDto>> GetAllPublicPlaylists()
    {
        if (Constants.APIKEY.IsNullOrWhiteSpace()) { General.ShowToast("APIKEY is missing."); return []; }

        // Enforce cooldown
        var timeSinceLast = DateTime.UtcNow - _lastRequestTime;
        if (timeSinceLast < _cooldown)
        {
            return [];
        }

        _lastRequestTime = DateTime.UtcNow;

        // The Supabase REST API expects JSON in the request body
        string url = $"{Constants.BASE_URL}{Constants.PLAYLIST_ROUTE}?is_active=eq.true&is_public=eq.true";

        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            var text = await response.Content.ReadAsStringAsync();
            throw new Exception($"Supabase GET failed: {response.StatusCode} — {text}");
        }

        var list = await response.Content.ReadFromJsonAsync<List<PlaylistDto>>(new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return list ?? new List<PlaylistDto>();
    }
}
