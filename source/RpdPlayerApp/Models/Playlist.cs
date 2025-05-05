using System.Collections.ObjectModel;

namespace RpdPlayerApp.Models;

internal class Playlist
{
    public string Name { get; set; }
    // Handy local variable,
    public string LocalPath { get; set; }
    public int Count { get; private set; }
    public DateTime CreationDate { get; set; }
    public DateTime LastModifiedDate { get; set; } = DateTime.MinValue;
    public int LengthInSeconds { get; set; } = 0;
    public TimeSpan Length { get; private set; }

    public ObservableCollection<SongPart> SongParts = [];

    public Playlist(DateTime creationDate, string name = "", string path = "", int count = 0)
    {
        CreationDate = creationDate;
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
        double lengthDouble = SongParts.Sum(t => t.ClipLength);
        Length = TimeSpan.FromSeconds(lengthDouble);

        return lengthDouble > 0;
    }
}
