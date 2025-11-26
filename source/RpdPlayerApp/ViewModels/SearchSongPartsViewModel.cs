using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RpdPlayerApp.Architecture;
using RpdPlayerApp.Enums;
using RpdPlayerApp.Managers;
using RpdPlayerApp.Models;

namespace RpdPlayerApp.ViewModels;

internal partial class SearchSongPartsViewModel : ObservableObject
{
    public IRelayCommand<SongPart> ShowActionsCommand { get; }

    public SearchSongPartsViewModel()
    {
        ShowActionsCommand = new RelayCommand<SongPart>(ShowActions);
    }

    private async void ShowActions(SongPart? segment)
    {
        string action = await Shell.Current.DisplayActionSheet(
            title: $"Actions for {segment!.Title}",
            cancel: "Cancel",
            destruction: null,
            "Favorite",
            "Add to queue",
            "Add to playlist"
        );

        switch (action)
        {
            case "Favorite":
                await FavoriteSegment(segment);
                break;
            case "Add to queue":
                AddToQueue(segment);
                break;
            case "Add to playlist":
                await AddToPlaylist(segment);
                break;
        }
    }

    private async Task FavoriteSegment(SongPart s)
    {
        bool success = await PlaylistsManager.TryAddSongPartToLocalPlaylist(Constants.FAVORITES, s!);
        string message = success ? $"Added {s.Title} to Favorites" : $"Failed to add {s.Title} to Favorites";
        General.ShowToast(message);
    }
    private void AddToQueue(SongPart songPart)
    {
        // TODO: METHOD
        if (!AppState.SongPartsQueue.Contains(songPart))
        {
            AppState.SongPartsQueue.Enqueue(songPart);
            General.ShowToast($"Enqueued: {songPart.ArtistName} - {songPart.Title} {songPart.PartNameFull}");
        }

        // Change mode to queue list
        AppState.PlayMode = PlayModeValue.Queue;

        AudioManager.AddedToQueue();
    }

    // TODO: Make a full playlist (local + cloud) and show lists to choose.
    private Task AddToPlaylist(SongPart p) => Shell.Current.DisplayAlert("WIP", $"TODO", "OK");
}
