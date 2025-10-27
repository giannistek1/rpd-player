using RpdPlayerApp.Enums;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace RpdPlayerApp.DTO;

[Table("log")]
internal class LogDto : BaseModel
{
    [Column("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [Column("category")]
    public LogCategoryValue Category { get; set; }
    [Column("message")]
    public string Message { get; set; } = string.Empty;

    [Column("session_id")]
    public string SessionId { get; set; } = string.Empty;

    [Column("device_id")]
    public string DeviceId { get; set; } = string.Empty;
}
