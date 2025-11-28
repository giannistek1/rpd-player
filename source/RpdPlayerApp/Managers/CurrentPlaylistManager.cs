using CommunityToolkit.Mvvm.ComponentModel;
using RpdPlayerApp.Enums;
using RpdPlayerApp.Models;
using System.Collections.ObjectModel;

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

    public static void PlayCurrentPlaylist()
    {
        if (!instance.ChosenPlaylist!.Segments.Any()) { return; }

        AppState.PlayMode = PlayModeValue.Playlist;

        instance.CurrentlyPlayingPlaylist = instance.ChosenPlaylist;
        AudioManager.ChangeAndStartSong(instance.ChosenPlaylist.Segments[0]);
    }

    public static void PlayPlaylist(Playlist playlist)
    {
        if (!playlist.Segments.Any()) { return; }

        AppState.PlayMode = PlayModeValue.Playlist;

        instance.CurrentlyPlayingPlaylist = playlist;
        AudioManager.ChangeAndStartSong(playlist.Segments[0]);
    }

    internal bool AddSongPartToCurrentPlaylist(SongPart songPart)
    {
        bool hasToAdd = false;

        if (ChosenPlaylist!.Segments is not null)
        {
            hasToAdd = !ChosenPlaylist.Segments.Contains(songPart);

            if (hasToAdd)
            {
                ChosenPlaylist.Segments.Add(songPart);
                RecalculatePlaylistTimingsAndIndices(ref ChosenPlaylist.Segments);
            }
        }

        return hasToAdd;
    }

    internal int AddSongPartsToCurrentPlaylist(List<SongPart> segments)
    {
        int songPartsAdded = 0;

        if (ChosenPlaylist!.Segments is null)
        {
            ChosenPlaylist.Segments = [];
        }

        foreach (SongPart segment in segments)
        {
            if (!ChosenPlaylist.Segments.Contains(segment))
            {
                ChosenPlaylist.Segments.Add(segment);
                songPartsAdded++;
            }
        }

        RecalculatePlaylistTimingsAndIndices(ref ChosenPlaylist.Segments);

        return songPartsAdded;
    }

    internal void RecalculatePlaylistTimingsAndIndices(ref ObservableCollection<SongPart> segments)
    {
        double totalDurationInSeconds = 0;
        int index = 1;

        foreach (SongPart segment in segments)
        {
            segment.PlaylistStartTime = TimeSpan.FromSeconds(totalDurationInSeconds);
            segment.Id = index;

            totalDurationInSeconds += segment.ClipLength;
            index++;
        }
    }

    internal void RemoveSongpartOfCurrentPlaylist(SongPart songpart)
    {
        var songpartToRemove = ChosenPlaylist!.Segments.FirstOrDefault(x => x.AudioUrl == songpart.AudioUrl);
        if (songpartToRemove is not null)
        {
            ChosenPlaylist.Segments.Remove(songpartToRemove);
        }
    }

    internal void ClearCurrentPlaylist() => ChosenPlaylist!.Segments.Clear();
}
