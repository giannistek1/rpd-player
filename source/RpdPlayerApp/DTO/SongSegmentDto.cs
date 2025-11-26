using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RpdPlayerApp.DTO;

public class SongSegmentDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    // No _ underscores in Json.
    [JsonPropertyName("albumName")] // TODO: Consistency: title
    public string AlbumName { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    // No _ underscores in Json.
    [JsonPropertyName("artistName")]
    public string ArtistName { get; set; }

    [JsonPropertyName("audiourl")] // TODO: fx consistency AudioUrl
    public string AudioUrl { get; set; }

    [JsonPropertyName("cliplength")]
    public double ClipLength { get; set; }

    [JsonPropertyName("segmentShort")]
    public string SegmentShort { get; set; }

    [JsonPropertyName("segmentNumber")]
    public string SegmentNumber { get; set; }
}