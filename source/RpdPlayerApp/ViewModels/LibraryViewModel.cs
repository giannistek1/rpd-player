using CommunityToolkit.Mvvm.Input;
using RpdPlayerApp.Architecture;
using RpdPlayerApp.Enums;
using RpdPlayerApp.Managers;
using RpdPlayerApp.Models;
using System.Text;

namespace RpdPlayerApp.ViewModels;

internal class LibraryViewModel
{
    public IRelayCommand<Playlist> ShowActionsCommand { get; }

    public LibraryViewModel()
    {
        ShowActionsCommand = new RelayCommand<Playlist>(ShowActions);
    }

    private async void ShowActions(Playlist? playlist)
    {
        string action = await Shell.Current.DisplayActionSheet(
            title: $"Playlist {playlist!.Name}",
            cancel: "Cancel",
            destruction: null,
            "Play",
            "Clone",
            "Save to cloud",
            "Make public",
            "Share",
            "Delete"
        );

        switch (action)
        {
            case "Play":
                PlayPlaylist(playlist);
                break;
            case "Clone":
                await Todo(playlist);
                break;
            case "Edit":
                await Todo(playlist);
                break;
            case "Save to cloud":
                await Todo(playlist);
                break;
            case "Make public":
                await Todo(playlist);
                break;
            case "Share":
                await SharePlaylist(playlist);
                break;
            case "Delete":
                await DeletePlaylist(playlist);
                break;
        }
    }

    private Task Todo(Playlist playlist) => Shell.Current.DisplayAlert("WIP", "TODO", "OK");
    private void PlayPlaylist(Playlist playlist) => CurrentPlaylistManager.PlayPlaylist(playlist);
    private async Task DeletePlaylist(Playlist playlist)
    {
        PlaylistDeletedReturnValue returnValue = await PlaylistsManager.DeletePlaylist(playlist);
        switch (returnValue)
        {
            case PlaylistDeletedReturnValue.DeletedLocally:
                General.ShowToast("Playlist deleted locally.");
                break;
            case PlaylistDeletedReturnValue.DeletedFromCloud:
                General.ShowToast("Playlist deleted from cloud.");
                break;
            case PlaylistDeletedReturnValue.FailedToDelete:
                General.ShowToast("Failed to delete playlist.");
                break;
            case PlaylistDeletedReturnValue.CantDeletePublicPlaylist:
                General.ShowToast("Can't delete public playlist.");
                break;
        }
    }

    /// <summary>
    /// Shares a playlist as a list. TODO: Add timestamps and owner?.
    /// </summary>
    /// <param name="playlist"></param>
    /// <returns></returns>
    private async Task SharePlaylist(Playlist playlist)
    {
        try
        {
            StringBuilder sb = new();

            sb.AppendLine($"Check out my playlist: {playlist.Name}");

            foreach (SongPart segment in playlist.Segments)
            {
                sb.AppendLine($"{segment.Artist} - {segment.Title}");
            }

            await Share.RequestAsync(new ShareTextRequest
            {
                Title = $"Share {playlist.Name}", // No idea what this does, because you don't see it in the Android share dialog.
                Text = sb.ToString()
            });
        }
        catch (Exception ex)
        {
            General.ShowToast($"Error sharing playlist: {ex.Message}");
        }
    }
}