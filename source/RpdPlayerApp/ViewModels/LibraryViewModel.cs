using CommunityToolkit.Mvvm.Input;
using RpdPlayerApp.Architecture;
using RpdPlayerApp.Enums;
using RpdPlayerApp.Managers;
using RpdPlayerApp.Models;
using RpdPlayerApp.Repositories;
using RpdPlayerApp.Services;
using RpdPlayerApp.Views;
using System.Text;

namespace RpdPlayerApp.ViewModels;

internal class LibraryViewModel
{
    public IRelayCommand<Playlist> ShowActionsCommand { get; }
    public IRelayCommand<Playlist> ShowPlaylistCommand { get; }
    public Action<Playlist>? OnShowPlaylist { get; set; }

    private Page? ParentPage => Shell.Current.CurrentPage;

    public LibraryViewModel()
    {
        ShowActionsCommand = new RelayCommand<Playlist>(ShowActions);

        ShowPlaylistCommand = new RelayCommand<Playlist>(playlist =>
        {
            CurrentPlaylistManager.Instance.ChosenPlaylist = playlist;
            OnShowPlaylist?.Invoke(playlist);
        });
    }

    private async void ShowActions(Playlist? playlist)
    {
        var actions = new List<string>
        {
            "Play",
            "Edit",
            //"Clone",
            "Share"
        };

        if (PlaylistsManager.PlaylistMode == PlaylistModeValue.Local)
        {
            actions.Insert(3, "Delete");
            actions.Insert(4, "Save to cloud");
        }
        else if (PlaylistsManager.PlaylistMode == PlaylistModeValue.Cloud)
        {
            actions.Insert(3, "Delete");
            actions.Insert(4, "Make public");
        }
        else if (PlaylistsManager.PlaylistMode == PlaylistModeValue.Public && playlist!.Owner.Equals(AppState.Username, StringComparison.OrdinalIgnoreCase)) // + Authenticated
        {
            actions.Insert(4, "Make private");
        }

        string action = await Shell.Current.DisplayActionSheet(
            title: $"Playlist {playlist!.Name} options",
            cancel: "Cancel",
            destruction: null,
            actions.ToArray()
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
                GoToCurrentPlaylistView(playlist);
                break;
            case "Save to cloud":
                await SaveToCloud(playlist);
                break;
            case "Make public":
                playlist.IsPublic = true;
                await UpdateCloudPlaylist(playlist);
                break;
            case "Make private":
                playlist.IsPublic = false;
                await UpdateCloudPlaylist(playlist);
                break;
            case "Share":
                await SharePlaylist(playlist);
                break;
            case "Delete":
                await DeletePlaylist(playlist);
                break;
        }
    }

    private Task Todo(Playlist playlist) => Shell.Current.DisplayAlert("Work in progress", "", "OK");

    private async Task SaveToCloud(Playlist playlist)
    {
        if (!CacheState.CloudPlaylists.Any(p => p.Name.Equals(playlist.Name, StringComparison.OrdinalIgnoreCase)))
        {
            CacheState.CloudPlaylists.Add(playlist);
        }

        try
        {
            await PlaylistRepository.SaveCloudPlaylistAsync(id: playlist.Id,
                                    creationDate: playlist.CreationDate,
                                    name: playlist.Name,
                                    playlist.LengthInSeconds,
                                    playlist.Count,
                                    playlist.Segments.ToList(),
                                    isPublic: playlist.IsPublic);

            General.ShowToast($"{playlist.Name} saved to cloud");
        }
        catch (Exception ex)
        {
            DebugService.Instance.Error($"LibraryViewModel: {ex.Message}");
            General.ShowToast($"Error saving playlist to cloud: {ex.Message}");
        }
    }

    private async Task UpdateCloudPlaylist(Playlist playlist)
    {
        try
        {
            await PlaylistRepository.SaveCloudPlaylistAsync(id: playlist.Id,
                                    creationDate: playlist.CreationDate,
                                    name: playlist.Name,
                                    playlist.LengthInSeconds,
                                    playlist.Count,
                                    playlist.Segments.ToList(),
                                    isPublic: playlist.IsPublic);

            General.ShowToast($"{playlist.Name} updated.");
        }
        catch (Exception ex)
        {
            DebugService.Instance.Error($"LibraryViewModel: {ex.Message}");
            General.ShowToast($"Error updating playlist to cloud: {ex.Message}");
        }
    }
    private void GoToCurrentPlaylistView(Playlist playlist)
    {
        CurrentPlaylistManager.Instance.ChosenPlaylist = playlist;
        if (Shell.Current.CurrentPage is MainPage mainPage)
        {
            mainPage.ShowPlaylistView();
        }
    }
    private void PlayPlaylist(Playlist playlist) => CurrentPlaylistManager.PlayPlaylist(playlist);
    private async Task DeletePlaylist(Playlist playlist)
    {
        bool accept = await ParentPage!.DisplayAlert("Confirmation", $"Delete {playlist.Name}?", "Yes", "No");
        if (accept)
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

    internal async Task CreateLocalPlaylist(string playlistName)
    {
        if (CacheState.LocalPlaylists.Any(p => p.Name.Equals(playlistName, StringComparison.OrdinalIgnoreCase)))
        {
            General.ShowToast("Already exists! Please choose different name.");
            return;
        }


        // HDR: Creation date | Modified date | Owner | Count | Length | Countdown mode
        string playlistHeader = $"HDR:[{DateTime.Today}][{DateTime.Today}][{AppState.Username}][0][{TimeSpan.Zero}][0]";

        string path = await FileManager.SavePlaylistStringToTextFileAsync(playlistName, playlistHeader);
        Playlist playlist = new(creationDate: DateTime.Today, lastModifiedDate: DateTime.Today, name: playlistName, path: path, owner: AppState.Username);

        CacheState.LocalPlaylists.Add(playlist);
    }

    internal async Task CreateEmptyCloudPlaylist(string playlistName)
    {
        if (CacheState.CloudPlaylists.Any(p => p.Name.Equals(playlistName, StringComparison.OrdinalIgnoreCase)))
        {
            General.ShowToast("Already exists! Please choose different name.");
            return;
        }

        Playlist playlist = new(creationDate: DateTime.Today, lastModifiedDate: DateTime.Today, name: playlistName, path: string.Empty, owner: AppState.Username);

        CacheState.CloudPlaylists.Add(playlist);

        await PlaylistRepository.SaveCloudPlaylistAsync(id: playlist.Id,
                                            creationDate: playlist.CreationDate,
                                            name: playlist.Name,
                                            playlist.LengthInSeconds,
                                            playlist.Count,
                                            playlist.Segments.ToList(),
                                            isPublic: playlist.IsPublic);
    }
}