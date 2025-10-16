using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace RpdPlayerApp.DTO;

public class PlaylistDto
{
    [Column("id")]
    public int Id { get; set; }

    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Column("creation_date")]
    public DateTime CreationDate { get; set; }

    [Column("last_modified_date")]
    public DateTime LastModifiedDate { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }

    [Column("is_premade")]
    public bool IsPremade { get; set; }

    [Column("owner")]
    public string Owner { get; set; } = string.Empty;

    // Columns may not contain the string "json".
    [Column("segments")]
    public List<SongSegmentDto> Segments { get; set; } = [];

    [Column("length_in_seconds")]
    public long LengthInSeconds { get; set; }

    [Column("count")]
    public int Count { get; set; }
}