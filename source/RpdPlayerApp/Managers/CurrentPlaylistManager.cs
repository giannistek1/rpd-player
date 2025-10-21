using CommunityToolkit.Mvvm.ComponentModel;
using RpdPlayerApp.Models;

namespace RpdPlayerApp.Managers;

internal partial class CurrentPlaylistManager : ObservableObject
{
    private static readonly CurrentPlaylistManager instance = new();

    /// <summary> Playlist being edited in CurrentPlaylistView. </summary>
    [ObservableProperty]
    private Playlist? _chosenPlaylist = null;

    /// <summary> Playlist being played. </summary>
    [ObservableProperty]
    private Playlist _currentlyPlayingPlaylist = new(creationDate: DateTime.Now, lastModifiedDate: DateTime.Now);

    private CurrentPlaylistManager() { }

    public static CurrentPlaylistManager Instance => instance;

    internal bool AddSongPartToCurrentPlaylist(SongPart songPart)
    {
        bool hasToAdd = false;

        if (ChosenPlaylist.Segments is not null)
        {
            hasToAdd = !ChosenPlaylist.Segments.Contains(songPart);

            if (hasToAdd)
            {
                ChosenPlaylist.Segments.Add(songPart);
            }
        }

        return hasToAdd;
    }

    internal int AddSongPartsToCurrentPlaylist(List<SongPart> songParts)
    {
        int songPartsAdded = 0;

        if (ChosenPlaylist!.Segments is null)
        {
            ChosenPlaylist.Segments = [];
        }

        foreach (SongPart songPart in songParts)
        {
            if (!ChosenPlaylist.Segments.Contains(songPart))
            {
                ChosenPlaylist.Segments.Add(songPart);
                songPartsAdded++;
            }
        }
        return songPartsAdded;
    }

    internal void RemoveSongpartOfCurrentPlaylist(SongPart songpart)
    {
        var songpartToRemove = ChosenPlaylist.Segments.FirstOrDefault(x => x.AudioURL == songpart.AudioURL);
        if (songpartToRemove is not null)
        {
            ChosenPlaylist.Segments.Remove(songpartToRemove);
        }
    }

    internal void ClearCurrentPlaylist() => ChosenPlaylist.Segments.Clear();
}
