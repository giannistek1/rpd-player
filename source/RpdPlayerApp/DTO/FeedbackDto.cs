using System.Text.Json.Serialization;

namespace RpdPlayerApp.DTO;


public class FeedbackDto
{
    // Json is case sensitive.
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("is_bug")]
    public bool IsBug { get; set; }

    [JsonPropertyName("requested_by")]
    public string RequestedBy { get; set; } = string.Empty;

    [JsonPropertyName("device_id")]
    public string DeviceId { get; set; } = string.Empty;
}
