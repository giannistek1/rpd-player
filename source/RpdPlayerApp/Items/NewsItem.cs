using Newtonsoft.Json;

namespace RpdPlayerApp.Items;

internal class NewsItem
{
    public string? Artist { get; set; } // For display.
    public string? Title { get; set; } // For display.
    public string? Part { get; set; } // For display.
    public string? AudioUrl { get; set; } // To compare difference (this is unique)
    public bool HasVideo { get; set; } // To compare difference (if there is a video now)
    [JsonIgnore]
    public bool HasNewVideo { get; set; } = false;
}
