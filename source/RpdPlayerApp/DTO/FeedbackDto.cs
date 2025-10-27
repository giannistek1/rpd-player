using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace RpdPlayerApp.DTO;

[Table("feedback")]
public class FeedbackDto : BaseModel
{
    [Column("text")]
    public string Text { get; set; } = string.Empty;

    [Column("is_bug")]
    public bool IsBug { get; set; }

    [Column("requested_by")]
    public string RequestedBy { get; set; } = string.Empty;

    [Column("device_id")]
    public string DeviceId { get; set; } = string.Empty;
}
