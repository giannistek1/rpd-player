using System.Text.Json.Serialization;

namespace RpdPlayerApp.DTO;

internal class SongRequestDto
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("artist")]
    public string Artist { get; set; } = string.Empty;

    [JsonPropertyName("part")]
    public string Part { get; set; } = string.Empty;

    [JsonPropertyName("video_request")]
    public bool? WithDancePractice { get; set; }

    [JsonPropertyName("requested_by")]
    public string RequestedBy { get; set; } = string.Empty;

    [JsonPropertyName("note")]
    public string Note { get; set; } = string.Empty;
}
