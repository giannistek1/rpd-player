using System.ComponentModel.DataAnnotations.Schema;

namespace RpdPlayerApp.DTO;

public class SongSegmentDto
{
    [Column("id")]
    public int Id { get; set; }
    // No _ underscores in Json.
    [Column("albumName")]
    public string AlbumName { get; set; }
    [Column("title")]
    // No _ underscores in Json.
    public string Title { get; set; }
    [Column("artistName")]
    public string ArtistName { get; set; }
    [Column("audiourl")]
    public string AudioUrl { get; set; }
    [Column("cliplength")]
    public double ClipLength { get; set; }
    [Column("segmentShort")]
    public string SegmentShort { get; set; }
    [Column("segmentNumber")]
    public string SegmentNumber { get; set; }
}