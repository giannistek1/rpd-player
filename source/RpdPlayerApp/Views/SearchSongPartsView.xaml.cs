using CommunityToolkit.Maui.Views;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using RpdPlayerApp.Models;
using RpdPlayerApp.Repository;
using CommunityToolkit.Maui.Core.Extensions;
using RpdPlayerApp.ViewModel;
using RpdPlayerApp.Architecture;

namespace RpdPlayerApp.Views;

public partial class SearchSongPartsView : ContentView
{
    public event EventHandler PlaySongPart;
    public event EventHandler SortPressed;

    ObservableCollection<SongPart> allSongParts;
    ObservableCollection<SongPart> songParts = new ObservableCollection<SongPart>();
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
        SongPart songPart = (SongPart)SonglibraryListView.SelectedItem;
        bool added = PlaylistManager.Instance.AddSongPartToCurrentPlaylist(songPart);
        if (added)
        {
            CommunityToolkit.Maui.Alerts.Toast.Make($"Added: {songPart.ArtistName} - {songPart.Title} {songPart.PartNameFull}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        }

        SonglibraryListView.SelectedItem = null;
    }

    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        songParts.Clear();
        var list = allSongParts.Where(s =>  s.ArtistName.ToLower().Contains(e.NewTextValue.ToLower()) || 
                                            s.Title.ToLower().Contains(e.NewTextValue.ToLower()) || 
                                            s.AlbumTitle.ToLower().Contains(e.NewTextValue.ToLower()))
                                            .ToList();
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

        SongPart songPart = (SongPart)((MenuItem)sender).CommandParameter;
        if (songPart.AudioURL != string.Empty)
        {
            MainViewModel.CurrentSongPart = songPart;
            PlaySongPart.Invoke(sender, e);
        }
    }

    private void AddResultsButton_Clicked(object sender, EventArgs e)
    {
        int addedSongParts = PlaylistManager.Instance.AddSongPartsToCurrentPlaylist(songParts.ToList());
        CommunityToolkit.Maui.Alerts.Toast.Make($"{addedSongParts} songs added!", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
    }

    private void PlayRandomButton_Clicked(object sender, EventArgs e)
    {
        NetworkAccess accessType = Connectivity.Current.NetworkAccess;

        if (accessType != NetworkAccess.Internet)
        {
            CommunityToolkit.Maui.Alerts.Toast.Make($"No internet connection!", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
            return;
        }

        var random = new Random();
        int index = random.Next(songParts.Count);
        SongPart songPart = songParts[index];

        MainViewModel.CurrentSongPart = songPart;
        PlaySongPart.Invoke(sender, e);
    }

    // Todo: Sort with UraniumUI bottom sheet
    private void SortButton_Clicked(object sender, EventArgs e)
    {
        SortPressed.Invoke(sender, e);
    }

    internal void RefreshSort()
    {
        switch (MainViewModel.SortMode)
        {
            case SortMode.ReleaseDate:
                songParts.CollectionChanged -= SongPartsCollectionChanged;
                songParts = songParts.OrderBy(s => s.Album?.ReleaseDate).ToObservableCollection();
                songParts.CollectionChanged += SongPartsCollectionChanged;
                SonglibraryListView.ItemsSource = songParts;
                break;
            case SortMode.Artist:
                songParts.CollectionChanged -= SongPartsCollectionChanged;
                songParts = songParts.OrderBy(s => s.ArtistName).ToObservableCollection();
                songParts.CollectionChanged += SongPartsCollectionChanged;
                SonglibraryListView.ItemsSource = songParts;
                break;
            case SortMode.Title:
                songParts.CollectionChanged -= SongPartsCollectionChanged;
                songParts = songParts.OrderBy(s => s.Title).ToObservableCollection();
                songParts.CollectionChanged += SongPartsCollectionChanged;
                SonglibraryListView.ItemsSource = songParts;
                break;
            case SortMode.GroupType:
                songParts.CollectionChanged -= SongPartsCollectionChanged;
                songParts = songParts.OrderBy(s => s.Artist?.GroupType).ToObservableCollection();
                songParts.CollectionChanged += SongPartsCollectionChanged;
                SonglibraryListView.ItemsSource = songParts;
                break;
            //case SortMode.ClipLength:
            //    songParts.CollectionChanged -= SongPartsCollectionChanged;
            //    songParts = songParts.OrderBy(s => s.AudioURL).ToObservableCollection();
            //    songParts.CollectionChanged += SongPartsCollectionChanged;
            //    SonglibraryListView.ItemsSource = songParts; 
            //    break;
            case SortMode.SongPart:
                songParts.CollectionChanged -= SongPartsCollectionChanged;
                songParts = songParts.OrderBy(s => s.PartNameFull).ToObservableCollection();
                songParts.CollectionChanged += SongPartsCollectionChanged;
                SonglibraryListView.ItemsSource = songParts;
                break;
        }
    }

    private void SwipeItemAddSong(object sender, EventArgs e)
    {
        SongPart songPart = (SongPart)((MenuItem)sender).CommandParameter;

        bool added = PlaylistManager.Instance.AddSongPartToCurrentPlaylist(songPart);

        if (added)
        {
            CommunityToolkit.Maui.Alerts.Toast.Make($"Added: {songPart.ArtistName} - {songPart.Title} {songPart.PartNameFull}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        }
    }

    private void SwipeItemQueueSong(object sender, EventArgs e)
    {
        SongPart songPart = (SongPart)((MenuItem)sender).CommandParameter;

        if (!MainViewModel.SongPartsQueue.Contains(songPart))
        {
            MainViewModel.SongPartsQueue.Enqueue(songPart);
            CommunityToolkit.Maui.Alerts.Toast.Make($"Enqueued: {songPart.ArtistName} - {songPart.Title} {songPart.PartNameFull}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        }

        // Change mode to queue list
        MainViewModel.IsPlayingPlaylist = false; 
    }
}