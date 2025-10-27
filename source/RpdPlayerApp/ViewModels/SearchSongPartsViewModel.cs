using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RpdPlayerApp.Architecture;
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
                await AddToQueue(segment);
                break;
            case "Add to playlist":
                await AddToPlaylist(segment);
                break;
        }
    }

    private async Task FavoriteSegment(SongPart p)
    {
        bool success = await PlaylistsManager.TryAddSongPartToLocalPlaylist(Constants.FAVORITES, p!);
        if (success)
        {
            General.ShowAlert("Favorite", $"Added {p.Title} to Favorites");
        }
        else
        {
            General.ShowAlert("Favorite", $"Failed to add {p.Title} to Favorites");
        }
    }
    private Task AddToQueue(SongPart p) => Shell.Current.DisplayAlert("WIP", "TODO", "OK");
    private Task AddToPlaylist(SongPart p) => Shell.Current.DisplayAlert("WIP", $"TODO", "OK");
}
