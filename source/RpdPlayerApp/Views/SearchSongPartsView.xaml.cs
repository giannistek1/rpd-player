using System.Collections.ObjectModel;
using System.Collections.Specialized;
using RpdPlayerApp.Models;
using RpdPlayerApp.Repositories;
using CommunityToolkit.Maui.Core.Extensions;
using RpdPlayerApp.ViewModels;
using RpdPlayerApp.Architecture;
using Syncfusion.Maui.DataSource;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Syncfusion.Maui.ListView.Helpers;
using Syncfusion.Maui.ListView;
using Syncfusion.Maui.GridCommon.ScrollAxis;
using RpdPlayerApp.Enums;
using RpdPlayerApp.Managers;

namespace RpdPlayerApp.Views;

public partial class SearchSongPartsView : ContentView
{
    internal event EventHandler? PlaySongPart;

    internal event EventHandler? AddSongPart;

    /// <summary> Updates swipe next item. </summary>
    internal event EventHandler? EnqueueSongPart;

    internal event EventHandler? ShowSortBy;

    internal ObservableCollection<SongPart>? allSongParts;
    internal ObservableCollection<SongPart> songParts = [];
    internal List<SongPart> searchFilteredSongParts = [];
    internal MainPage? ParentPage { get; set; }

    private VisualContainer? _visualContainer;
    private int _lastUpperItem = 17;

    public SearchSongPartsView()
    {
        InitializeComponent();
        Loaded += OnLoad;
    }

    private void OnLoad(object? sender, EventArgs e)
    {
        songParts.CollectionChanged += SongPartsCollectionChanged;

        allSongParts = SongPartRepository.SongParts;

        // Fill in indices.
        int i = 0;
        foreach (var songPart in allSongParts)
        {
            songPart.Id = i;
            songParts.Add(songPart);
            i++;
        }
        SonglibraryListView.ItemsSource = songParts;
        MainViewModel.SongParts = [.. songParts];

        UpdateResultsText();

        ClearCategoryFilterButton.IsVisible = (MainViewModel.SearchFilterMode != SearchFilterMode.All);
        ClearCategoryFilterImageButton.IsVisible = (MainViewModel.SearchFilterMode != SearchFilterMode.All);
    }

    internal void ToggleAudioModeButtonClicked(object? sender, EventArgs e)
    {
        MainViewModel.UsingVideoMode = !MainViewModel.UsingVideoMode;
        if (ParentPage!.Title.Equals(string.Empty))
        {
            ParentPage.ShowSecondaryToolbarItems();
        }
        else
        {
            ParentPage.SetupSearchToolbar();
        }

        if (MainViewModel.UsingVideoMode)
        {
            General.ShowToast("Choreo video mode");
            //CommunityToolkit.Maui.Alerts.Snackbar.Make($"Choreo video mode").Show();
        }
        else
        {
            General.ShowToast($"Audio only mode");
            //CommunityToolkit.Maui.Alerts.Snackbar.Make($"Choreo video mode").Show();
        }
    }

    internal void SongPartsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => UpdateResultsText();

    private void SonglibraryListView_Loaded(object sender, ListViewLoadedEventArgs e)
    {
        SonglibraryListView.DataSource?.GroupDescriptors.Clear();
        SonglibraryListView.IsStickyGroupHeader = true;

        songParts = songParts.OrderBy(s => s.ArtistName)
                             .ThenBy(s => s.Title)
                             .ThenBy(s => s.PartClassification).ToObservableCollection();

        songParts.ToList().ForEach(s => s.Artist!.ShowGroupTypeColor = true);

        SonglibraryListView.DataSource?.GroupDescriptors.Add(new GroupDescriptor()
        {
            PropertyName = "ArtistName",
            KeySelector = (object obj1) =>
            {
                var item = (obj1 as SongPart);

                if (item is not null && item.Artist is not null)
                {
                    return item.Artist;
                }
                else
                {
                    return "Artist not found";
                }
            },
        });

        songParts.ToList().ForEach(s => s.Album!.ShowAlbumReleaseDate = false);
        songParts.ToList().ForEach(s => s.Album!.ShowAlbumTitle = false);
        songParts.ToList().ForEach(s => s.ShowClipLength = false);

        SonglibraryListView.CollapseAll();

        _visualContainer = SonglibraryListView.GetVisualContainer();
        _visualContainer.ScrollRows!.Changed += ScrollRows_Changed;
    }

    private void ScrollRows_Changed(object sender, ScrollChangedEventArgs e)
    {
        var lastIndex = _visualContainer!.ScrollRows!.LastBodyVisibleLineIndex;

        // To include header if used
        var header = (SonglibraryListView.HeaderTemplate != null && !SonglibraryListView.IsStickyHeader) ? 1 : 0;

        var lastVisibleIndex = _visualContainer.ScrollRows!.LastBodyVisibleLineIndex;

        // To include footer if used
        var footer = (SonglibraryListView.FooterTemplate != null && !SonglibraryListView.IsStickyFooter) ? 1 : 0;
        var totalItems = SonglibraryListView.DataSource!.DisplayItems.Count + header + footer;

        if (lastIndex == totalItems - 1)
        {
            if (GoToBottomButton.IsVisible)
            {
                GoToBottomButton.IsVisible = false;
            }
        }
        else
        {
            GoToBottomButton.IsVisible = true;
        }

        if (lastVisibleIndex > _lastUpperItem)
        {
            GoToTopButton.IsVisible = true;
            _lastUpperItem = 17;
        }
        else
        {
            GoToTopButton.IsVisible = false;
            _lastUpperItem = 20;
        }
    }

