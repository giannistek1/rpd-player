using RpdPlayerApp.Enums;
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
    public CountdownModeValue CountdownMode { get; set; } = CountdownModeValue.Off; // TODO: Implement

    public ObservableCollection<SongPart> SongParts = [];

    public Playlist(DateTime creationDate, string name = "", string path = "", int count = 0)
    {
        CreationDate = creationDate;
        Name = name;
        LocalPath = path;
        Count = count;
    }

    // TODO: In property
    public void SetCount()
    {
        if (SongParts is not null)
        {
            Count = SongParts.Count;
        }
    }

    // TODO: In property
    public bool SetLength()
    {
        double lengthDouble = SongParts.Sum(t => t.ClipLength);
        Length = TimeSpan.FromSeconds(lengthDouble);

        return lengthDouble > 0;
    }
}
