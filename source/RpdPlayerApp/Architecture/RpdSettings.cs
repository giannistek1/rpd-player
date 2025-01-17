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
    internal List<GroupType> GroupTypes { get; set; } = [];
    internal List<string> Genres { get; set; } = [];
    internal List<Gen> Gens { get; set; } = [];
    internal List<string> Companies { get; set; } = [];

    internal List<string> OtherCompanies { get; set; } = [];
    internal List<string> NumberedPartsBlacklist { get; set; } = []; 
    internal List<string> PartsBlacklist { get; set; } = [];
}