    private void SwipeItemPlaySongPart(object sender, EventArgs e)
    {
        if (!General.HasInternetConnection()) { return; }
        SongPart songPart = (SongPart)((MenuItem)sender).CommandParameter;
        if (string.IsNullOrWhiteSpace(songPart.AudioURL)) { General.ShowToast("No valid audio URL."); return; }

        MainViewModel.CurrentSongPart = songPart;

        AudioManager.PlayAudio(songPart);
        PlaySongPart?.Invoke(sender, e);
    }

    // Sender is a Syncfusion.Maui.ListView.SfListView
    private async void SonglibraryListViewItemTapped(object sender, Syncfusion.Maui.ListView.ItemTappedEventArgs e)
    {
        if (!General.HasInternetConnection()) { return; }
        if (e.ItemType != Syncfusion.Maui.ListView.ItemType.Record) { return; }

        SongPart songPart = (SongPart)e.DataItem;

        if (MainViewModel.UsingVideoMode && songPart.HasVideo)
        {
            AudioManager.StopAudio();

            if (Navigation.NavigationStack.Count < 2)
            {
                await Navigation.PushAsync(new VideoPage(songPart), true);
            }
        }
        else if (!string.IsNullOrWhiteSpace(songPart.AudioURL))
        {
            // Mode to queue/single song
            MainViewModel.PlayMode = PlayMode.Queue;

            MainViewModel.CurrentSongPart = songPart;
            AudioManager.PlayAudio(songPart);

            PlaySongPart?.Invoke(sender, e);
        }

        SonglibraryListView.SelectedItems?.Clear();
    }

    private void SonglibraryListViewSwipeEnded(object sender, Syncfusion.Maui.ListView.SwipeEndedEventArgs e)
    {
        if (e.DataItem is null) { return; }

        // Swipe left to right (start), add to queue
        if (e.Direction == SwipeDirection.Right && e.Offset > 30)
        {
            SongPart songPart = (SongPart)e.DataItem;

            if (!MainViewModel.SongPartsQueue.Contains(songPart))
            {
                MainViewModel.SongPartsQueue.Enqueue(songPart);
                General.ShowToast($"Enqueued: {songPart.ArtistName} - {songPart.Title} {songPart.PartNameFull}");
            }

            // Change mode to queue list
            MainViewModel.PlayMode = PlayMode.Queue;

            EnqueueSongPart?.Invoke(sender, e);
        }

        // Swipe right to left (end), add to playlist
        else if (e.Direction == SwipeDirection.Left && e.Offset > -30)
        {
            SongPart songPart = (SongPart)e.DataItem;

            bool added = CurrentPlaylistManager.Instance.AddSongPartToCurrentPlaylist(songPart);

            if (added)
            {
                MainViewModel.PlaylistQueue.Enqueue(songPart);
                AddSongPart?.Invoke(sender, e);
                General.ShowToast($"Added: {songPart.ArtistName} - {songPart.Title} {songPart.PartNameFull}");
            }
        }

        // TODO: How to close swipe item automatically after swipe?
    }

    // Not used
    private void SwipeItemAddSong(object sender, EventArgs e)
    {
        SongPart songPart = (SongPart)((MenuItem)sender).CommandParameter;

        bool added = CurrentPlaylistManager.Instance.AddSongPartToCurrentPlaylist(songPart);

        if (added)
        {
            AddSongPart?.Invoke(sender, e);
            General.ShowToast($"Added: {songPart.ArtistName} - {songPart.Title} {songPart.PartNameFull}");
        }
    }

    private void SwipeItemQueueSong(object sender, EventArgs e)
    {
        SongPart songPart = (SongPart)((MenuItem)sender).CommandParameter;

        if (!MainViewModel.SongPartsQueue.Contains(songPart))
        {
            MainViewModel.SongPartsQueue.Enqueue(songPart);
            General.ShowToast($"Enqueued: {songPart.ArtistName} - {songPart.Title} {songPart.PartNameFull}");
        }

        // Change mode to queue list
        MainViewModel.PlayMode = PlayMode.Queue;
    }

    // SwipeEnded does not work because commandparameter only works with swipeitem
    // TODO: SwipeMode execute with a label that is EASY to see, right now you would need to swipe half the screen if swipemode execute...
    private void SwipeGroupItemAddSongs(object sender, EventArgs e)
    {
        // TODO: BUGGY
        if (CurrentPlaylistManager.Instance.CurrentPlaylist is null)
        {
            General.ShowToast($"Select a playlist first!");
            return;
        }

        SwipeItemView mi = (SwipeItemView)sender;
        var songPartsFromGroup = (IEnumerable<SongPart>)mi.CommandParameter;

        int addedSongParts = CurrentPlaylistManager.Instance.AddSongPartsToCurrentPlaylist(songPartsFromGroup.ToList());

        AddSongPart?.Invoke(sender, e);
        General.ShowToast($"{addedSongParts} songs added!");
    }

    private void GoToBottomButtonClicked(object sender, EventArgs e) => SonglibraryListView!.ItemsLayout!.ScrollToRowIndex(SonglibraryListView!.DataSource!.DisplayItems.Count - 1, animated: true);

