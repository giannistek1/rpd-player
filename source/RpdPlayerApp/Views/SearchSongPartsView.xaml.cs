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
            PlaySongPart?.Invoke(sender, e);
        }
    }

    private void SonglibraryListViewItemTapped(object sender, Syncfusion.Maui.ListView.ItemTappedEventArgs e)
    {
        if (!HelperClass.HasInternetConnection())
            return;

        if (e.ItemType != Syncfusion.Maui.ListView.ItemType.Record)
            return;

        SongPart songPart = (SongPart)e.DataItem;

        if (songPart.AudioURL != string.Empty)
        {
            // Mode to queue/single song
            MainViewModel.PlayMode = PlayMode.Queue;

            MainViewModel.CurrentSongPart = songPart;
            PlaySongPart?.Invoke(sender, e);
        }

        SonglibraryListView.SelectedItems?.Clear();
    }
    private void SonglibraryListViewSwipeEnded(object sender, Syncfusion.Maui.ListView.SwipeEndedEventArgs e)
    {
        // Swipe left to right (start)
        if (e.Direction == SwipeDirection.Right && e.Offset > 30)
        {
            SongPart songPart = (SongPart)e.DataItem;

            if (!MainViewModel.SongPartsQueue.Contains(songPart))
            {
                MainViewModel.SongPartsQueue.Enqueue(songPart);
                CommunityToolkit.Maui.Alerts.Toast.Make($"Enqueued: {songPart.ArtistName} - {songPart.Title} {songPart.PartNameFull}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
            }

            // Change mode to queue list
            MainViewModel.PlayMode = PlayMode.Queue;

        }

        // Swipe right to left (end)
        else if (e.Direction == SwipeDirection.Left && e.Offset > -30)
        {
            SongPart songPart = (SongPart)e.DataItem;

            bool added = PlaylistManager.Instance.AddSongPartToCurrentPlaylist(songPart);

            if (added)
            {
                AddSongPart?.Invoke(sender, e);
                CommunityToolkit.Maui.Alerts.Toast.Make($"Added: {songPart.ArtistName} - {songPart.Title} {songPart.PartNameFull}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
            }

        }
    }
    private void SwipeItemAddSong(object sender, EventArgs e)
    {
        SongPart songPart = (SongPart)((MenuItem)sender).CommandParameter;

        bool added = PlaylistManager.Instance.AddSongPartToCurrentPlaylist(songPart);

        if (added)
        {
            AddSongPart?.Invoke(sender, e);
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

    private void PlayRandomButton_Clicked(object sender, EventArgs e)
    {
        if (!HelperClass.HasInternetConnection())
            return;

        var random = new Random();
        int index = random.Next(songParts.Count);
        SongPart songPart = songParts[index];

        MainViewModel.CurrentSongPart = songPart;
        PlaySongPart?.Invoke(sender, e);
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

    private void CollapseAllButtonClicked(object sender, EventArgs e)
    {
        SonglibraryListView.CollapseAll();
    }

    private void ExpandAllButtonClicked(object sender, EventArgs e)
    {
        SonglibraryListView.ExpandAll();
    }

    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        // Set filter if set
        SetSearchFilterMode();

        List<SongPart> list = songParts.Where(s => s.ArtistName.ToLower().Contains(e.NewTextValue.ToLower()) ||
                                            s.Artist.AltName.ToLower().Contains(e.NewTextValue.ToLower()) ||
                                            s.Title.ToLower().Contains(e.NewTextValue.ToLower()))
                                            .ToList();

        SonglibraryListView.ItemsSource = list;
        ResultsLabel.Text = $"Currently showing: {list.Count} results";
    }

    private void SortButton_Clicked(object sender, EventArgs e)
    {
        SortPressed?.Invoke(sender, e);
    }

    #region Filter
    internal void SetSearchFilterMode()
    {
        songParts.CollectionChanged -= SongPartsCollectionChanged;
        switch (MainViewModel.SearchFilterMode)
        {
            case SearchFilterMode.None: FilterLabel.Text = "All songs"; 
                songParts = allSongParts; break;

            case SearchFilterMode.Male: FilterLabel.Text = "Boy(group)"; 
                songParts = allSongParts.Where(s => s.Artist?.GroupType == GroupType.BG).ToObservableCollection(); break;
            case SearchFilterMode.Female: FilterLabel.Text = "Girl(group)"; 
                songParts = allSongParts.Where(s => s.Artist?.GroupType == GroupType.GG).ToObservableCollection(); break;

            case SearchFilterMode.Hybe: FilterLabel.Text = "Hybe Label"; 
                songParts = allSongParts.Where(s => s.Artist?.Company == "HYBE Labels" || s.Artist?.Company == "Big Hit Entertainment").ToObservableCollection(); break;
            case SearchFilterMode.YG: FilterLabel.Text = "YG Ent."; 
                songParts = allSongParts.Where(s => s.Artist?.Company == "YG Entertainment").ToObservableCollection(); break;
            case SearchFilterMode.JYP: FilterLabel.Text = "JYP Ent."; 
                songParts = allSongParts.Where(s => s.Artist?.Company == "JYP Entertainment").ToObservableCollection(); break;
            case SearchFilterMode.SM: FilterLabel.Text = "SM Ent."; 
                songParts = allSongParts.Where(s => s.Artist?.Company == "SM Entertainment").ToObservableCollection(); break;
            case SearchFilterMode.Cube: FilterLabel.Text = "Cube Ent."; 
                songParts = allSongParts.Where(s => s.Artist?.Company == "Cube Entertainment").ToObservableCollection(); break;
            case SearchFilterMode.FNC: FilterLabel.Text = "FNC Ent."; 
                songParts = allSongParts.Where(s => s.Artist?.Company == "FNC Entertainment").ToObservableCollection(); break;
            case SearchFilterMode.Pledis: FilterLabel.Text = "Pledis Ent."; 
                songParts = allSongParts.Where(s => s.Artist?.Company == "Pledis Entertainment").ToObservableCollection(); break;
            case SearchFilterMode.Starship: FilterLabel.Text = "Starship Ent."; 
                songParts = allSongParts.Where(s => s.Artist?.Company == "Starship Entertainment").ToObservableCollection(); break;

            case SearchFilterMode.Firstgen: FilterLabel.Text = "First gen (< 2002)"; 
                songParts = allSongParts.Where(s => s.Artist?.DebutDate < HelperClass.secondGenStartDate && s.Album?.Language == "KR").ToObservableCollection(); break;
            case SearchFilterMode.Secondgen: FilterLabel.Text = "Second gen (2003 - 2011)"; 
                songParts = allSongParts.Where(s => s.Artist?.DebutDate > HelperClass.secondGenStartDate && s.Artist?.DebutDate < HelperClass.thirdGenStartDate).ToObservableCollection(); break;
            case SearchFilterMode.Thirdgen: FilterLabel.Text = "Third gen (2012 - 2017)"; 
                songParts = allSongParts.Where(s => s.Artist?.DebutDate > HelperClass.thirdGenStartDate && s.Artist?.DebutDate < HelperClass.fourthGenStartDate).ToObservableCollection(); break;
            case SearchFilterMode.Fourthgen: FilterLabel.Text = "Fourth gen (2018 - 2022)"; 
                songParts = allSongParts.Where(s => s.Artist?.DebutDate > HelperClass.fourthGenStartDate && s.Artist?.DebutDate < HelperClass.fifthGenStartDate).ToObservableCollection(); break;
            case SearchFilterMode.Fifthgen: FilterLabel.Text = "Fift gen (2023 >)"; 
                songParts = allSongParts.Where(s => s.Artist?.DebutDate > HelperClass.fifthGenStartDate).ToObservableCollection(); break;
            
            case SearchFilterMode.KR: FilterLabel.Text = "Korean"; 
                songParts = allSongParts.Where(s => s.Album?.Language == "KR").ToObservableCollection(); break;
            case SearchFilterMode.JP: FilterLabel.Text = "Japanese"; 
                songParts = allSongParts.Where(s => s.Album?.Language == "JP").ToObservableCollection(); break;
            case SearchFilterMode.EN: FilterLabel.Text = "English"; 
                songParts = allSongParts.Where(s => s.Album?.Language == "EN").ToObservableCollection(); break;
            case SearchFilterMode.TH: FilterLabel.Text = "Thai"; 
                songParts = allSongParts.Where(s => s.Album?.Language == "TH").ToObservableCollection(); break;
            
            case SearchFilterMode.Solo: FilterLabel.Text = "Solo artists"; 
                songParts = allSongParts.Where(s => s.Artist?.MemberCount == 1).ToObservableCollection(); break;
            case SearchFilterMode.Duo: FilterLabel.Text = "Duos"; 
                songParts = allSongParts.Where(s => s.Artist?.MemberCount == 2).ToObservableCollection(); break;
            case SearchFilterMode.Trio: FilterLabel.Text = "Trios"; 
                songParts = allSongParts.Where(s => s.Artist?.MemberCount == 3).ToObservableCollection(); break;
            case SearchFilterMode.Quadruplet: FilterLabel.Text = "Quadruplets"; 
                songParts = allSongParts.Where(s => s.Artist?.MemberCount == 4).ToObservableCollection(); break;
            case SearchFilterMode.Quintet: FilterLabel.Text = "Quintets"; 
                songParts = allSongParts.Where(s => s.Artist?.MemberCount == 5).ToObservableCollection(); break;
            case SearchFilterMode.Sextet: FilterLabel.Text = "Sextets"; 
                songParts = allSongParts.Where(s => s.Artist?.MemberCount == 6).ToObservableCollection(); break;
            case SearchFilterMode.Septet: FilterLabel.Text = "Septets"; 
                songParts = allSongParts.Where(s => s.Artist?.MemberCount == 7).ToObservableCollection(); break;
            case SearchFilterMode.Octet: FilterLabel.Text = "Octets"; 
                songParts = allSongParts.Where(s => s.Artist?.MemberCount == 8).ToObservableCollection(); break;
            case SearchFilterMode.Nonet: FilterLabel.Text = "Nonets"; 
                songParts = allSongParts.Where(s => s.Artist?.MemberCount == 9).ToObservableCollection(); break;
            case SearchFilterMode.Group: FilterLabel.Text = "Groups (2+ members)"; 
                songParts = allSongParts.Where(s => s.Artist?.MemberCount > 2).ToObservableCollection(); break;
        }
        songParts.CollectionChanged += SongPartsCollectionChanged;
        SonglibraryListView.ItemsSource = songParts;

        ResultsLabel.Text = $"Currently showing: {songParts.Count} results";
    }
    #endregion

    #region Sort
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

            case SortMode.Count:
                songParts = songParts.OrderByDescending(s => s.Artist?.Count).ToObservableCollection();
                songParts.ToList().ForEach(s => s.Album!.ShowAlbumReleaseDate = false);
                songParts.ToList().ForEach(s => s.Artist!.ShowGroupType = false);
                songParts.ToList().ForEach(s => s.ShowClipLength = false);
                break;
        }

        songParts.CollectionChanged += SongPartsCollectionChanged;
        SonglibraryListView.ItemsSource = songParts;
    }

    #endregion
}