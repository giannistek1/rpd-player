using RpdPlayerApp.Architecture;
using RpdPlayerApp.DTO;
using RpdPlayerApp.Managers;
using RpdPlayerApp.Models;
using RpdPlayerApp.Services;

namespace RpdPlayerApp.Repositories;

internal class PlaylistRepository
{
    private static DateTime _lastRequestTime = DateTime.MinValue;
    private static readonly TimeSpan _cooldown = TimeSpan.FromSeconds(1); // Cooldown period

    public PlaylistRepository() { }

    public async Task<int> SaveCloudPlaylist(long id, DateTime creationDate, string name, long lengthInSeconds, int count, List<SongPart> songSegments, bool isPublic)
    {
        List<SongSegmentDto> segments = [];

        for (int i = 0; i < songSegments.Count; i++)
        {
            var segment = songSegments[i];

            var baseUrl = Constants.SONGPARTS_BASE_URL;

            var fileName = segment.AudioURL.Replace(baseUrl, string.Empty)
                                           .Replace(Constants.RAW_PARAMETER, string.Empty);

            // Only encode if not already encoded
            if (!fileName.Contains("%"))
            {
                fileName = Uri.EscapeDataString(fileName);
            }

            var fullUrl = baseUrl + fileName + Constants.RAW_PARAMETER;

            segments.Add(new SongSegmentDto
            {
                Id = i,
                AlbumName = segment.AlbumTitle,
                Title = segment.Title,
                ArtistName = segment.ArtistName,
                SegmentShort = segment.PartNameShort,
                SegmentNumber = segment.PartNameNumber,
                AudioUrl = fullUrl,
                ClipLength = segment.ClipLength,
            });
        }

        var model = new PlaylistDto
        {
            Id = id,
            CreationDate = creationDate,
            LastModifiedDate = DateTime.Now,
            IsActive = true,
            IsPremade = false,
            IsPublic = isPublic,
            Name = name,
            Owner = AppState.Username,
            DeviceId = AppState.DeviceId,
            LengthInSeconds = lengthInSeconds,
            Count = count,
            Segments = segments
        };
        await SupabaseService.Client.From<PlaylistDto>().Upsert(model);

        return 1;
    }

    public async Task<List<PlaylistDto>> GetCloudPlaylists()
    {
        if (Constants.APIKEY.IsNullOrWhiteSpace()) { General.ShowToast("APIKEY is missing."); return []; }

        // Enforce cooldown
        var timeSinceLast = DateTime.UtcNow - _lastRequestTime;
        if (timeSinceLast < _cooldown)
        {
            var remaining = _cooldown - timeSinceLast;
            await Task.Delay(remaining);
        }

        _lastRequestTime = DateTime.UtcNow;

        var response = await SupabaseService.Client
             .From<PlaylistDto>()
             .Where(p => p.Owner == AppState.Username || p.DeviceId == AppState.DeviceId)
             .Where(p => p.IsActive == true)
             .Get();

        return response.Models;
    }

    public async Task<List<PlaylistDto>> GetAllPublicPlaylists()
    {
        if (Constants.APIKEY.IsNullOrWhiteSpace()) { General.ShowToast("APIKEY is missing."); return []; }

        // Enforce cooldown
        var timeSinceLast = DateTime.UtcNow - _lastRequestTime;
        if (timeSinceLast < _cooldown)
        {
            var remaining = _cooldown - timeSinceLast;
            await Task.Delay(remaining);
        }

        _lastRequestTime = DateTime.UtcNow;

        var response = await SupabaseService.Client
            .From<PlaylistDto>()
            .Where(x => x.IsActive == true) // Needs comparison "== true"
            .Where(x => x.IsPublic == true)
            .Get();

        return response.Models;
    }

    /// <summary> Don't actually delete playlist, but set active to false. </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    internal async Task<bool> DeleteCloudPlaylist(long id)
    {
        if (Constants.APIKEY.IsNullOrWhiteSpace()) { General.ShowToast("APIKEY is missing."); return false; }

        await SupabaseService.Client
              .From<PlaylistDto>()
              .Where(x => x.Id == id)
              .Set(x => x.IsActive, false)
              .Update();

        DebugService.Instance.Info($"Deleted cloud playlistId: {id}");
        return true;
    }
}