    private void GoToTopButtonClicked(object sender, EventArgs e) => SonglibraryListView.ScrollTo(0); // This one animates slower

    /// <summary> Plays or adds random songpart. </summary>
    /// <param name="sender"> </param>
    /// <param name="e"> </param>
    internal void PlayRandomButtonClicked(object? sender, EventArgs e)
    {
        if (!General.HasInternetConnection()) { return; }

        int index = General.Rng.Next(songParts.Count);
        SongPart songPart = songParts[index];

        if (MainViewModel.IsCurrentlyPlayingSongPart)
        {
            if (!MainViewModel.SongPartsQueue.Contains(songPart))
            {
                MainViewModel.SongPartsQueue.Enqueue(songPart);
                General.ShowToast($"Enqueued: {songPart.ArtistName} - {songPart.Title} {songPart.PartNameFull}");
                EnqueueSongPart?.Invoke(sender, e);
            }
        }
        else
        {
            MainViewModel.CurrentSongPart = songPart;
            AudioManager.PlayAudio(songPart);
            PlaySongPart?.Invoke(sender, e);
        }
    }

    private void AddResultsButton_Clicked(object sender, EventArgs e)
    {
        // TODO: BUGGY
        if (CurrentPlaylistManager.Instance.CurrentPlaylist is null)
        {
            General.ShowToast($"Select a playlist first!");
            return;
        }

        int addedSongParts;
        // If search filtered or not
        if (searchFilteredSongParts.Count > 0)
        {
            addedSongParts = CurrentPlaylistManager.Instance.AddSongPartsToCurrentPlaylist([.. searchFilteredSongParts]);
        }
        else
        {
            addedSongParts = CurrentPlaylistManager.Instance.AddSongPartsToCurrentPlaylist([.. songParts]);
        }

        // Show result
        if (addedSongParts > 0)
        {
            AddSongPart?.Invoke(sender, e);
            General.ShowToast($"{addedSongParts} songs added!");
        }
    }

    internal void CollapseAllButtonClicked(object? sender, EventArgs e) => SonglibraryListView.CollapseAll();

    internal void ExpandAllButtonClicked(object? sender, EventArgs e) => SonglibraryListView.ExpandAll();

    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        // Set filter if set.
        SetFilterMode();

