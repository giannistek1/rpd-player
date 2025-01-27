using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Newtonsoft.Json;
using RpdPlayerApp.Architecture;
using RpdPlayerApp.Models;
using RpdPlayerApp.Repositories;
using RpdPlayerApp.ViewModels;
using Syncfusion.Maui.Core;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text.Json;
using static Dropbox.Api.Sharing.ListFileMembersIndividualResult;

namespace RpdPlayerApp.Views;

public partial class HomeView : ContentView
{
    internal event EventHandler? PlaySongPart;
    internal event EventHandler? FilterPressed;
    internal event EventHandler? CreatePlaylistButtonPressed;
    internal event EventHandler? ShowCategories;
    internal event EventHandler? ShowNewsPopup;

    private readonly SettingsPage _settingsPage = new();
    internal MainPage? ParentPage { get; set; }
    internal RpdSettings? RpdSettings { get; set; } = new();

    private const string SONGPARTS = "SONGPARTS";

    public HomeView()
    {
        InitializeComponent();


        ArtistRepository.Artists.CollectionChanged += ArtistsCollectionChanged;
        AlbumRepository.Albums.CollectionChanged += AlbumsCollectionChanged;
        SongPartRepository.SongParts.CollectionChanged += SongPartsCollectionChanged;

        // Get data here because of collectionChanged.
        ArtistRepository.GetArtists();
        AlbumRepository.GetAlbums();
        VideoRepository.GetVideos();
        SongPartRepository.GetSongParts();

        this.Loaded += OnLoad;
    }

