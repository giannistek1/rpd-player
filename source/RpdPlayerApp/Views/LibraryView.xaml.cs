using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Maui.Views;
using RpdPlayerApp.Models;
using RpdPlayerApp.Repositories;
using RpdPlayerApp.Repository;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;

namespace RpdPlayerApp.Views;

public partial class LibraryView : ContentPage
{
    IFileSaver fileSaver;
    public int SongCount { get; set; } = 0;
    public LibraryView(IFileSaver filesaver)
    {
        InitializeComponent();
        this.fileSaver = filesaver;

        PlaylistManager.Instance.CurrentPlaylist.CollectionChanged += CurrentPlaylistCollectionChanged;

        CurrentPlaylistListView.ItemsSource = PlaylistManager.Instance.CurrentPlaylist;
    }

    private void CurrentPlaylistCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        CountLabel.Text = PlaylistManager.Instance.GetCurrentPlaylistSongCount().ToString();
    }

    private void CurrentPlaylistListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        NetworkAccess accessType = Connectivity.Current.NetworkAccess;

        if (accessType != NetworkAccess.Internet)
        {
            Toast.Make($"No internet connection!", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
            return;
        }

        string url = ((SongPart)CurrentPlaylistListView.SelectedItem).AudioURL;
        if (url != string.Empty)
        {
            audioMediaElement.Source = MediaSource.FromUri(url);
            audioMediaElement.Play();
        }

        CurrentPlaylistListView.SelectedItem = null;
    }

    private void ClearButton_Clicked(object sender, EventArgs e)
    {
        PlaylistManager.Instance.ClearCurrentPlaylist();
    }

    private void SwipeItemPlaySongPart(object sender, EventArgs e)
    {
        NetworkAccess accessType = Connectivity.Current.NetworkAccess;

        if (accessType != NetworkAccess.Internet)
        {
            Toast.Make($"No internet connection!", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
            return;
        }

        string url = ((MenuItem)sender).CommandParameter.ToString();
        if (url != string.Empty)
        {
            audioMediaElement.Source = MediaSource.FromUri(url);
            audioMediaElement.Play();
        }
    }

    private void ContentPage_Disappearing(object sender, EventArgs e)
    {
        if (audioMediaElement.CurrentState == CommunityToolkit.Maui.Core.Primitives.MediaElementState.Playing)
            audioMediaElement.Stop();
    }

    private void PlayPlaylistButton_Clicked(object sender, EventArgs e)
    {
        Toast.Make("Not done yet");
    }

    private void SavePlaylistButton_Clicked(object sender, EventArgs e)
    {
        NetworkAccess accessType = Connectivity.Current.NetworkAccess;

        if (accessType != NetworkAccess.Internet)
        {
            Toast.Make($"No internet connection!", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
            return;
        }

        try
        {
            var path = FileSystem.Current.AppDataDirectory;
            var fullPath = Path.Combine(path, $"{PlaylistNameEntry.Text}.txt");

            StringBuilder sb = new StringBuilder();
            foreach (SongPart songPart in PlaylistManager.Instance.CurrentPlaylist)
            {
                sb.AppendLine($"{{{songPart.ArtistName}}}{{{songPart.AlbumTitle}}}{{{songPart.Title}}}{{{songPart.PartNameShort}}}{{{songPart.PartNameNumber}}}{{{songPart.AudioURL}}}");
            }

            File.WriteAllText(fullPath, sb.ToString());

            DropboxRepository.SavePlaylist(PlaylistNameEntry.Text);
            Toast.Make("Saved playlist!");
        }
        catch (Exception ex)
        {
            Toast.Make(ex.Message, CommunityToolkit.Maui.Core.ToastDuration.Short);
        }
    }

    private async void LoadPlaylistButton_Clicked(object sender, EventArgs e)
    {
        NetworkAccess accessType = Connectivity.Current.NetworkAccess;

        if (accessType != NetworkAccess.Internet)
        {
            await Toast.Make($"No internet connection!", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
            return;
        }

        try
        {
            CurrentPlaylistListView.ItemsSource = null;
            PlaylistManager.Instance.ClearCurrentPlaylist();

            var result = await DropboxRepository.LoadPlaylist(PlaylistNameEntry.Text);

            if (result.Contains("Error"))
            { 
                Toast.Make(result); 
                return; 
            }

            // Convert text to songParts
            var pattern = @"\{(.*?)\}";
            var matches = Regex.Matches(result, pattern);

            for (int i = 0; i < matches.Count / 6; i++)
            {
                int n = 6 * i; // songpart number

                string artistName = matches[n + 0].Groups[1].Value;
                string albumTitle = matches[n + 1].Groups[1].Value;

                SongPart songPart = new SongPart(id: i, artistName: artistName, albumTitle: albumTitle, title: matches[n + 2].Groups[1].Value, partNameShort: $"{matches[n + 3].Groups[1].Value}", partNameNumber: matches[n + 4].Groups[1].Value, audioURL: matches[n + 5].Groups[1].Value);
                songPart.Album = AlbumRepository.MatchAlbum(artistName, albumTitle);

                songPart.AlbumURL = songPart.Album is not null ? songPart.Album.ImageURL : string.Empty;
                PlaylistManager.Instance.AddSongPartToCurrentPlaylist(songPart);
            }

            CurrentPlaylistListView.ItemsSource = PlaylistManager.Instance.CurrentPlaylist;
        }
        catch (Exception ex)
        {
            Toast.Make(ex.Message, CommunityToolkit.Maui.Core.ToastDuration.Short);
        }
    }

    private void SwipeItemRemoveSongPart(object sender, EventArgs e)
    {
        SongPart songPart = (SongPart)((MenuItem)sender).CommandParameter;
        PlaylistManager.Instance.RemoveSongpartOfCurrentPlaylist(songPart);
    }
}