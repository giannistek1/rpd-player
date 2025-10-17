using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace RpdPlayerApp.DTO;

[Table("playlist")]
public class PlaylistDto : BaseModel
{
    [PrimaryKey("id")]
    public long Id { get; set; }

    [Column("title")] // TODO: change to name.
    public string Name { get; set; } = string.Empty;

    [Column("creation_date")]
    public DateTime CreationDate { get; set; }

    [Column("last_modified_date")]
    public DateTime LastModifiedDate { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }

    [Column("is_premade")]
    public bool IsPremade { get; set; }

    [Column("is_public")]
    public bool IsPublic { get; set; }

    [Column("owner")]
    public string Owner { get; set; } = string.Empty;

    [Column("device_id")]
    public string DeviceId { get; set; } = string.Empty;

    // Columns may not contain the string "json".
    [Column("segments")]
    public List<SongSegmentDto> Segments { get; set; } = [];

    [Column("length_in_seconds")]
    public long LengthInSeconds { get; set; }

    [Column("count")]
    public int Count { get; set; }
}