using System.Collections.ObjectModel;

namespace RpdPlayerApp.Models;

internal class PlaylistManager : BindableObject
{
    private static readonly PlaylistManager instance = new PlaylistManager();

    private ObservableCollection<SongPart> currentPlaylist;
    public ObservableCollection<SongPart> CurrentPlaylist
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

    private PlaylistManager()
    {
        CurrentPlaylist = new ObservableCollection<SongPart>();
    }

    public static PlaylistManager Instance
    {
        get
        {
            return instance;
        }
    }

    internal bool AddSongPartToCurrentPlaylist(SongPart songPart)
    {
        bool hasToAdd = !currentPlaylist.Contains(songPart);

        if (hasToAdd)
        {
            currentPlaylist.Add(songPart);
        }

        return hasToAdd;
    }

    internal int AddSongPartsToCurrentPlaylist(List<SongPart> songParts)
    {
        int songPartsAdded = 0;

        foreach (SongPart songPart in songParts)
        {
            if (!currentPlaylist.Contains(songPart))
            {
                currentPlaylist.Add(songPart);
                songPartsAdded++;
            }
        }
        return songPartsAdded;
    }

    internal void RemoveSongpartOfCurrentPlaylist(SongPart songpart)
    {
        var songpartToRemove = currentPlaylist.SingleOrDefault(x => x.Id == songpart.Id);
        if (songpartToRemove != null)
            currentPlaylist.Remove(songpartToRemove);
    }

    internal void ClearCurrentPlaylist()
    {
        currentPlaylist.Clear();
    }

    internal int GetCurrentPlaylistSongCount()
    {
        return currentPlaylist.Count;
    }
}
