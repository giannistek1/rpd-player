using RpdPlayerApp.Architecture;
using RpdPlayerApp.DTO;
using RpdPlayerApp.Services;


namespace RpdPlayerApp.Repositories;

internal static class FeedbackRepository
{
    private static DateTime _lastRequestTime = DateTime.MinValue;
    private static readonly TimeSpan _cooldown = TimeSpan.FromSeconds(5); // Cooldown period

    public static async Task<int> InsertFeedbackAsync(FeedbackDto feedback)
    {
        if (Constants.APIKEY.IsNullOrWhiteSpace()) { return -3; }

        // Enforce cooldown
        var timeSinceLast = DateTime.UtcNow - _lastRequestTime;
        if (timeSinceLast < _cooldown)
        {
            return -2;
        }

        _lastRequestTime = DateTime.UtcNow;

        try
        {
            await SupabaseService.Client.From<FeedbackDto>().Insert(feedback);
            return 1;
        }
        catch (Exception ex)
        {
            DebugService.Instance.Error($"Supabase insert feedback failed: {ex.Message}");
            return -1;
        }
    }
}
