using RpdPlayerApp.Enums;

namespace RpdPlayerApp.DTO;

internal class LogDto
{
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public LogCategoryValue Category { get; set; }
    public string Message { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
    public string DeviceId { get; set; } = string.Empty;
}
