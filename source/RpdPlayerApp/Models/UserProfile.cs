using RpdPlayerApp.Architecture;

namespace RpdPlayerApp.Models;

internal class UserProfile
{
    public string Username { get; set; } = Constants.DEFAULT_USERNAME;
    public List<string> FavoriteGroups { get; set; } = new();
    public List<string> Genres { get; set; } = new();

    public void Reset()
    {
        Username = Constants.DEFAULT_USERNAME;
        FavoriteGroups.Clear();
        Genres.Clear();
    }
}
