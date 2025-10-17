using RpdPlayerApp.Architecture;
using Supabase;

namespace RpdPlayerApp.Services;

public static class SupabaseService
{
    private static Client? _client;

    public static async Task InitializeAsync()
    {
        var url = Constants.BASE_URL;
        var key = Constants.APIKEY;

        _client = new Client(url, key, new SupabaseOptions
        {
            AutoRefreshToken = true,
            AutoConnectRealtime = false
        });

        await _client.InitializeAsync();
    }

    public static Client Client => _client;
}