        SetSearchFilteredDataSource();
    }

    private void SetSearchFilteredDataSource()
    {
        searchFilteredSongParts = (SearchBarInput.Text is not null) ? 
                                                        searchFilteredSongParts = songParts.Where(s => s.ArtistName.Contains(SearchBarInput.Text, StringComparison.OrdinalIgnoreCase) ||
                                                        s.Artist.AltName.Contains(SearchBarInput.Text, StringComparison.OrdinalIgnoreCase) ||
                                                        s.Title.Contains(SearchBarInput.Text, StringComparison.OrdinalIgnoreCase))
                                                        .ToList() : [.. songParts];

        SonglibraryListView.ItemsSource = searchFilteredSongParts;
        MainViewModel.SongParts = [.. songParts];

        IEnumerable<SongPart> artistsBySongPart = searchFilteredSongParts.DistinctBy(s => new { s.ArtistName });
        ResultsLabel.Text = $"{searchFilteredSongParts.Count} results from {artistsBySongPart.Count()} artists";
    }

    internal void SortButtonClicked(object? sender, EventArgs e) => ShowSortBy?.Invoke(sender, e);

    #region Filter
    private void ClearCategoryFilterButtonClicked(object sender, EventArgs e)
    {
        MainViewModel.SearchFilterMode = SearchFilterMode.All;
        SetFilterMode();

        SetSearchFilteredDataSource();
        // TODO: Re-sort after filter.
    }

    private void ClearSearchFilterButtonClicked(object sender, EventArgs e) => SearchBarInput.Text = string.Empty;

    /// <summary> Sets filter mode and updates filterLabel </summary>
    internal void SetFilterMode()
    {
        songParts.CollectionChanged -= SongPartsCollectionChanged;

        ClearCategoryFilterButton.IsVisible = (MainViewModel.SearchFilterMode != SearchFilterMode.All);
        ClearCategoryFilterImageButton.IsVisible = (MainViewModel.SearchFilterMode != SearchFilterMode.All);

        try
        {
            switch (MainViewModel.SearchFilterMode)
            {
                case SearchFilterMode.All: songParts = allSongParts!; break;

                case SearchFilterMode.DanceVideos:
                    songParts = allSongParts!.Where(s => s.HasVideo).ToObservableCollection();
                    MainViewModel.UsingVideoMode = true;
                    break;

                case SearchFilterMode.Male:
                    songParts = allSongParts!.Where(s => s.Artist?.GroupType == GroupType.BG).ToObservableCollection(); break;
                case SearchFilterMode.Female:
                    songParts = allSongParts!.Where(s => s.Artist?.GroupType == GroupType.GG).ToObservableCollection(); break;

                case SearchFilterMode.Hybe:
                    songParts = allSongParts!.Where(s => MainViewModel.HybeCompanies.Contains(s.Artist.Company)).ToObservableCollection(); break;
                case SearchFilterMode.YG:
                    songParts = allSongParts!.Where(s => MainViewModel.YGCompanies.Contains(s.Artist.Company)).ToObservableCollection(); break;
                case SearchFilterMode.JYP:
                    songParts = allSongParts!.Where(s => s.Artist?.Company == "JYP Entertainment").ToObservableCollection(); break;
                case SearchFilterMode.SM:
                    songParts = allSongParts!.Where(s => MainViewModel.SMCompanies.Contains(s.Artist.Company)).ToObservableCollection(); break;
                case SearchFilterMode.Cube:
                    songParts = allSongParts!.Where(s => s.Artist?.Company == "Cube Entertainment").ToObservableCollection(); break;
                case SearchFilterMode.FNC:
                    songParts = allSongParts!.Where(s => s.Artist?.Company == "FNC Entertainment").ToObservableCollection(); break;
                case SearchFilterMode.Pledis:
                    songParts = allSongParts!.Where(s => s.Artist?.Company == "Pledis Entertainment").ToObservableCollection(); break;
                case SearchFilterMode.Starship:
                    songParts = allSongParts!.Where(s => s.Artist?.Company == "Starship Entertainment").ToObservableCollection(); break;
                case SearchFilterMode.RBW:
                    songParts = allSongParts!.Where(s => MainViewModel.RbwCompanies.Contains(s.Artist.Company)).ToObservableCollection(); break;
                case SearchFilterMode.Woollim:
                    songParts = allSongParts!.Where(s => s.Artist?.Company == "Woollim Entertainment").ToObservableCollection(); break;
                case SearchFilterMode.IST:
                    songParts = allSongParts!.Where(s => s.Artist?.Company == "IST Entertainment").ToObservableCollection(); break;

                case SearchFilterMode.CJ_ENM_Music:
                    songParts = allSongParts!.Where(s => MainViewModel.CjenmCompanies.Contains(s.Artist.Company)).ToObservableCollection(); break;
                case SearchFilterMode.Kakao_Entertainment:
                    songParts = allSongParts!.Where(s => MainViewModel.KakaoCompanies.Contains(s.Artist.Company)).ToObservableCollection(); break;

                case SearchFilterMode.Firstgen:
                    songParts = allSongParts!.Where(s => s.Artist?.Generation == MainViewModel.FIRST_GENERATION && s.Album?.GenreShort == MainViewModel.GenreKpop).ToObservableCollection(); break;
                case SearchFilterMode.Secondgen:
                    songParts = allSongParts!.Where(s => s.Artist?.Generation == MainViewModel.SECOND_GENERATION && s.Album?.GenreShort == MainViewModel.GenreKpop).ToObservableCollection(); break;
                case SearchFilterMode.Thirdgen:
                    songParts = allSongParts!.Where(s => s.Artist?.Generation == MainViewModel.THIRD_GENERATION && s.Album?.GenreShort == MainViewModel.GenreKpop).ToObservableCollection(); break;
                case SearchFilterMode.Fourthgen:
                    songParts = allSongParts!.Where(s => s.Artist?.Generation == MainViewModel.FOURTH_GENERATION && s.Album?.GenreShort == MainViewModel.GenreKpop).ToObservableCollection(); break;
                case SearchFilterMode.Fifthgen:
                    songParts = allSongParts!.Where(s => s.Artist?.Generation == MainViewModel.FIFTH_GENERATION && s.Album?.GenreShort == MainViewModel.GenreKpop).ToObservableCollection(); break;

                case SearchFilterMode.Kpop:
                    songParts = allSongParts!.Where(s => s.Album?.GenreShort == MainViewModel.GenreKpop).ToObservableCollection(); break;
                case SearchFilterMode.Jpop:
                    songParts = allSongParts!.Where(s => s.Album?.GenreShort == "JP").ToObservableCollection(); break;
                case SearchFilterMode.EN:
                    songParts = allSongParts!.Where(s => s.Album?.GenreShort == "EN").ToObservableCollection(); break;
                case SearchFilterMode.Cpop:
                    songParts = allSongParts!.Where(s => s.Album?.GenreShort == "CH").ToObservableCollection(); break;
                case SearchFilterMode.Tpop:
                    songParts = allSongParts!.Where(s => s.Album?.GenreShort == "TH").ToObservableCollection(); break;

                case SearchFilterMode.Solo:
                    songParts = allSongParts!.Where(s => s.Artist?.MemberCount == 1).ToObservableCollection(); break;
                case SearchFilterMode.Duo:
                    songParts = allSongParts!.Where(s => s.Artist?.MemberCount == 2).ToObservableCollection(); break;
                case SearchFilterMode.Trio:
                    songParts = allSongParts!.Where(s => s.Artist?.MemberCount == 3).ToObservableCollection(); break;
                case SearchFilterMode.Quadruplet:
                    songParts = allSongParts!.Where(s => s.Artist?.MemberCount == 4).ToObservableCollection(); break;
                case SearchFilterMode.Quintet:
                    songParts = allSongParts!.Where(s => s.Artist?.MemberCount == 5).ToObservableCollection(); break;
                case SearchFilterMode.Sextet:
                    songParts = allSongParts!.Where(s => s.Artist?.MemberCount == 6).ToObservableCollection(); break;
                case SearchFilterMode.Septet:
                    songParts = allSongParts!.Where(s => s.Artist?.MemberCount == 7).ToObservableCollection(); break;
                case SearchFilterMode.Octet:
                    songParts = allSongParts!.Where(s => s.Artist?.MemberCount == 8).ToObservableCollection(); break;
                case SearchFilterMode.Nonet:
                    songParts = allSongParts!.Where(s => s.Artist?.MemberCount == 9).ToObservableCollection(); break;
                case SearchFilterMode.Group:
                    songParts = allSongParts!.Where(s => s.Artist?.MemberCount > 2).ToObservableCollection(); break;

                case SearchFilterMode.KpopSoonerThan2012:
                    songParts = allSongParts!.Where(s => s.Album?.ReleaseDate.Year < 2012 && s.Album?.GenreShort == MainViewModel.GenreKpop).ToObservableCollection(); break;
                case SearchFilterMode.kpop2012:
                    songParts = allSongParts!.Where(s => s.Album?.ReleaseDate.Year == 2012 && s.Album?.GenreShort == MainViewModel.GenreKpop).ToObservableCollection(); break;
                case SearchFilterMode.kpop2013:
                    songParts = allSongParts!.Where(s => s.Album?.ReleaseDate.Year == 2013 && s.Album?.GenreShort == MainViewModel.GenreKpop).ToObservableCollection(); break;
                case SearchFilterMode.kpop2014:
                    songParts = allSongParts!.Where(s => s.Album?.ReleaseDate.Year == 2014 && s.Album?.GenreShort == MainViewModel.GenreKpop).ToObservableCollection(); break;
                case SearchFilterMode.kpop2015:
                    songParts = allSongParts!.Where(s => s.Album?.ReleaseDate.Year == 2015 && s.Album?.GenreShort == MainViewModel.GenreKpop).ToObservableCollection(); break;
                case SearchFilterMode.kpop2016:
                    songParts = allSongParts!.Where(s => s.Album?.ReleaseDate.Year == 2016 && s.Album?.GenreShort == MainViewModel.GenreKpop).ToObservableCollection(); break;
                case SearchFilterMode.kpop2017:
                    songParts = allSongParts!.Where(s => s.Album?.ReleaseDate.Year == 2017 && s.Album?.GenreShort == MainViewModel.GenreKpop).ToObservableCollection(); break;
                case SearchFilterMode.kpop2018:
                    songParts = allSongParts!.Where(s => s.Album?.ReleaseDate.Year == 2018 && s.Album?.GenreShort == MainViewModel.GenreKpop).ToObservableCollection(); break;
                case SearchFilterMode.kpop2019:
                    songParts = allSongParts!.Where(s => s.Album?.ReleaseDate.Year == 2019 && s.Album?.GenreShort == MainViewModel.GenreKpop).ToObservableCollection(); break;
                case SearchFilterMode.kpop2020:
                    songParts = allSongParts!.Where(s => s.Album?.ReleaseDate.Year == 2020 && s.Album?.GenreShort == MainViewModel.GenreKpop).ToObservableCollection(); break;
                case SearchFilterMode.kpop2021:
                    songParts = allSongParts!.Where(s => s.Album?.ReleaseDate.Year == 2021 && s.Album?.GenreShort == MainViewModel.GenreKpop).ToObservableCollection(); break;
                case SearchFilterMode.kpop2022:
                    songParts = allSongParts!.Where(s => s.Album?.ReleaseDate.Year == 2022 && s.Album?.GenreShort == MainViewModel.GenreKpop).ToObservableCollection(); break;
                case SearchFilterMode.kpop2023:
                    songParts = allSongParts!.Where(s => s.Album?.ReleaseDate.Year == 2023 && s.Album?.GenreShort == MainViewModel.GenreKpop).ToObservableCollection(); break;
                case SearchFilterMode.kpop2024:
                    songParts = allSongParts!.Where(s => s.Album?.ReleaseDate.Year == 2024 && s.Album?.GenreShort == MainViewModel.GenreKpop).ToObservableCollection(); break;
                case SearchFilterMode.kpop2025:
                    songParts = allSongParts!.Where(s => s.Album?.ReleaseDate.Year == 2025 && s.Album?.GenreShort == MainViewModel.GenreKpop).ToObservableCollection(); break;
            }

            ParentPage?.SetupSearchToolbar();

            songParts!.CollectionChanged += SongPartsCollectionChanged;
            SonglibraryListView.ItemsSource = songParts;
            MainViewModel.SongParts = [.. songParts];

            UpdateResultsText();
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
        }
    }
    // Update searchfilter as well.
    private void UpdateResultsText()
    {
        IEnumerable<SongPart> artistsBySongPart = songParts.DistinctBy(s => new { s.ArtistName });
        ResultsLabel.Text = $"{songParts.Count} results from {artistsBySongPart.Count()} artists";
    }

    #endregion Filter

    #region Sort

    internal void RefreshSort(bool fromSetSearchFilteredDataSource = false)
    {
        if (SonglibraryListView is null || SonglibraryListView.DataSource is null) { return; }

        songParts.CollectionChanged -= SongPartsCollectionChanged;

        SonglibraryListView.DataSource?.GroupDescriptors.Clear();
        SonglibraryListView.DataSource?.SortDescriptors.Clear();
        songParts.ToList().ForEach(s => s.Artist!.ShowGroupTypeColor = false);

        try
        {
            switch (MainViewModel.SortMode)
            {
                case SortMode.AlbumName:
                    songParts.ToList().ForEach(s => s.Album!.ShowAlbumTitle = true);

                    songParts = songParts.OrderBy(s => s.AlbumTitle).ThenBy(s => s.Title).ThenBy(s => s.PartClassification).ToObservableCollection();

                    SonglibraryListView.DataSource?.GroupDescriptors.Add(new GroupDescriptor()
                    {
                        PropertyName = "AlbumTitle",
                        KeySelector = (object obj1) =>
                        {
                            var item = (obj1 as SongPart);
                            return item!.AlbumTitle.ToUpper()[0].ToString();
                        },
                    });

                    songParts.ToList().ForEach(s => s.Album!.ShowAlbumReleaseDate = false);
                    songParts.ToList().ForEach(s => s.Artist!.ShowGroupType = false);
                    songParts.ToList().ForEach(s => s.ShowClipLength = false);
                    break;

                case SortMode.Artist:
                    songParts = songParts.OrderBy(s => s.ArtistName)
                                         .ThenBy(s => s.Title)
                                         .ThenBy(s => s.PartClassification).ToObservableCollection();

                    songParts.ToList().ForEach(s => s.Artist!.ShowGroupTypeColor = true);

                    SonglibraryListView.DataSource?.GroupDescriptors.Add(new GroupDescriptor()
                    {
                        PropertyName = "ArtistName",
                        KeySelector = (object obj1) =>
                        {
                            var item = (obj1 as SongPart);

                            if (item is not null && item.Artist is not null)
                            {
                                return item.Artist;
                            }
                            else
                            {
                                return "Artist not found";
                            }
                        },
                    });

                    songParts.ToList().ForEach(s => s.Album!.ShowAlbumTitle = false);
                    songParts.ToList().ForEach(s => s.Album!.ShowAlbumReleaseDate = false);
                    songParts.ToList().ForEach(s => s.Artist!.ShowGroupType = false);
                    songParts.ToList().ForEach(s => s.ShowClipLength = false);
                    break;

                case SortMode.ArtistSongCount:
                    songParts.ToList().ForEach(s => s.Artist!.ShowGroupTypeColor = true);

                    foreach (var s in songParts)
                    {
                        s.Artist!.FilteredTotalCount = 0;
                    }

                    foreach (var s in songParts)
                    {
                        s.Artist!.FilteredTotalCount++;
                    }
                    songParts = songParts.OrderByDescending(s => s.Artist?.FilteredTotalCount)
                                         .ThenBy(s => s.Album.ReleaseDate)
                                         .ThenBy(s => s.Title)
                                         .ThenBy(s => s.PartClassification)
                                         .ThenBy(s => s.PartNameNumber).ToObservableCollection();

                    SonglibraryListView.DataSource?.GroupDescriptors.Add(new GroupDescriptor()
                    {
                        PropertyName = "ArtistName",
                        KeySelector = (object obj1) =>
                        {
                            var item = (obj1 as SongPart);

                            if (item?.Artist is not null)
                            {
                                return item.Artist;
                            }
                            else
                            {
                                return "Artist not found";
                            }
                        },
                    });

                    songParts.ToList().ForEach(s => s.Album!.ShowAlbumTitle = false);
                    songParts.ToList().ForEach(s => s.Album!.ShowAlbumReleaseDate = false);
                    songParts.ToList().ForEach(s => s.Artist!.ShowGroupType = false);
                    songParts.ToList().ForEach(s => s.ShowClipLength = false);
                    break;

                case SortMode.Company:
                    songParts = songParts.OrderBy(s => s.Artist?.Company)
                                         .ThenBy(s => s.Title)
                                         .ThenBy(s => s.PartClassification).ToObservableCollection();

                    SonglibraryListView.DataSource?.GroupDescriptors.Add(new GroupDescriptor()
                    {
                        PropertyName = "Company",
                        KeySelector = (object obj1) =>
                        {
                            var item = (obj1 as SongPart);

                            if (item?.Artist is not null)
                            {
                                return item!.Artist!.Company;
                            }
                            else
                            {
                                return "Artist not found";
                            }
                        },
                    });

                    songParts.ToList().ForEach(s => s.Album!.ShowAlbumTitle = false);
                    songParts.ToList().ForEach(s => s.Album!.ShowAlbumReleaseDate = false);
                    songParts.ToList().ForEach(s => s.Artist!.ShowGroupType = false);
                    songParts.ToList().ForEach(s => s.ShowClipLength = false);
                    break;

                case SortMode.ClipLength:
                    songParts = songParts.OrderBy(s => s.ClipLength).ToObservableCollection();
                    songParts.ToList().ForEach(s => s.Artist!.ShowGroupTypeColor = false);

                    SonglibraryListView.DataSource?.GroupDescriptors.Add(new GroupDescriptor()
                    {
                        PropertyName = "ClipLengthRange",
                        KeySelector = (object obj1) =>
                        {
                            var item = (obj1 as SongPart);

                            return item?.ClipLength switch
                            {
                                <= 20.0 => "< 20s",
                                > 20.0 and <= 25.0 => "20-25s",
                                > 25.0 and <= 30.0 => "25-30s",
                                > 30.0 and <= 35.0 => "30-35s",
                                > 35.0 and <= 40.0 => "35-40s",
                                > 40.0 and <= 45.0 => "40-45s",
                                > 45.0 and <= 50.0 => "45-50s",
                                > 50.0 and <= 55.0 => "50-55s",
                                > 55.0 and <= 60.0 => "55-60s",
                                > 60.0 => "> 1 min",

                                _ => "Unknown"
                            };
                        },
                    });

                    songParts.ToList().ForEach(s => s.Album!.ShowAlbumTitle = false);
                    songParts.ToList().ForEach(s => s.Album!.ShowAlbumReleaseDate = false);
                    songParts.ToList().ForEach(s => s.Artist!.ShowGroupType = false);
                    songParts.ToList().ForEach(s => s.ShowClipLength = true);
                    break;

                case SortMode.Generation:
                    // Shows only korean songs, else you have to mess with the group non-kpop which
                    // gets in the way of grouping order.
                    songParts = songParts.Where(s => s.Album.GenreShort == MainViewModel.GenreKpop).OrderByDescending(s => s.Artist.Gen)
                                                                                .ThenBy(s => s.Title)
                                                                                .ThenBy(s => s.PartClassification).ToObservableCollection();

                    songParts.ToList().ForEach(s => s.Artist!.ShowGroupTypeColor = false);

                    SonglibraryListView.DataSource?.GroupDescriptors.Add(new GroupDescriptor()
                    {
                        PropertyName = "Generation",
                        KeySelector = (object obj1) =>
                        {
                            var item = (obj1 as SongPart);

                            if (item is not null && item.Artist is not null)
                            {
                                return item.Artist.Generation;
                            }
                            else
                            {
                                return "Artist not found";
                            }
                        },
                    });

                    foreach (SongPart sp in songParts)
                    {
                        sp.Album.ShowAlbumReleaseDate = false;
                        sp.Artist.ShowGroupType = false;
                        sp.ShowClipLength = false;
                    }
                    break;

                case SortMode.GroupType:
                    songParts = songParts.OrderBy(s => s.Artist?.GroupType)
                                         .ThenBy(s => s.Title)
                                         .ThenBy(s => s.PartClassification).ToObservableCollection();

                    // TODO: Does not work because you need to give Artist object and override ToString() needs to give grouptype somehow but now it gives artist name.
                    //songParts.ToList().ForEach(s => s.Artist!.ShowGroupTypeColor = false);

                    SonglibraryListView?.DataSource?.GroupDescriptors.Add(new GroupDescriptor()
                    {
                        PropertyName = "GroupType",
                        KeySelector = (object obj1) =>
                        {
                            var item = (obj1 as SongPart);

                            if (item is not null && item.Artist is not null)
                            {
                                return item!.Artist!.GroupType.ToString();
                            }
                            else
                            {
                                return "Artist not found";
                            }
                        },
                    });

                    songParts.ToList().ForEach(s => s.Album!.ShowAlbumTitle = false);
                    songParts.ToList().ForEach(s => s.Album!.ShowAlbumReleaseDate = false);
                    songParts.ToList().ForEach(s => s.Artist!.ShowGroupType = true);
                    songParts.ToList().ForEach(s => s.ShowClipLength = false);
                    break;

                case SortMode.Genre:
                    songParts = songParts.OrderBy(s => s.Album?.GenreShort)
                                         .ThenBy(s => s.Title)
                                         .ThenBy(s => s.PartClassification).ToObservableCollection();

                    SonglibraryListView?.DataSource?.GroupDescriptors.Add(new GroupDescriptor()
                    {
                        PropertyName = "Genre",
                        KeySelector = (object obj1) =>
                        {
                            var item = (obj1 as SongPart);
                            if (item is not null && item.Album is not null)
                            {
                                return item.Album.GenreFull;
                            }
                            else
                            {
                                return "Album not found";
                            }
                        }
                    });

                    songParts.ToList().ForEach(s => s.Album!.ShowAlbumTitle = false);
                    songParts.ToList().ForEach(s => s.Album.ShowAlbumReleaseDate = false);
                    songParts.ToList().ForEach(s => s.Artist.ShowGroupType = false);
                    songParts.ToList().ForEach(s => s.ShowClipLength = false);
                    break;

                case SortMode.MemberCount:
                    songParts = songParts.OrderByDescending(s => s.Artist?.MemberCount)
                                         .ThenBy(s => s.Title)
                                         .ThenBy(s => s.PartClassification).ToObservableCollection();

                    songParts.ToList().ForEach(s => s.Artist!.ShowGroupTypeColor = false);

                    SonglibraryListView.DataSource?.GroupDescriptors.Add(new GroupDescriptor()
                    {
                        PropertyName = "MemberCount",
                        KeySelector = (object obj1) =>
                        {
                            var item = (obj1 as SongPart);
                            if (item!.Artist!.MemberCount == 1)
                                return $"{item!.Artist!.MemberCount} member";
                            else
                                return $"{item!.Artist!.MemberCount} members";
                        },
                    });

                    songParts.ToList().ForEach(s => s.Album!.ShowAlbumTitle = false);
                    songParts.ToList().ForEach(s => s.Album!.ShowAlbumReleaseDate = false);
                    songParts.ToList().ForEach(s => s.Artist!.ShowGroupType = false);
                    songParts.ToList().ForEach(s => s.ShowClipLength = false);
                    break;

                case SortMode.ReleaseDate:
                    songParts = songParts.OrderByDescending(s => s.Album.ReleaseDate)
                                         .ThenBy(s => s.Title)
                                         .ThenBy(s => s.PartClassification).ToObservableCollection();

                    SonglibraryListView.DataSource?.GroupDescriptors.Add(new GroupDescriptor()
                    {
                        PropertyName = "ReleaseYear",
                        KeySelector = (object obj1) =>
                        {
                            var item = (obj1 as SongPart);

                            if (item is not null && item.Album is not null)
                            {
                                return item.Album.ReleaseDate.Year;
                            }
                            else
                            {
                                return "Album not found";
                            }
                        },
                    });

                    songParts.ToList().ForEach(s => s.Album!.ShowAlbumTitle = false);
                    songParts.ToList().ForEach(s => s.Album!.ShowAlbumReleaseDate = true);
                    songParts.ToList().ForEach(s => s.Artist!.ShowGroupType = false);
                    songParts.ToList().ForEach(s => s.ShowClipLength = false);
                    break;

                case SortMode.SongPart:
                    songParts = songParts.OrderBy(s => s.PartClassification).ToObservableCollection();

                    SonglibraryListView.DataSource?.GroupDescriptors.Add(new GroupDescriptor()
                    {
                        PropertyName = "PartNameClassification",
                        KeySelector = (object obj1) =>
                        {
                            var item = (obj1 as SongPart);
                            return item!.PartClassification;
                        },
                    });

                    songParts.ToList().ForEach(s => s.Album!.ShowAlbumTitle = false);
                    songParts.ToList().ForEach(s => s.Album!.ShowAlbumReleaseDate = false);
                    songParts.ToList().ForEach(s => s.Artist!.ShowGroupType = false);
                    songParts.ToList().ForEach(s => s.ShowClipLength = false);

                    break;

                case SortMode.Title:
                    songParts = songParts.OrderBy(s => s.Title)
                                         .ThenBy(s => s.PartClassification).ToObservableCollection();

                    SonglibraryListView.DataSource?.GroupDescriptors.Add(new GroupDescriptor()
                    {
                        PropertyName = "Title",
                        KeySelector = (object obj1) =>
                        {
                            var item = (obj1 as SongPart);
                            return item!.Title.ToUpper()[0].ToString();
                        },
                    });

                    songParts.ToList().ForEach(s => s.Album!.ShowAlbumTitle = false);
                    songParts.ToList().ForEach(s => s.Album!.ShowAlbumReleaseDate = false);
                    songParts.ToList().ForEach(s => s.Artist!.ShowGroupType = false);
                    songParts.ToList().ForEach(s => s.ShowClipLength = false);
                    break;

                case SortMode.ReleaseWeekDay:
                    songParts = songParts.OrderBy(s => s.Album.ReleaseDate.DayOfWeek)
                                         .ThenBy(s => s.Title)
                                         .ThenBy(s => s.PartClassification).ToObservableCollection();

                    SonglibraryListView.DataSource?.GroupDescriptors.Add(new GroupDescriptor()
                    {
                        PropertyName = "ReleaseDateWeekDAy",
                        KeySelector = (object obj1) =>
                        {
                            var item = (obj1 as SongPart);
                            return item!.Album.ReleaseDate.DayOfWeek.ToString();
                        },
                    });

                    songParts.ToList().ForEach(s => s.Album!.ShowAlbumTitle = false);
                    songParts.ToList().ForEach(s => s.Album!.ShowAlbumReleaseDate = false);
                    songParts.ToList().ForEach(s => s.Artist!.ShowGroupType = false);
                    songParts.ToList().ForEach(s => s.ShowClipLength = false);
                    break;

                case SortMode.YearlyDate:
                    songParts = songParts.OrderBy(s => s.Album.ReleaseDate.Month)
                                         .ThenBy(s => s.Album.ReleaseDate.Day)
                                         .ThenBy(s => s.Title)
                                         .ThenBy(s => s.PartClassification).ToObservableCollection();

                    SonglibraryListView.DataSource?.GroupDescriptors.Add(new GroupDescriptor()
                    {
                        PropertyName = "YearlyDate",
                        KeySelector = (object obj1) =>
                        {
                            var item = (obj1 as SongPart);
                            if (item is not null)
                            {
                                int month = item!.Album.ReleaseDate.Month;
                                int day = item!.Album.ReleaseDate.Day;
                                return $"{month}/{day}";
                            }
                            else 
                            { 
                                return "Invalid item."; 
                            }
                        },
                    });

                    songParts.ToList().ForEach(s => s.Album!.ShowAlbumTitle = false);
                    songParts.ToList().ForEach(s => s.Album!.ShowAlbumReleaseDate = false);
                    songParts.ToList().ForEach(s => s.Artist!.ShowGroupType = false);
                    songParts.ToList().ForEach(s => s.ShowClipLength = false);
                    break;
            }
        }
        catch (Exception ex)
        {
            Dictionary<string, object> dict = new()
            {
                { "SortMode", MainViewModel.SortMode.ToString() },
                { "SearchFilterMode", MainViewModel.SearchFilterMode.ToString() }
            };
            ex.AddSentryContext("data", dict);

            SentrySdk.CaptureException(ex);
            General.ShowToast(ex.Message);
        }

        if (SearchBarInput is not null && SearchBarInput.Text is not null && SearchBarInput.Text.Trim().Length > 0)
        {
            SetSearchFilteredDataSource();
        }
        else if (!fromSetSearchFilteredDataSource)
        {
            SonglibraryListView!.ItemsSource = songParts;
        }

        songParts.CollectionChanged += SongPartsCollectionChanged;

        SortModeLabel.Text = MainViewModel.SortMode.ToString();
    }

    #endregion Sort
}