    private void OnLoad(object? sender, EventArgs e)
    {
        try
        {
            if (Preferences.ContainsKey(SONGPARTS))
            {
                string songs = Preferences.Get(SONGPARTS, string.Empty);
                var oldSongList = JsonConvert.DeserializeObject<List<NewsItem>>(songs);
                if (oldSongList is not null)
                {
                    List<NewsItem> newNewsItems = SongPartRepository.SongParts.Select(s => new NewsItem()
                    {
                        Title = s.Title,
                        Artist = s.ArtistName,
                        Part = s.PartNameFull,
                        AudioUrl = s.AudioURL,
                        HasVideo = s.HasVideo
                    }).ToList();

#if RELEASE
                    var differences = newNewsItems.Where(item1 => !oldSongList.Any(item2 => item1.AudioUrl == item2.AudioUrl)).ToList();
#else
                    var differences = newNewsItems.Where(s => s.Artist.Equals("ATEEZ", StringComparison.OrdinalIgnoreCase)).ToList();
#endif

                    if (differences.Count > 0)
                    {
                        NewsBadgeView.BadgeText = differences.Count.ToString();
                        MainViewModel.SongPartsDifference = differences;
                    }
                    else
                    {
                        NewsBadgeView.IsVisible = false;
                    }
                }
            }
        }
        catch(Exception ex)
        {
            Toast.Make(ex.Message).Show();
        }

        // Version
#if DEBUG
        string releaseMode = "D"; // Debug
#else
        string releaseMode = "P"; // Production
#endif
        VersionLabel.Text = $"v{AppInfo.Current.VersionString}.{AppInfo.Current.BuildString}.{releaseMode}";

        // Fill other companies.
        MainViewModel.AllCompanies = ArtistRepository.Artists.Select(artist => artist.Company).Distinct().ToList();
        List<string> mainCompanies = [];
        mainCompanies.AddRange(MainViewModel.YGCompanies);
        mainCompanies.AddRange(MainViewModel.HybeCompanies);
        mainCompanies.AddRange(MainViewModel.SMCompanies);
        mainCompanies.Add("JYP Entertainment");
        RpdSettings!.OtherCompanies = MainViewModel.AllCompanies.Except(mainCompanies).ToList();

        var groupedTitles = from s in SongPartRepository.SongParts
                            group s.Title by s.Title into g
                            select new { Title = g.Key, Titles = g.ToList() };
#if DEBUG
        UniqueSongCountLabel.Text = $"{groupedTitles.Count()}";
#endif
#if RELEASE
        UniqueSongCountLabel.IsVisible = false;
        UniqueSongCountImage.IsVisible = false;
#endif

        HomeModeSegmentedControl.ItemsSource = new string[] { "Start RPD", "Generate playlist" };
        HomeModeSegmentedControl.SelectedIndex = 0;
        HomeModeSegmentedControl.SelectionChanged += HomeModeSegmentedControl_SelectionChanged;

        StartModeButton.Text = "Start RPD";
        StartModeButton.Clicked += StartModeButtonClicked;

        DurationChipGroup?.Items?.Add(new SfChip() { Text = "∞", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        //DurationChipGroup?.Items?.Add(new SfChip() { Text = "2H", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        //DurationChipGroup?.Items?.Add(new SfChip() { Text = "1H", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        //DurationChipGroup?.Items?.Add(new SfChip() { Text = "Other", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        DurationChipGroup!.SelectedItem = DurationChipGroup?.Items?[0];

        // TODO: string list of grouptypes and loop
        GrouptypesChipGroup?.Items?.Add(new SfChip() { Text = "Male", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        GrouptypesChipGroup?.Items?.Add(new SfChip() { Text = "Female", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        GrouptypesChipGroup?.Items?.Add(new SfChip() { Text = "Mixed", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        GrouptypesChipGroup!.SelectedItem = new ObservableCollection<SfChip>(GrouptypesChipGroup.Items!);

        // TODO: string list of genres and loop
        GenresChipGroup?.Items?.Add(new SfChip() { Text = "K-pop", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        GenresChipGroup?.Items?.Add(new SfChip() { Text = "K-RnB", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        GenresChipGroup?.Items?.Add(new SfChip() { Text = "J-pop", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        GenresChipGroup?.Items?.Add(new SfChip() { Text = "C-pop", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        GenresChipGroup?.Items?.Add(new SfChip() { Text = "T-pop", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        GenresChipGroup?.Items?.Add(new SfChip() { Text = "Pop", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        GenresChipGroup!.SelectedItem = new ObservableCollection<SfChip>(GenresChipGroup.Items!);
        GenresChipGroup.SelectionChanged += GenresChipGroup_SelectionChanged;

        // TODO: string list of gens and loop (only with kpop)
        GenerationsChipGroup?.Items?.Add(new SfChip() { Text = "1", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        GenerationsChipGroup?.Items?.Add(new SfChip() { Text = "2", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        GenerationsChipGroup?.Items?.Add(new SfChip() { Text = "3", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        GenerationsChipGroup?.Items?.Add(new SfChip() { Text = "4", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        GenerationsChipGroup?.Items?.Add(new SfChip() { Text = "5", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        GenerationsChipGroup?.Items?.Add(new SfChip() { Text = "Non-kpop", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        GenerationsChipGroup!.SelectedItem = new ObservableCollection<SfChip>(GenerationsChipGroup.Items!);

        // TODO: string list of companies and loop (only with kpop)
        CompaniesChipGroup?.Items?.Add(new SfChip() { Text = "SM", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        CompaniesChipGroup?.Items?.Add(new SfChip() { Text = "HYBE", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        CompaniesChipGroup?.Items?.Add(new SfChip() { Text = "JYP", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        CompaniesChipGroup?.Items?.Add(new SfChip() { Text = "YG", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        CompaniesChipGroup?.Items?.Add(new SfChip() { Text = "Others", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        CompaniesChipGroup!.SelectedItem = new ObservableCollection<SfChip>(CompaniesChipGroup.Items!);

        // TODO: string list of options
        var customChips = new ObservableCollection<CustomChipModel>
        {
            new() { Name = "Last chorus" },
            new() { Name = "Dance breaks" },
            new() { Name = "Tiktoks" }
        };
        OtherOptionsChipGroup.ItemsSource = customChips;
        // Only if playlist option:
        //OtherOptionsChipGroup?.Items?.Add(new SfChip() { Text = "Ending with chorus 3", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });

        List<NewsItem> newsItems = SongPartRepository.SongParts.Select(s => new NewsItem()
        {
            Title = s.Title,
            Artist = s.ArtistName,
            Part = s.PartNameFull,
            AudioUrl = s.AudioURL,
            HasVideo = s.HasVideo
        }).ToList();

        var jsonSongParts = JsonConvert.SerializeObject(newsItems);
        Preferences.Set(SONGPARTS, jsonSongParts);
    }

    private void GenresChipGroup_SelectionChanged(object? sender, Syncfusion.Maui.Core.Chips.SelectionChangedEventArgs e)
    {
        // K-pop only.
        GenerationsGrid.IsVisible = GenresChipGroup.Items![0].IsSelected;
        CompaniesGrid.IsVisible = GenresChipGroup.Items![0].IsSelected;
    }

    internal void RefreshThemeColors()
    {
        DurationChipGroup.SelectedChipBackground = (Color)Application.Current!.Resources["SecondaryButton"];
        DurationChipGroup.Items![0].TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"];
        DurationChipGroup.SelectedItem = null;
        DurationChipGroup!.SelectedItem = DurationChipGroup.Items[0];

        // TODO: ObservableCollection should be optimized.
        GrouptypesChipGroup.SelectedChipBackground = (Color)Application.Current!.Resources["SecondaryButton"];
        foreach (var grouptype in GrouptypesChipGroup.Items!)
        {
            grouptype.TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"];
        }
        GrouptypesChipGroup.SelectedItem = null;
        GrouptypesChipGroup!.SelectedItem = new ObservableCollection<SfChip>(GrouptypesChipGroup?.Items!);

        GenresChipGroup.SelectedChipBackground = (Color)Application.Current!.Resources["SecondaryButton"];
        foreach (var genre in GenresChipGroup.Items!)
        {
            genre.TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"];
        }
        GenresChipGroup.SelectedItem = null;
        GenresChipGroup!.SelectedItem = new ObservableCollection<SfChip>(GenresChipGroup?.Items!);

        GenerationsChipGroup.SelectedChipBackground = (Color)Application.Current!.Resources["SecondaryButton"];
        foreach (var generation in GenerationsChipGroup.Items!)
        {
            generation.TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"];
        }
        GenerationsChipGroup.SelectedItem = null;
        GenerationsChipGroup!.SelectedItem = new ObservableCollection<SfChip>(GenerationsChipGroup?.Items!);

        CompaniesChipGroup.SelectedChipBackground = (Color)Application.Current!.Resources["SecondaryButton"];
        foreach (var company in CompaniesChipGroup.Items!)
        {
            company.TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"];
        }
        CompaniesChipGroup.SelectedItem = null;
        CompaniesChipGroup!.SelectedItem = new ObservableCollection<SfChip>(CompaniesChipGroup?.Items!);

        OtherOptionsChipGroup.ItemsSource = null!;
        var customChips = new ObservableCollection<CustomChipModel>
        {
            new() { Name = "Last chorus" },
            new() { Name = "Dance breaks" },
            new() { Name = "Tiktoks" }
        };
        OtherOptionsChipGroup.ItemsSource = customChips;
    }

    private void OnSelectionChanged(object sender, Syncfusion.Maui.Core.Chips.SelectionChangedEventArgs e)
    {
        if (e.AddedItem is not null)
        {
            (e.AddedItem as CustomChipModel)!.IsSelected = true;
        }

        if (e.RemovedItem is not null)
        {
            (e.RemovedItem as CustomChipModel)!.IsSelected = false;
        }
    }

    private void SongPartsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => SongPartCountLabel.Text = $"{SongPartRepository.SongParts.Count}";

    private void ArtistsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => ArtistCountLabel.Text = $"{ArtistRepository.Artists.Count}";

    private void AlbumsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => AlbumCountLabel.Text = $"{AlbumRepository.Albums.Count}";

    internal void FeedbackButtonPressed(object? sender, EventArgs e)
    {

    }

    internal async void SettingsButtonPressed(object? sender, EventArgs e)
    {
        if (Navigation.NavigationStack.Count < 2)
        {
            await Navigation.PushAsync(_settingsPage, true);
            _settingsPage.HomeView = this;
        }
    }
    private void CreatePlaylistButtonClicked(object sender, EventArgs e) => CreatePlaylistButtonPressed?.Invoke(sender, e);
    private void SearchByCategoryButtonClicked(object sender, EventArgs e) => ShowCategories?.Invoke(sender, e);
    private void HomeModeSegmentedControl_SelectionChanged(object? sender, Syncfusion.Maui.Buttons.SelectionChangedEventArgs e)
    {
        RpdSettings!.UsingGeneratePlaylist = (HomeModeSegmentedControl.SelectedIndex == 1);
        StartModeButton.Text = (RpdSettings!.UsingGeneratePlaylist) ? "Generate playlist" : "Start RPD";

        DurationChipGroup!.Items!.Clear();
        if (RpdSettings.UsingGeneratePlaylist)
        {
            //DurationChipGroup?.Items?.Add(new SfChip() { Text = "∞", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
            DurationChipGroup?.Items?.Add(new SfChip() { Text = "2H", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
            DurationChipGroup?.Items?.Add(new SfChip() { Text = "1H", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
            DurationChipGroup?.Items?.Add(new SfChip() { Text = "Other", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
            //DurationChipGroup!.SelectedItem = DurationChipGroup?.Items?[0];
        }
        else
        {
            DurationChipGroup?.Items?.Add(new SfChip() { Text = "∞", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
            DurationChipGroup!.SelectedItem = DurationChipGroup?.Items?[0];
        }

    }
    private void StartModeButtonClicked(object? sender, EventArgs e)
    {
        if (RpdSettings!.UsingGeneratePlaylist) { GeneratePlaylistButtonClicked(); }
        else { StartRpdButtonClicked(sender, e); }
    }
    private void GeneratePlaylistButtonClicked() => Toast.Make($"Not implemented yet!", ToastDuration.Short, 14).Show();

    private void StartRpdButtonClicked(object? sender, EventArgs e)
    {
        if (!HelperClass.HasInternetConnection())
            return;

        RpdSettings?.GroupTypes.Clear();
        for (var i = 0; i < GrouptypesChipGroup?.Items?.Count; i++)
        {
            if (GrouptypesChipGroup.Items[i].IsSelected)
            {
                GroupType groupType = GrouptypesChipGroup.Items[i].Text switch
                {
                    "Male" => GroupType.BG,
                    "Female" => GroupType.GG,
                    "Mixed" => GroupType.MIX,
                    _ => GroupType.NOT_SET
                };
                RpdSettings?.GroupTypes.Add(groupType);
            }
        }

        RpdSettings?.Genres.Clear();
        for (var i = 0; i < GenresChipGroup?.Items?.Count; i++)
        {
            if (GenresChipGroup.Items[i].IsSelected)
            {
                string genre = GenresChipGroup.Items[i].Text;
                RpdSettings?.Genres.Add(genre);
            }
        }

        RpdSettings?.Gens.Clear();
        for (var i = 0; i < GenerationsChipGroup?.Items?.Count; i++)
        {
            if (GenerationsChipGroup.Items[i].IsSelected)
            {
                Gen gen = GenerationsChipGroup.Items[i].Text switch
                {
                    "1" => Gen.First,
                    "2" => Gen.Second,
                    "3" => Gen.Third,
                    "4" => Gen.Fourth,
                    "5" => Gen.Fifth,
                    _ => Gen.NotKpop
                };
                RpdSettings?.Gens.Add(gen);
            }
        }

        RpdSettings?.Companies.Clear();
        for (var i = 0; i < CompaniesChipGroup?.Items?.Count; i++)
        {
            if (CompaniesChipGroup.Items[i].IsSelected)
            {
                switch (CompaniesChipGroup.Items[i].Text)
                {
                    case "SM": RpdSettings?.Companies.AddRange(MainViewModel.SMCompanies); break;
                    case "HYBE": RpdSettings?.Companies.AddRange(MainViewModel.HybeCompanies); break;
                    case "JYP": RpdSettings?.Companies.Add("JYP Entertainment"); break;
                    case "YG": RpdSettings?.Companies.AddRange(MainViewModel.HybeCompanies); break;
                    case "Others": RpdSettings?.Companies.AddRange(RpdSettings.OtherCompanies); break;
                }
            }
        }

        RpdSettings?.NumberedPartsBlacklist.Clear();
        RpdSettings?.PartsBlacklist.Clear();

        ObservableCollection<CustomChipModel>? antiOptionsItems = OtherOptionsChipGroup.ItemsSource as ObservableCollection<CustomChipModel>;
        for (var i = 0; i < antiOptionsItems?.Count; i++)
        {
            if (antiOptionsItems[i].IsSelected)
            {
                switch (antiOptionsItems[i].Name)
                {
                    case "Last chorus": RpdSettings?.NumberedPartsBlacklist.AddRange(["CE2", "C3", "CDB3", "CE3", "CE2", "P3", "PDB3"]); break;
                    case "Dance breaks": RpdSettings?.PartsBlacklist.AddRange(["CDB", "CDBE", "DB", "DBC", "DBE", "DBO", "PDB", "B", "O"]); break;
                    case "Tiktoks": RpdSettings?.PartsBlacklist.Add("T"); break;
                }
            }
        }
        // Apply RPD settings.
        var songParts = SongPartRepository.SongParts.Where(s => RpdSettings!.GroupTypes.Contains(s.Artist.GroupType))
                                                    .Where(s => RpdSettings!.Genres.Contains(s.Album.GenreFull))
                                                    .Where(s => !RpdSettings!.NumberedPartsBlacklist.Contains(s.PartNameShortWithNumber))
                                                    .Where(s => !RpdSettings!.PartsBlacklist.Contains(s.PartNameShort)).ToList();
        // K-pop only.
        if (GenresChipGroup!.Items![0].IsSelected)
        {
            songParts = songParts.Where(s => RpdSettings!.Gens.Contains(s.Artist.Gen))
                                 .Where(s => RpdSettings!.Companies.Contains(s.Artist.Company)).ToList();
        }



        // Guard
        if (songParts.Count <= 0) { Toast.Make($"No songs found! Please change settings.", ToastDuration.Short, 14).Show(); return; }

        // Set current songs.
        MainViewModel.SongParts = songParts;

        // Choose random song.
        var random = new Random();
        int index = random.Next(songParts.Count);
        SongPart songPart = songParts[index];

        MainViewModel.AutoplayMode = 2; // Shuffle
        MainViewModel.CurrentSongPart = songPart;
        PlaySongPart?.Invoke(sender, EventArgs.Empty);
    }

    private void NewsImageButton_Pressed(object sender, EventArgs e)
    {
        ShowNewsPopup?.Invoke(sender, e);
    }
}