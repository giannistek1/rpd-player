using RpdPlayerApp.ViewModel;
using System.Collections.ObjectModel;

namespace RpdPlayerApp.Models;

internal class PlaylistManager : BindableObject
{
    private static readonly PlaylistManager instance = new();
    internal int CurrentSongPartIndex = -1;

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
        CurrentPlaylist = []; //new ObservableCollection<SongPart>();
    }

    public static PlaylistManager Instance
    {
        get => instance;
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

    internal void ClearCurrentPlaylist() => currentPlaylist.Clear();

    internal int GetCurrentPlaylistSongCount() => currentPlaylist.Count;
    internal int GetCurrentPlaylistBoygroupCount() => currentPlaylist.AsEnumerable().Count(s => s.Artist?.GroupType == GroupType.BG);
    internal int GetCurrentPlaylistGirlgroupCount() => currentPlaylist.AsEnumerable().Count(s => s.Artist?.GroupType == GroupType.GG);

    internal void IncrementSongPartIndex()
    {
        if (CurrentPlaylist.Count == 0)
            return;

        if (CurrentSongPartIndex == CurrentPlaylist.Count - 1)
        {
            // Change mode to queue list
            MainViewModel.IsPlayingPlaylist = false;
            CurrentSongPartIndex = 0;
        }
        else
            CurrentSongPartIndex++;
    }
}
