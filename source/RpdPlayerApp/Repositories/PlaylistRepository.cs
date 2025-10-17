using Newtonsoft.Json.Linq;
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

            var fileName = segment.AudioURL.Replace(baseUrl, string.Empty);
            fileName = fileName.Replace(Constants.RAW_PARAMETER, string.Empty);
            var encodedFileName = Uri.EscapeDataString(fileName);

            var fullUrl = baseUrl + encodedFileName + Constants.RAW_PARAMETER;

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
             .Where(x => x.Owner == AppState.Username || x.DeviceId == AppState.DeviceId)
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
            .Where(x => x.IsActive == true)
            .Where(x => x.IsPublic == true)
            .Get();

        return response.Models;
    }
}
