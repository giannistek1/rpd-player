using System.Collections.ObjectModel;
using System.Collections.Specialized;
using RpdPlayerApp.Models;
using RpdPlayerApp.Repository;
using CommunityToolkit.Maui.Core.Extensions;
using RpdPlayerApp.ViewModel;
using RpdPlayerApp.Architecture;
using Syncfusion.Maui.DataSource;
using UraniumUI.Icons.MaterialSymbols;
using CommunityToolkit.Maui.Alerts;

namespace RpdPlayerApp.Views;

public partial class SearchSongPartsView : ContentView
{
    private FontImageSource _videoOnIcon = new();
    private FontImageSource _videoOffIcon = new();

    internal event EventHandler? PlaySongPart;
    internal event EventHandler? StopSongPart;
    internal event EventHandler? AddSongPart;
    internal event EventHandler? SortPressed;

    internal ObservableCollection<SongPart> allSongParts;
    internal ObservableCollection<SongPart> songParts = [];
    internal List<SongPart> searchFilteredSongParts = [];
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

        _videoOnIcon = new FontImageSource
        {
            FontFamily = "MaterialRegular",
            Glyph = MaterialOutlined.Videocam,
        };

        _videoOffIcon = new FontImageSource
        {
            FontFamily = "MaterialRegular",
            Glyph = MaterialOutlined.Videocam_off,
        };

