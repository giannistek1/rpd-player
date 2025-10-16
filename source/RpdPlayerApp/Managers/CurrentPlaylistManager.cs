using CommunityToolkit.Mvvm.ComponentModel;
using RpdPlayerApp.Models;
using System.Collections.ObjectModel;

namespace RpdPlayerApp.Managers;

internal partial class CurrentPlaylistManager : ObservableObject
{
    private static readonly CurrentPlaylistManager instance = new();

    /// <summary> Playlist being edited in CurrentPlaylistView. </summary>
    [ObservableProperty]
    private Playlist _chosenPlaylist = new(creationDate: DateTime.Now);

    /// <summary> Playlist being played. </summary>
    [ObservableProperty]
    private Playlist _currentlyPlayingPlaylist = new(creationDate: DateTime.Now);

    static CurrentPlaylistManager() { }

    private CurrentPlaylistManager() => ChosenPlaylist = new Playlist(creationDate: DateTime.Now);

    public static CurrentPlaylistManager Instance => instance;

    internal bool AddSongPartToCurrentPlaylist(SongPart songPart)
    {
        bool hasToAdd = false;

        if (ChosenPlaylist.SongParts is not null)
        {
            hasToAdd = !ChosenPlaylist.SongParts.Contains(songPart);

            if (hasToAdd)
            {
                ChosenPlaylist.SongParts.Add(songPart);
                ChosenPlaylist.SetCount();
                ChosenPlaylist.SetLength();
            }
        }

        return hasToAdd;
    }

    internal int AddSongPartsToCurrentPlaylist(List<SongPart> songParts)
    {
        int songPartsAdded = 0;

        if (ChosenPlaylist.SongParts is null)
        {
            ChosenPlaylist.SongParts = new ObservableCollection<SongPart>();
        }

        foreach (SongPart songPart in songParts)
        {
            if (!ChosenPlaylist.SongParts.Contains(songPart))
            {
                ChosenPlaylist.SongParts.Add(songPart);
                songPartsAdded++;
            }
        }
        return songPartsAdded;
    }

    internal void RemoveSongpartOfCurrentPlaylist(SongPart songpart)
    {
        var songpartToRemove = ChosenPlaylist.SongParts.FirstOrDefault(x => x.AudioURL == songpart.AudioURL);
        if (songpartToRemove is not null)
        {
            ChosenPlaylist.SongParts.Remove(songpartToRemove);
        }
    }

    internal void ClearCurrentPlaylist() => ChosenPlaylist.SongParts.Clear();
}
