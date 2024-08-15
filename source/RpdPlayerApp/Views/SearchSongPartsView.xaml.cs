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
    internal event EventHandler? PlaySongPart;
    internal event EventHandler? AddSongPart;
    internal event EventHandler? SortPressed;

    internal ObservableCollection<SongPart> allSongParts;
    internal ObservableCollection<SongPart> songParts = new ObservableCollection<SongPart>();
    internal int SongCount { get; set; } = 0;

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

    internal void SongPartsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        ResultsLabel.Text = $"Currently showing: {songParts.Count} results";
    }

    internal void RefreshSongParts()
    {
        SonglibraryListView.ItemsSource = songParts;
    }

    private void SwipeItemPlaySongPart(object sender, EventArgs e)
    {
        if (!HelperClass.HasInternetConnection())
            return;

        SongPart songPart = (SongPart)((MenuItem)sender).CommandParameter;
        if (songPart.AudioURL != string.Empty)
        {
            MainViewModel.CurrentSongPart = songPart;
            PlaySongPart.Invoke(sender, e);
        }
    }

    private void PlayRandomButton_Clicked(object sender, EventArgs e)
    {
        if (!HelperClass.HasInternetConnection())
            return;

        var random = new Random();
        int index = random.Next(songParts.Count);
        SongPart songPart = songParts[index];

        MainViewModel.CurrentSongPart = songPart;
        PlaySongPart.Invoke(sender, e);
    }

    private void AddResultsButton_Clicked(object sender, EventArgs e)
    {
        if (PlaylistManager.Instance.CurrentPlaylist is null)
        {
            CommunityToolkit.Maui.Alerts.Toast.Make($"Select a playlist first!", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
            return;
        }

        int addedSongParts = PlaylistManager.Instance.AddSongPartsToCurrentPlaylist(songParts.ToList());
        CommunityToolkit.Maui.Alerts.Toast.Make($"{addedSongParts} songs added!", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
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

    private void SwipeItemAddSong(object sender, EventArgs e)
    {
        SongPart songPart = (SongPart)((MenuItem)sender).CommandParameter;

        bool added = PlaylistManager.Instance.AddSongPartToCurrentPlaylist(songPart);

        if (added)
        {
            AddSongPart.Invoke(sender, e);
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
        MainViewModel.PlayMode = PlayMode.Queue;
    }

    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        songParts.Clear();
        var list = allSongParts.Where(s => s.ArtistName.ToLower().Contains(e.NewTextValue.ToLower()) ||
                                            s.Artist.AltName.ToLower().Contains(e.NewTextValue.ToLower()) ||
                                            s.Title.ToLower().Contains(e.NewTextValue.ToLower()))
                                            .ToList();
        foreach (var item in list)
        {
            AddSongPart.Invoke(sender, e);
            songParts.Add(item);
        }
        SonglibraryListView.ItemsSource = songParts;
    }

    private void SortButton_Clicked(object sender, EventArgs e)
    {
        SortPressed.Invoke(sender, e);
    }

    internal void SetSearchFilterMode()
    {
        songParts.CollectionChanged -= SongPartsCollectionChanged;
        switch (MainViewModel.SearchFilterMode)
        {
            case SearchFilterMode.None: songParts = allSongParts; break;
            case SearchFilterMode.Male: songParts = allSongParts.Where(s => s.Artist?.GroupType == GroupType.BG).ToObservableCollection(); break;
            case SearchFilterMode.Female: songParts = allSongParts.Where(s => s.Artist?.GroupType == GroupType.GG).ToObservableCollection(); break;
            case SearchFilterMode.YG: songParts = allSongParts.Where(s => s.Artist?.Company == "YG Entertainment").ToObservableCollection(); break;
            case SearchFilterMode.JYP: songParts = allSongParts.Where(s => s.Artist?.Company == "JYP Entertainment").ToObservableCollection(); break;
            case SearchFilterMode.SM: songParts = allSongParts.Where(s => s.Artist?.Company == "SM Entertainment").ToObservableCollection(); break;
            case SearchFilterMode.Firstgen: songParts = allSongParts.Where(s => s.Album?.ReleaseDate < HelperClass.firstGenEndDate).ToObservableCollection(); break;
            case SearchFilterMode.Secondgen: songParts = allSongParts.Where(s => s.Album?.ReleaseDate < HelperClass.firstGenEndDate).ToObservableCollection(); break;
            case SearchFilterMode.Thirdgen: songParts = allSongParts.Where(s => s.Album?.ReleaseDate < HelperClass.firstGenEndDate).ToObservableCollection(); break;
            case SearchFilterMode.Fourthgen: songParts = allSongParts.Where(s => s.Album?.ReleaseDate < HelperClass.firstGenEndDate).ToObservableCollection(); break;
            case SearchFilterMode.Fifthgen: songParts = allSongParts.Where(s => s.Album?.ReleaseDate > HelperClass.fifthGenStartDate).ToObservableCollection(); break;
            case SearchFilterMode.KR: songParts = allSongParts.Where(s => s.Album?.Language == "KR").ToObservableCollection(); break;
            case SearchFilterMode.JP: songParts = allSongParts.Where(s => s.Album?.Language == "JP").ToObservableCollection(); break;
            case SearchFilterMode.EN: songParts = allSongParts.Where(s => s.Album?.Language == "EN").ToObservableCollection(); break;
        }
        songParts.CollectionChanged += SongPartsCollectionChanged;
        SonglibraryListView.ItemsSource = songParts;

        ResultsLabel.Text = $"Currently showing: {songParts.Count} results";
    }

    internal void RefreshSort()
    {
        songParts.CollectionChanged -= SongPartsCollectionChanged;
        switch (MainViewModel.SortMode)
        {
            case SortMode.ReleaseDate:
                songParts = songParts.OrderBy(s => s.Album?.ReleaseDate).ToObservableCollection();
                songParts.ToList().ForEach(s => s.Album!.ShowAlbumReleaseDate = true);
                songParts.ToList().ForEach(s => s.Artist!.ShowGroupType = false);
                songParts.ToList().ForEach(s => s.ShowClipLength = false);
                break;

            case SortMode.Artist:
                songParts.CollectionChanged -= SongPartsCollectionChanged;
                songParts = songParts.OrderBy(s => s.ArtistName).ToObservableCollection();
                songParts.ToList().ForEach(s => s.Album!.ShowAlbumReleaseDate = false);
                songParts.ToList().ForEach(s => s.Artist!.ShowGroupType = false);
                songParts.ToList().ForEach(s => s.ShowClipLength = false);
                break;

            case SortMode.Title:
                songParts.CollectionChanged -= SongPartsCollectionChanged;
                songParts = songParts.OrderBy(s => s.Title).ToObservableCollection();
                songParts.ToList().ForEach(s => s.Album!.ShowAlbumReleaseDate = false);
                songParts.ToList().ForEach(s => s.Artist!.ShowGroupType = false);
                songParts.ToList().ForEach(s => s.ShowClipLength = false);
                break;

            case SortMode.GroupType:
                songParts.CollectionChanged -= SongPartsCollectionChanged;
                songParts = songParts.OrderBy(s => s.Artist?.GroupType).ToObservableCollection();
                songParts.ToList().ForEach(s => s.Album!.ShowAlbumReleaseDate = false);
                songParts.ToList().ForEach(s => s.Artist!.ShowGroupType = true);
                songParts.ToList().ForEach(s => s.ShowClipLength = false);
                break;

            case SortMode.SongPart:
                songParts.CollectionChanged -= SongPartsCollectionChanged;
                songParts = songParts.OrderBy(s => s.PartNameFull).ToObservableCollection();
                songParts.ToList().ForEach(s => s.Album!.ShowAlbumReleaseDate = false);
                songParts.ToList().ForEach(s => s.Artist!.ShowGroupType = false);
                songParts.ToList().ForEach(s => s.ShowClipLength = false);

                break;

            case SortMode.ClipLength:
                songParts = songParts.OrderBy(s => s.ClipLength).ToObservableCollection();
                songParts.ToList().ForEach(s => s.Album!.ShowAlbumReleaseDate = false);
                songParts.ToList().ForEach(s => s.Artist!.ShowGroupType = false);
                songParts.ToList().ForEach(s => s.ShowClipLength = true);
                break;
        }

        songParts.CollectionChanged += SongPartsCollectionChanged;
        SonglibraryListView.ItemsSource = songParts;
    }
}