using RpdPlayerApp.Architecture;
using RpdPlayerApp.ViewModel;
using System.Collections.ObjectModel;

namespace RpdPlayerApp.Models;

internal class PlaylistManager : BindableObject
{
    private static readonly PlaylistManager instance = new();
    internal int CurrentSongPartIndex = -1;

    private Playlist currentPlaylist;
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

    private PlaylistManager()
    {
        CurrentPlaylist = new Playlist();//new ObservableCollection<SongPart>();
    }

    public static PlaylistManager Instance
    {
        get => instance;
    }

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
        var songpartToRemove = currentPlaylist.SongParts.SingleOrDefault(x => x.Id == songpart.Id);
        if (songpartToRemove != null)
            currentPlaylist.SongParts.Remove(songpartToRemove);
    }

    internal void ClearCurrentPlaylist() => currentPlaylist.SongParts.Clear();
    internal int GetCurrentPlaylistBoygroupCount() => currentPlaylist.SongParts.AsEnumerable().Count(s => s.Artist?.GroupType == GroupType.BG);
    internal int GetCurrentPlaylistGirlgroupCount() => currentPlaylist.SongParts.AsEnumerable().Count(s => s.Artist?.GroupType == GroupType.GG);

    internal void IncrementSongPartIndex()
    {
        if (CurrentPlaylist.Count == 0)
            return;

        if (CurrentSongPartIndex == CurrentPlaylist.Count - 1)
        {
            // Change mode to queue list
            MainViewModel.PlayMode = PlayMode.Queue;
            CurrentSongPartIndex = 0;
        }
        else
            CurrentSongPartIndex++;
    }
}
