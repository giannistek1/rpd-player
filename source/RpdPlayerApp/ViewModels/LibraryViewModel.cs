using CommunityToolkit.Mvvm.Input;
using RpdPlayerApp.Architecture;
using RpdPlayerApp.Managers;
using RpdPlayerApp.Models;

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
            title: $"Actions for {playlist!.Name}",
            cancel: "Cancel",
            destruction: null,
            "Clone",
            "Make public",
            "Save to cloud",
            "Share",
            "Delete"
        );

        switch (action)
        {
            case "Clone":
                await Todo(playlist);
                break;
            case "Edit":
                await Todo(playlist);
                break;
            case "Make public":
                await Todo(playlist);
                break;
            case "Save to cloud":
                await Todo(playlist);
                break;
            case "Share":
                await Todo(playlist);
                break;
            case "Delete":
                await Todo(playlist);
                break;
        }
    }

    private Task Todo(Playlist p) => Shell.Current.DisplayAlert("WIP", "TODO", "OK");
}