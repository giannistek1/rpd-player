using RpdPlayerApp.Models;
using System.Collections.ObjectModel;

namespace RpdPlayerApp.Repository;

internal static class AlbumRepository
{
    public readonly static ObservableCollection<Album> Albums = new ObservableCollection<Album>();

    public static List<Album> GetAlbums()
    {
        // In the future get artist list from text file like
        // [I Love][2022-10-17][URL]

        if (Albums.Count == 0)
        {
            List<string> albumNames = SongPartRepository.SongParts.Select(s => s.AlbumName).Distinct().ToList();
            foreach (string name in albumNames)
            {
                Albums.Add(new Album(name: name, date: DateTime.Now));
            }
        }
        return Albums.ToList();
    }
}
