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

    internal byte[] GroupTypes { get; set; } = [];
    internal string[] Genre { get; set; } = [];


}
