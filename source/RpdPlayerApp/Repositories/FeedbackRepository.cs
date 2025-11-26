using RpdPlayerApp.Architecture;
using RpdPlayerApp.DTO;
using RpdPlayerApp.Enums;
using RpdPlayerApp.Services;


namespace RpdPlayerApp.Repositories;

internal static class FeedbackRepository
{
    private static DateTime _lastRequestTime = DateTime.MinValue;
    private static readonly TimeSpan _cooldown = TimeSpan.FromSeconds(5); // Cooldown period

    public static async Task<SubmitFeedbackResultValue> InsertFeedbackAsync(FeedbackDto feedback)
    {
        if (Constants.APIKEY.IsNullOrWhiteSpace()) { return SubmitFeedbackResultValue.ApiKeyMissing; }

        // Enforce cooldown
        var timeSinceLast = DateTime.UtcNow - _lastRequestTime;
        if (timeSinceLast < _cooldown)
        {
            return SubmitFeedbackResultValue.Cooldown;
        }

        _lastRequestTime = DateTime.UtcNow;

        try
        {
            await SupabaseService.Client.From<FeedbackDto>().Insert(feedback);
            return SubmitFeedbackResultValue.Success;
        }
        catch (Exception ex)
        {
            DebugService.Instance.Error($"Supabase insert feedback failed: {ex.Message}");
            return SubmitFeedbackResultValue.Error;
        }
    }
}
