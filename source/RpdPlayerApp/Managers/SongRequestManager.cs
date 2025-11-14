using RpdPlayerApp.Repositories;
using RpdPlayerApp.Services;

namespace RpdPlayerApp.Managers;

internal static class SongRequestManager
{
    internal static async Task<int> SubmitSongRequest(string title,
                                                      string artist,
                                                      string songPart,
                                                      bool withDancePractice,
                                                      string deviceId,
                                                      string requestedBy = "Guest",
                                                      string note = "",
                                                      bool enforceCooldown = true,
                                                      string origin = "REQUEST_PAGE")
    {
        int success = -1;
        try
        {
            success = await SongRequestRepository.InsertSongRequestAsync(new()
            {
                Artist = artist,
                Title = title,
                Part = songPart,
                WithDancePractice = withDancePractice,
                RequestedBy = requestedBy,
                DeviceId = deviceId,
                Note = note,
                Origin = origin
            }, enforceCooldown: enforceCooldown);
        }
        catch (Exception ex)
        {
            DebugService.Instance.Debug($"SongRequestManager: {ex.Message}");
        }
        return success;
    }
}
