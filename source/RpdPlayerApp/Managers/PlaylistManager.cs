using RpdPlayerApp.Models;
using System.Collections.ObjectModel;

namespace RpdPlayerApp.Managers;

internal class PlaylistManager : BindableObject
{
    private static readonly PlaylistManager instance = new();

    private Playlist currentPlaylist = new();
    public Playlist CurrentPlaylist
    {
        get => currentPlaylist;
        set
        {
            currentPlaylist = value;
            OnPropertyChanged(nameof(CurrentPlaylist));
        }
    }

    static PlaylistManager()
    {
    }

    private PlaylistManager() => CurrentPlaylist = new Playlist();

    public static PlaylistManager Instance => instance;

    internal bool AddSongPartToCurrentPlaylist(SongPart songPart)
    {
        bool hasToAdd = false;

        if (currentPlaylist.SongParts is not null)
        {
            hasToAdd = !currentPlaylist.SongParts.Contains(songPart);

            if (hasToAdd)
            {
                currentPlaylist.SongParts.Add(songPart);
                currentPlaylist.SetCount();
                currentPlaylist.SetLength();
            }
        }

        return hasToAdd;
    }

    internal int AddSongPartsToCurrentPlaylist(List<SongPart> songParts)
    {
        int songPartsAdded = 0;

        if (currentPlaylist.SongParts is null)
        {
            currentPlaylist.SongParts = new ObservableCollection<SongPart>();
        }

        foreach (SongPart songPart in songParts)
        {
            if (!currentPlaylist.SongParts.Contains(songPart))
            {
                currentPlaylist.SongParts.Add(songPart);
                songPartsAdded++;
            }
        }
        return songPartsAdded;
    }

    internal void RemoveSongpartOfCurrentPlaylist(SongPart songpart)
    {
        var songpartToRemove = currentPlaylist.SongParts.FirstOrDefault(x => x.AudioURL == songpart.AudioURL);
        if (songpartToRemove is not null)
        {
            currentPlaylist.SongParts.Remove(songpartToRemove);
        }
    }

    internal void ClearCurrentPlaylist() => currentPlaylist.SongParts.Clear();
}
