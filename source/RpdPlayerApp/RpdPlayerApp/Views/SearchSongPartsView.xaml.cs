using CommunityToolkit.Maui.Views;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using RpdPlayerApp.Models;
using RpdPlayerApp.Repository;
using CommunityToolkit.Maui.Core.Extensions;

namespace RpdPlayerApp.Views;

public partial class SearchSongPartsView : ContentPage
{
    ObservableCollection<SongPart> allSongParts;
    ObservableCollection<SongPart> songParts = new ObservableCollection<SongPart>();
    private bool customSort = false;
    public int SongCount { get; set; } = 0;

    public SearchSongPartsView()
    {
        InitializeComponent();

        songParts.CollectionChanged += SongPartsCollectionChanged;

        allSongParts = SongPartRepository.SongParts;

        int i = 0;
        foreach (var songPart in allSongParts)
        {
            songPart.Id = i;
            songParts.Add(songPart);
            i++;
        }
        SonglibraryListView.ItemsSource = songParts;
        SongCount = allSongParts.Count;

        ResultsLabel.Text = $"Currently showing: {songParts.Count} results";
    }

    private void SongPartsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        ResultsLabel.Text = $"Currently showing: {songParts.Count} results";
    }

    private void SonglibraryListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        bool added = PlaylistManager.Instance.AddSongPartToCurrentPlaylist((SongPart)SonglibraryListView.SelectedItem);
        if (added)
        {
            CommunityToolkit.Maui.Alerts.Toast.Make($"Added: {((SongPart)SonglibraryListView.SelectedItem).Title}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        }

        SonglibraryListView.SelectedItem = null;
    }

    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        songParts.Clear();
        var list = allSongParts.Where(s => s.ArtistName.ToLower().StartsWith(e.NewTextValue.ToLower())).ToList();
        foreach (var item in list)
        {
            songParts.Add(item);
        }
        SonglibraryListView.ItemsSource = songParts;
    }

    private void SwipeItemPlaySong(object sender, EventArgs e)
    {
        NetworkAccess accessType = Connectivity.Current.NetworkAccess;

        if (accessType != NetworkAccess.Internet)
        {
            CommunityToolkit.Maui.Alerts.Toast.Make($"No internet connection!", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
            return;
        }

        SongPart songpart = (SongPart)((MenuItem)sender).CommandParameter;
        if (songpart.AudioURL != string.Empty)
        {
            audioMediaElement.Source = MediaSource.FromUri(songpart.AudioURL);
            audioMediaElement.Play();
            CommunityToolkit.Maui.Alerts.Toast.Make($"Now playing: {songpart.Title}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        }
    }

    private void AddResultsButton_Clicked(object sender, EventArgs e)
    {
        int addedSongParts = PlaylistManager.Instance.AddSongPartsToCurrentPlaylist(songParts.ToList());
        CommunityToolkit.Maui.Alerts.Toast.Make($"{addedSongParts} songs added!", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
    }

    private void ContentPage_Disappearing(object sender, EventArgs e)
    {
        if (audioMediaElement.CurrentState == CommunityToolkit.Maui.Core.Primitives.MediaElementState.Playing)
            audioMediaElement.Stop();
    }

    private void PlayRandom_Clicked(object sender, EventArgs e)
    {
        NetworkAccess accessType = Connectivity.Current.NetworkAccess;

        if (accessType != NetworkAccess.Internet)
        {
            CommunityToolkit.Maui.Alerts.Toast.Make($"No internet connection!", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
            return;
        }

        var random = new Random();
        int index = random.Next(songParts.Count);

        audioMediaElement.Source = MediaSource.FromUri(songParts[index].AudioURL);
        audioMediaElement.Play();
        CommunityToolkit.Maui.Alerts.Toast.Make($"Now playing: {songParts[index].Title}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
    }

    private void SortButton_Clicked(object sender, EventArgs e)
    {
        if (!customSort)
        {
            songParts = songParts.OrderBy(s => s.Album?.ReleaseDate).ToObservableCollection();
            SonglibraryListView.ItemsSource = songParts;
            customSort = true;
        }
        else
        {
            songParts = songParts.OrderBy(s => s.ArtistName).ToObservableCollection();
            SonglibraryListView.ItemsSource = songParts;
            customSort = false;
        } 
    }
}