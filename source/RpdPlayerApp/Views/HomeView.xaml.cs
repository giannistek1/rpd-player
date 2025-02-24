using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Newtonsoft.Json;
using RpdPlayerApp.Architecture;
using RpdPlayerApp.Items;
using RpdPlayerApp.Managers;
using RpdPlayerApp.Models;
using RpdPlayerApp.Repositories;
using RpdPlayerApp.ViewModels;
using Syncfusion.Maui.Core;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text.Json;
using static Dropbox.Api.Sharing.ListFileMembersIndividualResult;

namespace RpdPlayerApp.Views;

public partial class HomeView : ContentView
{
    internal event EventHandler? PlaySongPart;
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

        LoadInitialData();
        Loaded += OnLoad;
    }

    private void LoadInitialData()
    {
        ArtistRepository.GetArtists();
        AlbumRepository.GetAlbums();
        VideoRepository.GetVideos();
        SongPartRepository.GetSongParts();
    }

    private void OnLoad(object? sender, EventArgs e)
    {
        try
        {
            _ = HandleSongParts();
            SetVersionLabel();
#if RELEASE 
            UniqueSongCountImage.IsVisible = false;
            UniqueSongCountLabel.IsVisible = false;
#endif
            InitializeCompanies();
            InitializeChipGroups();
            SaveNews();
            HandleAutoStartRpd();

            ChipGroupSelectionChanged(null,null);
        }
        catch (Exception ex)
        {
            HelperClass.ShowToast(ex.Message);
        }
    }

    private async Task HandleSongParts()
    {
        var oldSongList = await FileManager.LoadNewsItemsFromFilePath($"{SONGPARTS}.txt");

        if (oldSongList is not null)
        {
            var newSongList = CreateNewsItemsFromSongParts();
            var differentNewSongs = FindDifferentNewSongs(newSongList, oldSongList);

            UpdateNewsBadge(differentNewSongs);
        }
    }

    private List<NewsItem> CreateNewsItemsFromSongParts() => SongPartRepository.SongParts.Select(s => new NewsItem
    {
        Title = s.Title,
        Artist = s.ArtistName,
        Part = s.PartNameFull,
        AudioUrl = s.AudioURL,
        HasVideo = s.HasVideo
    }).ToList();

    private List<NewsItem> FindDifferentNewSongs(List<NewsItem> newSongList, List<NewsItem> oldSongList)
    {
#if RELEASE
        var differentNewSongs = newSongList.Where(item1 => !oldSongList.Any(item2 => item1.AudioUrl == item2.AudioUrl)).ToList();
        foreach (var newSongPart in newSongList)
        {
            if (!newSongPart.HasVideo) continue;

            var oldSong = oldSongList.FirstOrDefault(s => s?.AudioUrl == newSongPart.AudioUrl);
            if (oldSong is not null && oldSong.HasVideo != newSongPart.HasVideo)
            {
                newSongPart.HasNewVideo = true;
            }
        }
        differentNewSongs.AddRange(newSongList.Where(item1 => !oldSongList.Any(item2 => item1.HasVideo == item2.HasVideo)).ToList());
#else
        var differentNewSongs = newSongList.Where(s => s.Artist!.Equals("ATEEZ", StringComparison.OrdinalIgnoreCase)).ToList();
#endif
        return differentNewSongs;
    }

    private void UpdateNewsBadge(List<NewsItem> differentNewSongs)
    {
        if (differentNewSongs.Count > 0)
        {
            NewsBadgeView.BadgeText = differentNewSongs.Count.ToString();
            MainViewModel.SongPartsDifference = differentNewSongs;
        }
        else
        {
            NewsBadgeView.IsVisible = false;
        }
    }

    private void SetVersionLabel()
    {
#if DEBUG
        string releaseMode = "D"; // Debug
#else
        string releaseMode = "P"; // Production
#endif
        VersionLabel.Text = $"v{AppInfo.Current.VersionString}.{AppInfo.Current.BuildString}.{releaseMode}";
    }

    /// <summary> Fills in companies. </summary>
    private void InitializeCompanies()
    {
        MainViewModel.AllCompanies = ArtistRepository.Artists.Select(artist => artist.Company).Distinct().ToList();
        var mainCompanies = MainViewModel.YGCompanies.Concat(MainViewModel.HybeCompanies)
                                                     .Concat(MainViewModel.SMCompanies)
                                                     .Append("JYP Entertainment")
                                                     .ToList();
        RpdSettings!.OtherCompanies = MainViewModel.AllCompanies.Except(mainCompanies).ToList();
    }

    private void InitializeChipGroups()
    {
        InitializeHomeModeSegmentedControl();
        InitializeDurationChipGroup();
        InitializeTimerChipGroup();
        InitializeVoiceAnnouncementsChipGroup();
        InitializeGroupTypesChipGroup();
        InitializeGenresChipGroup();
        InitializeGenerationsChipGroup();
        InitializeCompaniesChipGroup();
        InitializeYearsChipGroup();
        InitializeOtherOptionsChipGroup();
    }

    private void InitializeHomeModeSegmentedControl()
    {
        HomeModeSegmentedControl.ItemsSource = new[] { "Start RPD", "Generate playlist" };
        HomeModeSegmentedControl.SelectedIndex = 0;
        HomeModeSegmentedControl.SelectionChanged += HomeModeSegmentedControl_SelectionChanged;

        StartModeButton.Text = "Start RPD";
        StartModeButton.Clicked += StartModeButtonClicked;
    }

    private void InitializeDurationChipGroup()
    {
        DurationChipGroup?.Items?.Add(new SfChip() { Text = "∞", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        DurationChipGroup!.SelectedItem = DurationChipGroup?.Items?[0];
    }

    private void InitializeTimerChipGroup()
    {
        string[] options = ["Off", "3s", "5s", "Custom (Pro)"];
        foreach (var option in options)
        {
            TimerChipGroup?.Items?.Add(new SfChip() { Text = option, TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        }
        TimerChipGroup!.SelectedItem = TimerChipGroup?.Items?[0];
    }

    private void InitializeVoiceAnnouncementsChipGroup()
    {
        string[] options = ["Off", "Non-(pre-)chorus", "Always"];
        foreach (var option in options)
        {
            VoiceAnnouncementsChipGroup?.Items?.Add(new SfChip() { Text = option, TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        }
        VoiceAnnouncementsChipGroup!.SelectedItem = VoiceAnnouncementsChipGroup?.Items?[0];
    }

    private void InitializeGroupTypesChipGroup()
    {
        GrouptypesChipGroup?.Items?.Add(new SfChip() { Text = "Male", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        GrouptypesChipGroup?.Items?.Add(new SfChip() { Text = "Female", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        GrouptypesChipGroup?.Items?.Add(new SfChip() { Text = "Mixed", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        GrouptypesChipGroup!.SelectedItem = new ObservableCollection<SfChip>(GrouptypesChipGroup.Items!);
        GrouptypesChipGroup.SelectionChanged += ChipGroupSelectionChanged;
    }

    private void InitializeGenresChipGroup()
    {
        string[] options = ["K-pop", "K-RnB", "J-pop", "C-pop", "T-pop", "Pop"];
        foreach (var option in options)
        {
            GenresChipGroup?.Items?.Add(new SfChip() { Text = option, TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        }
        GenresChipGroup!.SelectedItem = new ObservableCollection<SfChip>(GenresChipGroup.Items!);
        GenresChipGroup.SelectionChanged += GenresChipGroup_SelectionChanged;
        GenresChipGroup.SelectionChanged += ChipGroupSelectionChanged;
    }

    private void InitializeGenerationsChipGroup()
    {
        string[] options = ["1", "2", "3", "4", "5", "Non-kpop"];
        foreach (var option in options)
        {
            GenerationsChipGroup?.Items?.Add(new SfChip() { Text = option, TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        }
        GenerationsChipGroup!.SelectedItem = new ObservableCollection<SfChip>(GenerationsChipGroup.Items!);
        GenerationsChipGroup.SelectionChanged += ChipGroupSelectionChanged;
    }

    private void InitializeCompaniesChipGroup()
    {
        string[] options = ["SM", "HYBE", "JYP", "YG", "Others"];
        foreach (var option in options)
        {
            CompaniesChipGroup?.Items?.Add(new SfChip() { Text = option, TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        }
        CompaniesChipGroup!.SelectedItem = new ObservableCollection<SfChip>(CompaniesChipGroup.Items!);
        CompaniesChipGroup.SelectionChanged += ChipGroupSelectionChanged;
    }

    private void InitializeYearsChipGroup()
    {
        List<string> options = ["< 2012"];
        for(int year = 2012; year <= 2025; year++)
        {
            options.Add(year.ToString());
        }
        foreach (var option in options)
        {
            YearsChipGroup?.Items?.Add(new SfChip() { Text = option, TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        }
        YearsChipGroup!.SelectedItem = new ObservableCollection<SfChip>(YearsChipGroup.Items!);
        YearsChipGroup.SelectionChanged += ChipGroupSelectionChanged;
    }

    private void InitializeOtherOptionsChipGroup()
    {
        var customChips = new ObservableCollection<CustomChipModel>
        {
            new() { Name = "Last chorus" },
            new() { Name = "Dance breaks" },
            new() { Name = "Tiktoks" }
        };
        OtherOptionsChipGroup.ItemsSource = customChips;
        OtherOptionsChipGroup.SelectionChanged += ChipGroupSelectionChanged;
    }

    private void HandleAutoStartRpd()
    {
        if (Preferences.ContainsKey(CommonSettings.START_RPD_AUTOMATIC))
        {
            bool startRpd = Preferences.Get(CommonSettings.START_RPD_AUTOMATIC, false);
            if (startRpd) StartRpdButtonClicked(this, EventArgs.Empty);
        }
    }
    // TODO: Needs rework to be used for Windows using a txt file.
    private static void SaveNews()
    {
        var newsItems = SongPartRepository.SongParts.Select(s => new NewsItem
        {
            Title = s.Title,
            Artist = s.ArtistName,
            Part = s.PartNameFull,
            AudioUrl = s.AudioURL,
            HasVideo = s.HasVideo
        }).ToList();

        var jsonSongParts = JsonConvert.SerializeObject(newsItems);
        FileManager.SaveJsonToFileAsync($"{SONGPARTS}.txt", jsonSongParts);
    }

    private void ChipGroupSelectionChanged(object? sender, Syncfusion.Maui.Core.Chips.SelectionChangedEventArgs? e)
    {
        RpdSettings?.DetermineGroupTypes(GrouptypesChipGroup);
        RpdSettings?.DetermineGenres(GenresChipGroup);
        RpdSettings?.DetermineGens(GenerationsChipGroup);
        RpdSettings?.DetermineCompanies(CompaniesChipGroup);
        RpdSettings?.DetermineYears(YearsChipGroup);

        RpdSettings?.NumberedPartsBlacklist.Clear();
        RpdSettings?.PartsBlacklist.Clear();

        ApplyAntiOptions();

        var songParts = FilterSongParts();

        RpdSizeLabel.Text = $"{songParts.Count}";
        IEnumerable<SongPart> artistsBySongPart = songParts.DistinctBy(s => new { s.ArtistName });
        RpdArtistSizeLabel.Text = $"{artistsBySongPart.Count()}";
    }

    private void GenresChipGroup_SelectionChanged(object? sender, Syncfusion.Maui.Core.Chips.SelectionChangedEventArgs e)
    {
        GenerationsGrid.IsVisible = GenresChipGroup.Items![0].IsSelected;
        CompaniesGrid.IsVisible = GenresChipGroup.Items![0].IsSelected;
    }

    internal void RefreshThemeColors()
    {
        RefreshChipGroupColors(DurationChipGroup);
        RefreshChipGroupColors(GrouptypesChipGroup);
        RefreshChipGroupColors(GenresChipGroup);
        RefreshChipGroupColors(GenerationsChipGroup);
        RefreshChipGroupColors(CompaniesChipGroup);
        RefreshChipGroupColors(YearsChipGroup);

        OtherOptionsChipGroup.ItemsSource = null!;
        var customChips = new ObservableCollection<CustomChipModel>
        {
            new() { Name = "Last chorus" },
            new() { Name = "Dance breaks" },
            new() { Name = "Tiktoks" }
        };
        OtherOptionsChipGroup.ItemsSource = customChips;
    }

    private void RefreshChipGroupColors(SfChipGroup chipGroup)
    {
        chipGroup.SelectedChipBackground = (Color)Application.Current!.Resources["SecondaryButton"];
        foreach (var chip in chipGroup.Items!)
        {
            chip.TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"];
        }
        chipGroup.SelectedItem = null;
        chipGroup.SelectedItem = new ObservableCollection<SfChip>(chipGroup.Items!);
    }

    private void OnSelectionChanged(object sender, Syncfusion.Maui.Core.Chips.SelectionChangedEventArgs e)
    {
        if (e.AddedItem is not null) ((CustomChipModel)e.AddedItem).IsSelected = true;
        if (e.RemovedItem is not null) ((CustomChipModel)e.RemovedItem).IsSelected = false;
    }

    private void SongPartsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => SongPartCountLabel.Text = $"{SongPartRepository.SongParts.Count}";

    private void ArtistsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => ArtistCountLabel.Text = $"{ArtistRepository.Artists.Count}";

    private void AlbumsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => AlbumCountLabel.Text = $"{AlbumRepository.Albums.Count}";

    internal void FeedbackButtonPressed(object? sender, EventArgs e){ /* TODO: */ }

    internal async void SettingsButtonPressed(object? sender, EventArgs e)
    {
        if (Navigation.NavigationStack.Count < 2)
        {
            await Navigation.PushAsync(_settingsPage, true);
            _settingsPage.HomeView = this;
        }
    }

    private void NewsImageButton_Pressed(object sender, EventArgs e) => ShowNewsPopup?.Invoke(sender, e);

    private void CreatePlaylistButtonClicked(object sender, EventArgs e) => CreatePlaylistButtonPressed?.Invoke(sender, e);

    private void SearchByCategoryButtonClicked(object sender, EventArgs e) => ShowCategories?.Invoke(sender, e);

    private void HomeModeSegmentedControl_SelectionChanged(object? sender, Syncfusion.Maui.Buttons.SelectionChangedEventArgs e)
    {
        RpdSettings!.UsingGeneratePlaylist = (HomeModeSegmentedControl.SelectedIndex == 1);
        StartModeButton.Text = RpdSettings.UsingGeneratePlaylist ? "Generate playlist" : "Start RPD";

        DurationChipGroup!.Items!.Clear();
        if (RpdSettings.UsingGeneratePlaylist)
        {
            AddChipsToDurationChipGroup(["2.5", "2", "1.5", "1", "0.5", "Other"]);
        }
        else
        {
            AddChipsToDurationChipGroup(["∞"]);
            DurationChipGroup.SelectedItem = DurationChipGroup.Items[0];
        }
    }

    private void AddChipsToDurationChipGroup(string[] chipTexts)
    {
        foreach (var text in chipTexts)
        {
            DurationChipGroup?.Items?.Add(new SfChip() { Text = text, TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        }
    }

    private void StartModeButtonClicked(object? sender, EventArgs e)
    {
        if (RpdSettings!.UsingGeneratePlaylist) GeneratePlaylistButtonClicked();
        else StartRpdButtonClicked(sender, e);
    }

    private void GeneratePlaylistButtonClicked()
    {
        var newNewsItems = CreateNewsItemsFromSongParts();
        var differentNewSongs = newNewsItems.Where(s => s.Artist!.Equals("ATEEZ", StringComparison.OrdinalIgnoreCase)).ToList();
        foreach (var item in differentNewSongs)
        {
            item.HasNewVideo = Convert.ToBoolean(HelperClass.Rng.Next(2));
        }

        UpdateNewsBadge(differentNewSongs);
        HelperClass.ShowToast("Not implemented yet!");
    }

    private void StartRpdButtonClicked(object? sender, EventArgs e)
    {
        if (!HelperClass.HasInternetConnection()) return;

        SetTimerMode();
        RpdSettings?.DetermineGroupTypes(GrouptypesChipGroup);
        RpdSettings?.DetermineGenres(GenresChipGroup);
        RpdSettings?.DetermineGens(GenerationsChipGroup);
        RpdSettings?.DetermineCompanies(CompaniesChipGroup);
        RpdSettings?.DetermineYears(YearsChipGroup);

        RpdSettings?.NumberedPartsBlacklist.Clear();
        RpdSettings?.PartsBlacklist.Clear();

        ApplyAntiOptions();

        var songParts = FilterSongParts();
        if (songParts.Count <= 0)
        {
            HelperClass.ShowToast("No songs found! Please change your settings.");
            return;
        }

        MainViewModel.SongParts = songParts;
        PlayRandomSong(songParts);
    }

    private void SetTimerMode()
    {
        for (byte i = 0; i < TimerChipGroup.Items!.Count; i++)
        {
            if (TimerChipGroup.Items[i].IsSelected)
            {
                MainViewModel.TimerMode = i;
                break;
            }
        }
    }

    private void ApplyAntiOptions()
    {
        var antiOptionsItems = OtherOptionsChipGroup.ItemsSource as ObservableCollection<CustomChipModel>;
        foreach (var item in antiOptionsItems!)
        {
            if (item.IsSelected)
            {
                switch (item.Name)
                {
                    case "Last chorus":
                        RpdSettings?.NumberedPartsBlacklist.AddRange(["CE2", "C3", "CDB3", "CE3", "CE2", "P3", "PDB3"]);
                        break;

                    case "Dance breaks":
                        RpdSettings?.PartsBlacklist.AddRange(["CDB", "CDBE", "DB", "DBC", "DBE", "DBO", "PDB", "B", "O"]);
                        break;

                    case "Tiktoks":
                        RpdSettings?.PartsBlacklist.Add("T");
                        break;
                }
            }
        }
    }

    private List<SongPart> FilterSongParts()
    {
        var songParts = SongPartRepository.SongParts.Where(s => RpdSettings!.GroupTypes.Contains(s.Artist.GroupType))
                                                    .Where(s => RpdSettings!.Genres.Contains(s.Album.GenreFull))
                                                    .Where(s => !RpdSettings!.NumberedPartsBlacklist.Contains(s.PartNameShortWithNumber))
                                                    .Where(s => !RpdSettings!.PartsBlacklist.Contains(s.PartNameShort))
                                                    .Where(s => RpdSettings!.Years.Contains(s.Album.ReleaseDate.Year))
                                                    .ToList();

        if (GenresChipGroup!.Items![0].IsSelected)
        {
            songParts = songParts.Where(s => RpdSettings!.Gens.Contains(s.Artist.Gen))
                                 .Where(s => RpdSettings!.Companies.Contains(s.Artist.Company)).ToList();
        }

        return songParts;
    }

    private void PlayRandomSong(List<SongPart> songParts)
    {
        int index = HelperClass.Rng.Next(songParts.Count);
        SongPart songPart = songParts[index];

        MainViewModel.AutoplayMode = 2; // Shuffle
        MainViewModel.CurrentSongPart = songPart;
        PlaySongPart?.Invoke(this, EventArgs.Empty);
    }
}