        ToggleAudioModeImage.Source = (MainViewModel.UsingVideoMode) ? _videoOnIcon : _videoOffIcon;
        ClearFilterButton.IsVisible = (MainViewModel.SearchFilterMode != SearchFilterMode.All);
    }

    private void ToggleAudioModeButtonClicked(object sender, EventArgs e)
    {
        MainViewModel.UsingVideoMode = !MainViewModel.UsingVideoMode;
        ToggleAudioModeImage.Source = (MainViewModel.UsingVideoMode) ? _videoOnIcon : _videoOffIcon;

        if (MainViewModel.UsingVideoMode)
        {
            Toast.Make($"Choreo video mode", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        }
        else
        {
            Toast.Make($"Audio only mode", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        }
    }


    internal void SongPartsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        ResultsLabel.Text = $"Currently showing: {songParts.Count} results";
    }

    internal void RefreshSongParts()
    {
        SonglibraryListView.ItemsSource = songParts;
    }

    private void SonglibraryListView_Loaded(object sender, Syncfusion.Maui.ListView.ListViewLoadedEventArgs e)
    {
        SonglibraryListView.DataSource?.GroupDescriptors.Clear();

        songParts = songParts.OrderBy(s => s.ArtistName).ToObservableCollection();
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

    // Sender is a Syncfusion.Maui.ListView.SfListView
    private async void SonglibraryListViewItemTapped(object sender, Syncfusion.Maui.ListView.ItemTappedEventArgs e)
    {
        if (!HelperClass.HasInternetConnection())
            return;

        if (e.ItemType != Syncfusion.Maui.ListView.ItemType.Record)
            return;

        SongPart songPart = (SongPart)e.DataItem;

        if (MainViewModel.UsingVideoMode && songPart.HasVideo)
        {
            StopSongPart?.Invoke(sender, e);

            if (Navigation.NavigationStack.Count < 2)
            {
                await Navigation.PushAsync(new VideoPage(songPart), true);
            }
        }
        else if (songPart.AudioURL != string.Empty)
        {
            // Mode to queue/single song
            MainViewModel.PlayMode = PlayMode.Queue;

            MainViewModel.CurrentSongPart = songPart;
            PlaySongPart?.Invoke(sender, e);
            TimerManager.songPart = songPart;
            TimerManager.StartInfiniteScaleYAnimationWithTimer();
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
                Toast.Make($"Enqueued: {songPart.ArtistName} - {songPart.Title} {songPart.PartNameFull}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
            }

            // Change mode to queue list
            MainViewModel.PlayMode = PlayMode.Queue;
        }

        // Swipe right to left (end), add to playlist
        else if (e.Direction == SwipeDirection.Left && e.Offset > -30)
        {
            SongPart songPart = (SongPart)e.DataItem;

            bool added = PlaylistManager.Instance.AddSongPartToCurrentPlaylist(songPart);

            if (added)
            {
                AddSongPart?.Invoke(sender, e);
                Toast.Make($"Added: {songPart.ArtistName} - {songPart.Title} {songPart.PartNameFull}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
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
            Toast.Make($"Added: {songPart.ArtistName} - {songPart.Title} {songPart.PartNameFull}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        }
    }

    private void SwipeItemQueueSong(object sender, EventArgs e)
    {
        SongPart songPart = (SongPart)((MenuItem)sender).CommandParameter;

        if (!MainViewModel.SongPartsQueue.Contains(songPart))
        {
            MainViewModel.SongPartsQueue.Enqueue(songPart);
            Toast.Make($"Enqueued: {songPart.ArtistName} - {songPart.Title} {songPart.PartNameFull}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        }

        // Change mode to queue list
        MainViewModel.PlayMode = PlayMode.Queue;
    }

    // SwipeEnded does not work because commandparameter only works with swipeitem
    // TODO: SwipeMode execute with a label that is EASY to see, right now you would need to swipe half the screen if swipemode execute...
    private void SwipeGroupItemAddSongs(object sender, EventArgs e)
    {
        // TODO: BUGGY
        if (PlaylistManager.Instance.CurrentPlaylist is null)
        {
            Toast.Make($"Select a playlist first!", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
            return;
        }

        MenuItem mi = (MenuItem)sender;
        var songParts = (IEnumerable<SongPart>)mi.CommandParameter;

        int addedSongParts = PlaylistManager.Instance.AddSongPartsToCurrentPlaylist(songParts.ToList());

        Toast.Make($"{addedSongParts} songs added!", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
    }

    private void GoToBottomButtonClicked(object sender, EventArgs e)
    {
        SonglibraryListView.ScrollTo(int.MaxValue);
    }
    private void GoToTopButtonClicked(object sender, EventArgs e)
    {
        SonglibraryListView.ScrollTo(0);
    }

    private void PlayRandomButton_Clicked(object sender, EventArgs e)
    {
        if (!HelperClass.HasInternetConnection())
            return;

        var random = new Random();
        int index = random.Next(songParts.Count);
        SongPart songPart = songParts[index];

        if (MainViewModel.CurrentlyPlaying)
        {
            if (!MainViewModel.SongPartsQueue.Contains(songPart))
            {
                MainViewModel.SongPartsQueue.Enqueue(songPart);
                Toast.Make($"Enqueued: {songPart.ArtistName} - {songPart.Title} {songPart.PartNameFull}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
            }
        }
        else
        {
            MainViewModel.CurrentSongPart = songPart;
            PlaySongPart?.Invoke(sender, e);
        }
    }

    private void AddResultsButton_Clicked(object sender, EventArgs e)
    {
        // TODO: BUGGY
        if (PlaylistManager.Instance.CurrentPlaylist is null)
        {
            Toast.Make($"Select a playlist first!", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
            return;
        }

        int addedSongParts;
        if (searchFilteredSongParts.Count > 0)
        {
            addedSongParts = PlaylistManager.Instance.AddSongPartsToCurrentPlaylist(searchFilteredSongParts.ToList());
        }
        else
        {
            addedSongParts = PlaylistManager.Instance.AddSongPartsToCurrentPlaylist(songParts.ToList());
        }

        Toast.Make($"{addedSongParts} songs added!", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
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
        SetFilterMode();

        SetSearchFilteredDataSource();
    }

    private void SetSearchFilteredDataSource()
    {
        searchFilteredSongParts = songParts.Where(s => s.ArtistName.Contains(SearchBarInput.Text, StringComparison.OrdinalIgnoreCase) ||
                                    s.Artist.AltName.Contains(SearchBarInput.Text, StringComparison.OrdinalIgnoreCase) ||
                                    s.Title.Contains(SearchBarInput.Text, StringComparison.OrdinalIgnoreCase))
                                    .ToList();


        SonglibraryListView.ItemsSource = searchFilteredSongParts;

        ResultsLabel.Text = $"Currently showing {searchFilteredSongParts.Count} results";
    }

    private void SortButton_Clicked(object sender, EventArgs e)
    {
        SortPressed?.Invoke(sender, e);
    }

    #region Filter
    private void ClearFilterButtonClicked(object sender, EventArgs e)
    {
        MainViewModel.SearchFilterMode = SearchFilterMode.All;
        SetFilterMode();

        SetSearchFilteredDataSource();
    }

    /// <summary>
    /// Sets filter mode and updates filterLabel
    /// </summary>
    internal void SetFilterMode()
    {
        songParts.CollectionChanged -= SongPartsCollectionChanged;

        ClearFilterButton.IsVisible = (MainViewModel.SearchFilterMode != SearchFilterMode.All);

        try
        {
            switch (MainViewModel.SearchFilterMode)
            {
                case SearchFilterMode.All: FilterLabel.Text = "All songs"; songParts = allSongParts; break;

                case SearchFilterMode.DanceVideos:
                    FilterLabel.Text = "Dance videos";
                    songParts = allSongParts.Where(s => s.HasVideo).ToObservableCollection();
                    MainViewModel.UsingVideoMode = true;
                    ToggleAudioModeImage.Source = _videoOnIcon;
                    break;

                case SearchFilterMode.Male:
                    FilterLabel.Text = "Boy(groups)";
                    songParts = allSongParts.Where(s => s.Artist?.GroupType == GroupType.BG).ToObservableCollection(); break;
                case SearchFilterMode.Female:
                    FilterLabel.Text = "Girl(groups)";
                    songParts = allSongParts.Where(s => s.Artist?.GroupType == GroupType.GG).ToObservableCollection(); break;

                case SearchFilterMode.Hybe:
                    FilterLabel.Text = "Hybe Labels";
                    songParts = allSongParts.Where(s => s.Artist?.Company == "HYBE Labels" || s.Artist?.Company == "Big Hit Entertainment" || s.Artist?.Company == "Source Music").ToObservableCollection(); break;
                case SearchFilterMode.YG:
                    FilterLabel.Text = "YG Entertainment";
                    songParts = allSongParts.Where(s => s.Artist?.Company == "YG Entertainment").ToObservableCollection(); break;
                case SearchFilterMode.JYP:
                    FilterLabel.Text = "JYP Entertainment";
                    songParts = allSongParts.Where(s => s.Artist?.Company == "JYP Entertainment").ToObservableCollection(); break;
                case SearchFilterMode.SM:
                    FilterLabel.Text = "SM Entertainment";
                    songParts = allSongParts.Where(s => s.Artist?.Company == "SM Entertainment" || s.Artist?.Company == "Label V").ToObservableCollection(); break;
                case SearchFilterMode.Cube:
                    FilterLabel.Text = "Cube Entertainment";
                    songParts = allSongParts.Where(s => s.Artist?.Company == "Cube Entertainment").ToObservableCollection(); break;
                case SearchFilterMode.FNC:
                    FilterLabel.Text = "FNC Entertainment";
                    songParts = allSongParts.Where(s => s.Artist?.Company == "FNC Entertainment").ToObservableCollection(); break;
                case SearchFilterMode.Pledis:
                    FilterLabel.Text = "Pledis Entertainment";
                    songParts = allSongParts.Where(s => s.Artist?.Company == "Pledis Entertainment").ToObservableCollection(); break;
                case SearchFilterMode.Starship:
                    FilterLabel.Text = "Starship Entertainment";
                    songParts = allSongParts.Where(s => s.Artist?.Company == "Starship Entertainment").ToObservableCollection(); break;
                case SearchFilterMode.RBW:
                    FilterLabel.Text = "RBW Entertainment";
                    songParts = allSongParts.Where(s => s.Artist?.Company == "RBW Entertainment" || 
                                                        s.Artist?.Company == "WM Entertainment").ToObservableCollection(); break;
                case SearchFilterMode.Woollim:
                    FilterLabel.Text = "Woollim Entertainment";
                    songParts = allSongParts.Where(s => s.Artist?.Company == "Woollim Entertainment").ToObservableCollection(); break;
                case SearchFilterMode.IST:
                    FilterLabel.Text = "IST Entertainment";
                    songParts = allSongParts.Where(s => s.Artist?.Company == "IST Entertainment").ToObservableCollection(); break;

                case SearchFilterMode.CJ_ENM_Music:
                    FilterLabel.Text = "CJ ENM Music";
                    songParts = allSongParts.Where(s => s.Artist?.Company == "AOMG" ||
                                                        s.Artist?.Company == "B2M Entertainment" ||
                                                        s.Artist?.Company == "Jellyfish Entertainment" ||
                                                        s.Artist?.Company == "Wake One" || // Formerly known as MMO Entertainment
                                                        s.Artist?.Company == "Stone Music Entertainment").ToObservableCollection(); break;
                case SearchFilterMode.Kakao_Entertainment:
                    FilterLabel.Text = "Kakao Entertainment";
                    songParts = allSongParts.Where(s => s.Artist?.Company == "IST Entertainment" || // Went through a lot of renaming: A Cube -> Play A -> PLay M
                                                        s.Artist?.Company == "Starship Entertainment" ||
                                                        s.Artist?.Company == "EDAM Entertainment" ||
                                                        s.Artist?.Company == "Bluedot Entertainment" ||
                                                        s.Artist?.Company == "High Up Entertainment" ||
                                                        s.Artist?.Company == "Bluedot Entertainment" ||
                                                        s.Artist?.Company == "Antenna" ||
                                                        s.Artist?.Company == "FLEX M").ToObservableCollection(); break;

                case SearchFilterMode.Firstgen:
                    FilterLabel.Text = "First gen (< 2002)";
                    songParts = allSongParts.Where(s => s.Artist?.Generation == MainViewModel.FIRST_GENERATION && s.Album?.Language == "KR").ToObservableCollection(); break;
                case SearchFilterMode.Secondgen:
                    FilterLabel.Text = "Second gen (2003 - 2012)";
                    songParts = allSongParts.Where(s => s.Artist?.Generation == MainViewModel.SECOND_GENERATION && s.Album?.Language == "KR").ToObservableCollection(); break;
                case SearchFilterMode.Thirdgen:
                    FilterLabel.Text = "Third gen (2012 - 2017)";
                    songParts = allSongParts.Where(s => s.Artist?.Generation == MainViewModel.THIRD_GENERATION && s.Album?.Language == "KR").ToObservableCollection(); break;
                case SearchFilterMode.Fourthgen:
                    FilterLabel.Text = "Fourth gen (2018 - 2022)";
                    songParts = allSongParts.Where(s => s.Artist?.Generation == MainViewModel.FOURTH_GENERATION && s.Album?.Language == "KR").ToObservableCollection(); break;
                case SearchFilterMode.Fifthgen:
                    FilterLabel.Text = "Fifth gen (2023 >)";
                    songParts = allSongParts.Where(s => s.Artist?.Generation == MainViewModel.FIFTH_GENERATION && s.Album?.Language == "KR").ToObservableCollection(); break;

                case SearchFilterMode.KR:
                    FilterLabel.Text = "K-pop";
                    songParts = allSongParts.Where(s => s.Album?.Language == "KR").ToObservableCollection(); break;
                case SearchFilterMode.JP:
                    FilterLabel.Text = "J-pop";
                    songParts = allSongParts.Where(s => s.Album?.Language == "JP").ToObservableCollection(); break;
                case SearchFilterMode.EN:
                    FilterLabel.Text = "English pop";
                    songParts = allSongParts.Where(s => s.Album?.Language == "EN").ToObservableCollection(); break;
                case SearchFilterMode.CH:
                    FilterLabel.Text = "C-pop";
                    songParts = allSongParts.Where(s => s.Album?.Language == "CH").ToObservableCollection(); break;
                case SearchFilterMode.TH:
                    FilterLabel.Text = "T-pop";
                    songParts = allSongParts.Where(s => s.Album?.Language == "TH").ToObservableCollection(); break;

                case SearchFilterMode.Solo:
                    FilterLabel.Text = "Solo artists";
                    songParts = allSongParts.Where(s => s.Artist?.MemberCount == 1).ToObservableCollection(); break;
                case SearchFilterMode.Duo:
                    FilterLabel.Text = "Duos";
                    songParts = allSongParts.Where(s => s.Artist?.MemberCount == 2).ToObservableCollection(); break;
                case SearchFilterMode.Trio:
                    FilterLabel.Text = "Trios";
                    songParts = allSongParts.Where(s => s.Artist?.MemberCount == 3).ToObservableCollection(); break;
                case SearchFilterMode.Quadruplet:
                    FilterLabel.Text = "Quadruplets";
                    songParts = allSongParts.Where(s => s.Artist?.MemberCount == 4).ToObservableCollection(); break;
                case SearchFilterMode.Quintet:
                    FilterLabel.Text = "Quintets";
                    songParts = allSongParts.Where(s => s.Artist?.MemberCount == 5).ToObservableCollection(); break;
                case SearchFilterMode.Sextet:
                    FilterLabel.Text = "Sextets";
                    songParts = allSongParts.Where(s => s.Artist?.MemberCount == 6).ToObservableCollection(); break;
                case SearchFilterMode.Septet:
                    FilterLabel.Text = "Septets";
                    songParts = allSongParts.Where(s => s.Artist?.MemberCount == 7).ToObservableCollection(); break;
                case SearchFilterMode.Octet:
                    FilterLabel.Text = "Octets";
                    songParts = allSongParts.Where(s => s.Artist?.MemberCount == 8).ToObservableCollection(); break;
                case SearchFilterMode.Nonet:
                    FilterLabel.Text = "Nonets";
                    songParts = allSongParts.Where(s => s.Artist?.MemberCount == 9).ToObservableCollection(); break;
                case SearchFilterMode.Group:
                    FilterLabel.Text = "Groups (2+ members)";
                    songParts = allSongParts.Where(s => s.Artist?.MemberCount > 2).ToObservableCollection(); break;
            }

            songParts.CollectionChanged += SongPartsCollectionChanged;
            SonglibraryListView.ItemsSource = songParts;

            ResultsLabel.Text = $"Currently showing: {songParts.Count} results";
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
        }
    }
    #endregion

    #region Sort
    internal void RefreshSort()
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

                    songParts = songParts.OrderBy(s => s.AlbumTitle).ToObservableCollection();
                    
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
                    songParts = songParts.OrderBy(s => s.ArtistName).ToObservableCollection();

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
                    songParts = songParts.OrderBy(s => s.Artist?.Company).ToObservableCollection();

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
                    // Shows only korean songs, else you have to mess with the group non-kpop which gets in the way of grouping order.
                    songParts = songParts.Where(s => s.Album.Language == "KR").OrderByDescending(s => s.Artist.Gen).ToObservableCollection();
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
                    songParts = songParts.OrderBy(s => s.Artist?.GroupType).ToObservableCollection();

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

                case SortMode.Language:
                    songParts = songParts.OrderBy(s => s.Album?.Language).ToObservableCollection();

                    SonglibraryListView?.DataSource?.GroupDescriptors.Add(new GroupDescriptor()
                    {
                        PropertyName = "Language",
                        KeySelector = (object obj1) =>
                        {
                            var item = (obj1 as SongPart);
                            if (item is not null && item.Album is not null)
                            {
                                return item.Album.Language.ToString();
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
                    songParts = songParts.OrderByDescending(s => s.Artist?.MemberCount).ToObservableCollection();
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
                    songParts = songParts.OrderByDescending(s => s.Album.ReleaseDate).ToObservableCollection();

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
                    songParts = songParts.OrderBy(s => s.Title).ToObservableCollection();

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
                    songParts = songParts.OrderBy(s => s.Album.ReleaseDate.DayOfWeek).ToObservableCollection();

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
                    songParts = songParts.OrderBy(s => s.Album.ReleaseDate.Month).ThenBy(s => s.Album.ReleaseDate.Day).ToObservableCollection();

                    SonglibraryListView.DataSource?.GroupDescriptors.Add(new GroupDescriptor()
                    {
                        PropertyName = "YearlyDate",
                        KeySelector = (object obj1) =>
                        {
                            var item = (obj1 as SongPart);
                            int month = item!.Album.ReleaseDate.Month;
                            int day = item!.Album.ReleaseDate.Day;

                            // Consider whether you want DateTime.UtcNow.Year instead
                            DateTime date = new(DateTime.Now.Year, month, day);
                            return date.ToString("MM/dd");
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
            Dictionary<string, object> dict = new();
            dict.Add("SortMode", MainViewModel.SortMode.ToString());
            dict.Add("SearchFilterMode", MainViewModel.SearchFilterMode.ToString());
            ex.AddSentryContext("data", dict);

            SentrySdk.CaptureException(ex);
        }

        if (SearchBarInput is not null && SearchBarInput.Text is not null && SearchBarInput.Text.Trim().Length > 0)
        {
            SetSearchFilteredDataSource();
        }
        else
        {
            SonglibraryListView!.ItemsSource = songParts;
        }

        songParts.CollectionChanged += SongPartsCollectionChanged;

        SortModeLabel.Text = MainViewModel.SortMode.ToString();
    }

    #endregion
}