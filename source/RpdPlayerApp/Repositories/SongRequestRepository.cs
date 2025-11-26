using RpdPlayerApp.Architecture;
using RpdPlayerApp.DTO;
using RpdPlayerApp.Enums;
using RpdPlayerApp.Services;

namespace RpdPlayerApp.Repositories;

internal static class SongRequestRepository
{
    private static DateTime _lastRequestTime = DateTime.MinValue;
    private static readonly TimeSpan _cooldown = TimeSpan.FromSeconds(5); // Cooldown period

    /// <summary> </summary>
    /// <param name="request"></param>
    /// <param name="enforceCooldown"></param>
    /// <returns>1 success, -3 API key is missing, -2 cooldown, -1 error</returns>
    public static async Task<SongRequestResultValue> InsertSongRequestAsync(SongRequestDto request, bool enforceCooldown = true)
    {
        if (Constants.APIKEY.IsNullOrWhiteSpace()) { return SongRequestResultValue.ApiKeyMissing; }

        // Enforce cooldown
        var timeSinceLast = DateTime.UtcNow - _lastRequestTime;
        if (enforceCooldown && timeSinceLast < _cooldown)
        {
            return SongRequestResultValue.Cooldown;
        }

        _lastRequestTime = DateTime.UtcNow;

        try
        {
            await SupabaseService.Client.From<SongRequestDto>().Insert(request);
            return SongRequestResultValue.Success;
        }
        catch (Exception ex)
        {
            DebugService.Instance.Error($"Supabase insert songrequest failed: {ex.Message}");
            return SongRequestResultValue.Error;
        }
    }
}
