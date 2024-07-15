using System.Collections.ObjectModel;

namespace RpdPlayerApp.Models;

internal class Playlist
{
    public string Name { get; set; }
    public string LocalPath { get; set; }
    public int Count {  get; set; }
    public TimeSpan Length { get; set; }

    public ObservableCollection<SongPart> SongParts;

    public Playlist(string name = "", string path = "", int count = 0)
    {
        Name = name;
        LocalPath = path;
        Count = count;
    }
}
