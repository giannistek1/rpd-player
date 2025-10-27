using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace RpdPlayerApp.DTO;

[Table("song_request")]
internal class SongRequestDto : BaseModel
{
    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Column("artist")]
    public string Artist { get; set; } = string.Empty;

    [Column("part")]
    public string Part { get; set; } = string.Empty;

    [Column("video_request")]
    public bool? WithDancePractice { get; set; }

    [Column("requested_by")]
    public string RequestedBy { get; set; } = string.Empty;

    [Column("note")]
    public string Note { get; set; } = string.Empty;

    [Column("device_id")]
    public string DeviceId { get; set; } = string.Empty;
}
