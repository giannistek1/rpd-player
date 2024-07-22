using System.Collections.ObjectModel;

namespace RpdPlayerApp.Models;

internal class Playlist
{
    public string Name { get; set; }
    public string LocalPath { get; set; }
    public int Count { get; set; }
    public TimeSpan Length { get; set; }

    public ObservableCollection<SongPart> SongParts;

    public Playlist(string name = "", string path = "", int count = 0)
    {
        Name = name;
        LocalPath = path;
        Count = count;
    }

    public void SetCount() 
    { 
        if (SongParts is not null) 
        { 
            Count = SongParts.Count; 
        } 
    }

    public bool SetLength()
    {
        if (SongParts is not null)
        {
            double lengthDouble = SongParts.Sum(t => t.ClipLength);
            Length = TimeSpan.FromSeconds(lengthDouble);

            return lengthDouble > 0;
        }

        return false;
        
    }
}
