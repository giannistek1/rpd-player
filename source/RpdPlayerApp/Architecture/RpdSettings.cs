using RpdPlayerApp.Models;

namespace RpdPlayerApp.Architecture;

class RpdSettings
{
    public RpdSettings()
    {
        
    }

    /// <summary>
    /// When false, mode is StartRpd.
    /// </summary>
    internal bool UsingGeneratePlaylist { get; set; } = false;
    internal TimeSpan Duration { get; set; } 
    internal List<string> GroupTypes { get; set; } = [];
    internal string[] Genres { get; set; } = [];

    internal Gen[] Gens { get; set; } = [];


}
