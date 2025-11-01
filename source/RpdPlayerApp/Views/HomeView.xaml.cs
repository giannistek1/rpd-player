using Newtonsoft.Json;
using RpdPlayerApp.Architecture;
using RpdPlayerApp.Enums;
using RpdPlayerApp.Items;
using RpdPlayerApp.Managers;
using RpdPlayerApp.Models;
using RpdPlayerApp.Repositories;
using RpdPlayerApp.Services;
using Syncfusion.Maui.Core;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace RpdPlayerApp.Views;

public partial class HomeView : ContentView
{
    //internal event EventHandler? PlaySongPart;
    internal event EventHandler? CreatePlaylistButtonPressed;
    internal event EventHandler? ShowCategories;
    internal event EventHandler? ShowNewsPopup;

    private readonly SettingsPage _settingsPage = new();
    private readonly SongSegmentRequestPage _feedbackPage = new();
    internal MainPage? ParentPage { get; set; }
    internal RpdSettings? RpdSettings { get; set; } = new();

    public HomeView()
    {
        InitializeComponent();

        ArtistRepository.Artists.CollectionChanged += ArtistsCollectionChanged;
        AlbumRepository.Albums.CollectionChanged += AlbumsCollectionChanged;
        SongPartRepository.SongParts.CollectionChanged += SongPartsCollectionChanged;

        LoadInitialData();
        Loaded += OnLoad;
    }

    private static void LoadInitialData()
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
            _ = HandleSongPartsDifference();
            SetVersionLabel();
#if RELEASE 
            UniqueSongCountImage.IsVisible = false;
            UniqueSongCountLabel.IsVisible = false;
#endif
            InitializeCompanies();
            InitializeChipGroups();
            _ = NewsManager.SaveNews().ConfigureAwait(false);
            HandleAutoStartRpd();

            ChipGroupSelectionChanged(null, null);

            IsOnlineImage.IsVisible = General.HasInternetConnection() && !Constants.APIKEY.IsNullOrWhiteSpace() && !Constants.BASE_URL.IsNullOrWhiteSpace();
        }
        catch (Exception ex)
        {
            DebugService.Instance.Debug(ex.Message);
            General.ShowToast(ex.Message);
        }
    }

    private async Task HandleSongPartsDifference()
    {
        var oldSongList = await FileManager.LoadNewsItemsFromFilePath($"{NewsManager.SONGPARTS}.txt");

        if (oldSongList is not null)
        {
            var newSongList = NewsManager.CreateNewsItemsFromSongParts();
            var differentNewSongs = FindDifferentNewSongs(newSongList, oldSongList);

            UpdateNewsBadge(differentNewSongs);
        }
    }

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

    internal void UpdateNewsBadge(List<NewsItem> differentNewSongs)
    {
        if (differentNewSongs.Count > 0)
        {
            NewsManager.SongPartsDifference = differentNewSongs;

            NewsBadgeView.BadgeText = differentNewSongs.Count.ToString();
            NewsBadgeView.IsVisible = true; // Make clickable
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
        VersionLabel.Text = $"v{AppInfo.Current.VersionString}-{AppInfo.Current.BuildString}{releaseMode}";
    }

    /// <summary> Fills in companies. </summary>
    private void InitializeCompanies()
    {
        Constants.AllCompanies = ArtistRepository.Artists.Select(artist => artist.Company).Distinct().ToList();
        var mainCompanies = Constants.YGCompanies.Concat(Constants.HybeCompanies)
                                                     .Concat(Constants.SMCompanies)
                                                     .Append("JYP Entertainment")
                                                     .ToList();
        RpdSettings!.OtherCompanies = Constants.AllCompanies.Except(mainCompanies).ToList();
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
        InitializeAntiOptionsChipGroup();
    }

    private void InitializeHomeModeSegmentedControl()
    {
        HomeModeSegmentedControl.ItemsSource = new[] { "Start RPD", "Generate playlist" };
        HomeModeSegmentedControl.SelectedIndex = 0;
        HomeModeSegmentedControl.SelectionChanged += HomeModeSegmentedControlSelectionChanged;

        StartModeButton.Text = "Start RPD";
        StartModeButton.ImageSource = IconManager.PlayIcon;
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

        // Load preference/setting.
        if (Preferences.ContainsKey(CommonSettings.HOME_TIMER))
        {
            var selectedIndex = LoadTagAsInt(CommonSettings.HOME_TIMER);

            TimerChipGroup!.SelectedItem = TimerChipGroup!.Items![selectedIndex];
        }
        else
        {
            TimerChipGroup!.SelectedItem = TimerChipGroup?.Items?[0];
        }
    }

    private void InitializeVoiceAnnouncementsChipGroup()
    {
        string[] options = ["Off", "Non-(pre-)chorus", "AlwaysSongPart"];
        foreach (var option in options)
        {
            VoiceAnnouncementsChipGroup?.Items?.Add(new SfChip() { Text = option, TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        }

        // Load preference/setting.
        if (Preferences.ContainsKey(CommonSettings.HOME_VOICES))
        {
            var selectedIndex = LoadTagAsInt(CommonSettings.HOME_VOICES);

            VoiceAnnouncementsChipGroup!.SelectedItem = VoiceAnnouncementsChipGroup!.Items![selectedIndex];
        }
        else
        {
            VoiceAnnouncementsChipGroup!.SelectedItem = VoiceAnnouncementsChipGroup?.Items?[0];
        }
    }

    string[] _groupTypeOptions = ["Male", "Female", "Mixed"];
    private void InitializeGroupTypesChipGroup()
    {
        // Fill options.
        foreach (var option in _groupTypeOptions)
        {
            GrouptypesChipGroup?.Items?.Add(new SfChip() { Text = option, TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        }

        // Load preference/setting.
        if (Preferences.ContainsKey(CommonSettings.HOME_GROUPTYPES))
        {
            var selectedIndices = LoadTagsAsIntArray(CommonSettings.HOME_GROUPTYPES); // [2,3,4,5]

            var selectedItems = new ObservableCollection<SfChip>();
            for (var i = 0; i < selectedIndices!.Length; i++)
            {
                selectedItems.Add(GrouptypesChipGroup!.Items![selectedIndices[i]]);
            }
            GrouptypesChipGroup!.SelectedItem = selectedItems;
        }
        else
        {
            GrouptypesChipGroup!.SelectedItem = new ObservableCollection<SfChip>(GrouptypesChipGroup.Items!);
        }

        GrouptypesChipGroup.SelectionChanged += ChipGroupSelectionChanged;
    }

    private string[] _genreOptions = ["K-pop", "K-RnB", "J-pop", "C-pop", "T-pop", "Pop"];
    private void InitializeGenresChipGroup()
    {
        foreach (var option in _genreOptions)
        {
            GenresChipGroup?.Items?.Add(new SfChip() { Text = option, TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        }

        GenresChipGroup!.SelectionChanged += GenresChipGroup_SelectionChanged;

        if (Preferences.ContainsKey(CommonSettings.HOME_GENRES))
        {
            var selectedIndices = LoadTagsAsIntArray(CommonSettings.HOME_GENRES); // [2,3,4,5]

            var selectedItems = new ObservableCollection<SfChip>();
            for (var i = 0; i < selectedIndices!.Length; i++)
            {
                selectedItems.Add(GenresChipGroup!.Items![selectedIndices[i]]);
            }
            GenresChipGroup!.SelectedItem = selectedItems;
        }
        else
        {
            GenresChipGroup!.SelectedItem = new ObservableCollection<SfChip>(GenresChipGroup.Items!);
        }

        GenresChipGroup.SelectionChanged += ChipGroupSelectionChanged;
    }

    string[] _genOptions = ["1", "2", "3", "4", "5", "Non-kpop"];

    private void InitializeGenerationsChipGroup()
    {
        foreach (var option in _genOptions)
        {
            GenerationsChipGroup?.Items?.Add(new SfChip() { Text = option, TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        }

        if (Preferences.ContainsKey(CommonSettings.HOME_GENS))
        {
            var selectedIndices = LoadTagsAsIntArray(CommonSettings.HOME_GENS); // [2,3,4,5]

            var selectedItems = new ObservableCollection<SfChip>();
            for (var i = 0; i < selectedIndices!.Length; i++)
            {
                selectedItems.Add(GenerationsChipGroup!.Items![selectedIndices[i]]);
            }
            GenerationsChipGroup!.SelectedItem = selectedItems;
        }
        else
        {
            GenerationsChipGroup!.SelectedItem = new ObservableCollection<SfChip>(GenerationsChipGroup.Items!);
        }

        GenerationsChipGroup.SelectionChanged += ChipGroupSelectionChanged;
    }

    private string[] _companiesOptions = ["SM", "HYBE", "JYP", "YG", "Others"];
    private void InitializeCompaniesChipGroup()
    {
        foreach (var option in _companiesOptions)
        {
            CompaniesChipGroup?.Items?.Add(new SfChip() { Text = option, TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        }

        if (Preferences.ContainsKey(CommonSettings.HOME_COMPANIES))
        {
            var selectedIndices = LoadTagsAsIntArray(CommonSettings.HOME_COMPANIES); // [2,3,4,5]

            var selectedItems = new ObservableCollection<SfChip>();
            for (var i = 0; i < selectedIndices!.Length; i++)
            {
                selectedItems.Add(CompaniesChipGroup!.Items![selectedIndices[i]]);
            }
            CompaniesChipGroup!.SelectedItem = selectedItems;
        }
        else
        {
            CompaniesChipGroup!.SelectedItem = new ObservableCollection<SfChip>(CompaniesChipGroup.Items!);
        }

        CompaniesChipGroup.SelectionChanged += ChipGroupSelectionChanged;
    }

    private void InitializeYearsChipGroup()
    {
        List<string> options = ["< 2012"];
        for (int year = 2012; year <= 2025; year++)
        {
            options.Add(year.ToString());
        }
        foreach (var option in options)
        {
            YearsChipGroup?.Items?.Add(new SfChip() { Text = option, TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        }

        if (Preferences.ContainsKey(CommonSettings.HOME_YEARS))
        {
            var selectedIndices = LoadTagsAsIntArray(CommonSettings.HOME_YEARS); // [2,3,4,5]

            var selectedItems = new ObservableCollection<SfChip>();
            for (var i = 0; i < selectedIndices!.Length; i++)
            {
                selectedItems.Add(YearsChipGroup.Items[selectedIndices[i]]);
            }
            YearsChipGroup!.SelectedItem = selectedItems;
        }
        else
        {
            YearsChipGroup!.SelectedItem = new ObservableCollection<SfChip>(YearsChipGroup.Items!);
        }

        YearsChipGroup.SelectionChanged += ChipGroupSelectionChanged;
    }

    private void InitializeAntiOptionsChipGroup()
    {
        ObservableCollection<CustomChipModel> customChips = new();
        // TODO: Change color or invoke a selected/tap event?
        if (Preferences.ContainsKey(CommonSettings.HOME_ANTI_OPTIONS))
        {
            Dictionary<string, bool> antiOptionsValues = LoadTagsAsDictionary(CommonSettings.HOME_ANTI_OPTIONS);

            customChips.Add(new() { Name = "Last chorus", IsSelected = antiOptionsValues["Last chorus"] });
            customChips.Add(new() { Name = "Dance breaks", IsSelected = antiOptionsValues["Dance breaks"] });
            customChips.Add(new() { Name = "Tiktoks", IsSelected = antiOptionsValues["Tiktoks"] });
        }
        else
        {
            customChips.Add(new() { Name = "Last chorus", IsSelected = false });
            customChips.Add(new() { Name = "Dance breaks", IsSelected = false });
            customChips.Add(new() { Name = "Tiktoks", IsSelected = false });
        }
        AntiOptionsChipGroup.ItemsSource = customChips;
        AntiOptionsChipGroup.SelectionChanged += ChipGroupSelectionChanged;
    }

    private void HandleAutoStartRpd()
    {
        if (Preferences.ContainsKey(CommonSettings.START_RPD_AUTOMATIC))
        {
            bool startRpd = Preferences.Get(CommonSettings.START_RPD_AUTOMATIC, false);
            if (startRpd) { StartRpdButtonClicked(this, EventArgs.Empty); }
        }
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
        RefreshChipGroupColors(TimerChipGroup);
        RefreshChipGroupColors(VoiceAnnouncementsChipGroup);
        RefreshChipGroupColors(GrouptypesChipGroup);
        RefreshChipGroupColors(GenresChipGroup);
        RefreshChipGroupColors(GenerationsChipGroup);
        RefreshChipGroupColors(CompaniesChipGroup);
        RefreshChipGroupColors(YearsChipGroup);

        AntiOptionsChipGroup.ItemsSource = null!;
        ObservableCollection<CustomChipModel> customChips = new();

        if (Preferences.ContainsKey(CommonSettings.HOME_ANTI_OPTIONS))
        {
            Dictionary<string, bool> antiOptionsValues = LoadTagsAsDictionary(CommonSettings.HOME_ANTI_OPTIONS);

            customChips.Add(new() { Name = "Last chorus", IsSelected = antiOptionsValues["Last chorus"] });
            customChips.Add(new() { Name = "Dance breaks", IsSelected = antiOptionsValues["Dance breaks"] });
            customChips.Add(new() { Name = "Tiktoks", IsSelected = antiOptionsValues["Tiktoks"] });
        }
        else
        {
            customChips.Add(new() { Name = "Last chorus", IsSelected = false });
            customChips.Add(new() { Name = "Dance breaks", IsSelected = false });
            customChips.Add(new() { Name = "Tiktoks", IsSelected = false });
        }
        AntiOptionsChipGroup.ItemsSource = customChips;
        AntiOptionsChipGroup.SelectionChanged += ChipGroupSelectionChanged;
    }

    private static void RefreshChipGroupColors(SfChipGroup chipGroup)
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

    internal async void FeedbackButtonPressed(object? sender, EventArgs e)
    {
        if (Navigation.NavigationStack.Count < 2)
        {
            await Navigation.PushAsync(_feedbackPage, true);
            _feedbackPage.HomeView = this;
        }
    }

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

    private void HomeModeSegmentedControlSelectionChanged(object? sender, Syncfusion.Maui.Buttons.SelectionChangedEventArgs e)
    {
        RpdSettings!.UsingGeneratePlaylist = (HomeModeSegmentedControl.SelectedIndex == 1);

        StartModeButton.Text = RpdSettings.UsingGeneratePlaylist ? "Generate playlist" : "Start RPD";
        StartModeButton.ImageSource = RpdSettings.UsingGeneratePlaylist ? IconManager.SparkleIcon : IconManager.PlayIcon;

        DurationChipGroup!.Items!.Clear();
        if (RpdSettings.UsingGeneratePlaylist)
        {
            AddChipsToDurationChipGroup(["2.5", "2", "1.5", "1", "0.5"]); // TODO: Other, needs to match with SetDuration method.
            DurationChipGroup.SelectedItem = DurationChipGroup.Items[3];
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
        if (RpdSettings!.UsingGeneratePlaylist) { GeneratePlaylistButtonClicked(); }
        else { StartRpdButtonClicked(sender, e); }
    }

    private async void GeneratePlaylistButtonClicked()
    {
        if (!General.HasInternetConnection()) { return; }

        SetDuration();
        if (RpdSettings!.Duration == TimeSpan.Zero || RpdSettings.Duration == TimeSpan.MinValue)
        {
            General.ShowToast("Choose a duration.");
            return;
        }

        SetTimerMode();

        RpdSettings?.DetermineGroupTypes(GrouptypesChipGroup);
        RpdSettings!.DetermineGenres(GenresChipGroup);
        RpdSettings!.DetermineGens(GenerationsChipGroup);
        RpdSettings!.DetermineCompanies(CompaniesChipGroup);
        RpdSettings!.DetermineYears(YearsChipGroup);
        RpdSettings!.NumberedPartsBlacklist.Clear();
        RpdSettings!.PartsBlacklist.Clear();
        ApplyAntiOptions();

        var songParts = FilterSongParts();
        if (songParts.Count <= 0)
        {
            General.ShowToast("No songs found! Please change your settings.");
            return;
        }

        string generatedName = General.GenerateRandomName();

        InputPromptResult result = await General.ShowInputPrompt("Playlist name:", generatedName);
        if (result.IsCanceled) { return; }
        else if (result.ResultText.IsNullOrWhiteSpace()) { result.ResultText = generatedName; }
        await PlaylistsManager.GeneratePlaylistFromSongParts(result.ResultText, songParts, (int)RpdSettings!.Duration.TotalMinutes);
    }

    private void StartRpdButtonClicked(object? sender, EventArgs e)
    {
        if (!General.HasInternetConnection()) { return; }

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
            General.ShowToast("No songs found! Please change your settings.");
            return;
        }

        AppState.SongParts = songParts;
        PlayRandomSong(songParts);
    }

    private void SetDuration()
    {
        TimeSpan[] durations =
        {
            TimeSpan.FromHours(2.5),
            TimeSpan.FromHours(2),
            TimeSpan.FromHours(1.5),
            TimeSpan.FromHours(1),
            TimeSpan.FromHours(0.5)
        };

        for (int i = 0; i < DurationChipGroup.Items!.Count; i++)
        {
            if (DurationChipGroup.Items[i].IsSelected)
            {
                if (i < durations.Length)
                    RpdSettings!.Duration = durations[i];
                break;
            }
        }
    }

    private void SetTimerMode()
    {
        for (byte i = 0; i < TimerChipGroup.Items!.Count; i++)
        {
            if (TimerChipGroup.Items[i].IsSelected)
            {
                AppState.CountdownMode = (CountdownModeValue)i;
                break;
            }
        }
    }

    private void ApplyAntiOptions()
    {
        var antiOptionsItems = AntiOptionsChipGroup.ItemsSource as ObservableCollection<CustomChipModel>;
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

    private static void PlayRandomSong(List<SongPart> songParts)
    {
        int index = General.Rng.Next(songParts.Count);
        SongPart songPart = songParts[index];

        AppState.AutoplayMode = AutoplayModeValue.Shuffle;

        AudioManager.ChangeAndStartSong(songPart);
    }

    private readonly string[] antiOptionsList = ["Last chorus", "Dance breaks", "Tiktoks"];
    private void SaveTemplateImageButtonClicked(object sender, EventArgs e)
    {
        // Timer
        for (int i = 0; i < TimerChipGroup.Items!.Count; i++)
        {
            if (TimerChipGroup.Items[i].IsSelected)
            {
                SaveTag(CommonSettings.HOME_TIMER, i);
                break;
            }
        }

        // Voices
        for (int i = 0; i < VoiceAnnouncementsChipGroup.Items!.Count; i++)
        {
            if (VoiceAnnouncementsChipGroup.Items[i].IsSelected)
            {
                SaveTag(CommonSettings.HOME_VOICES, i);
                break;
            }
        }

        // TODO: METHODS
        // GroupTypes
        List<int> groupTypes = new();

        for (int i = 0; i < GrouptypesChipGroup.Items!.Count; i++)
        {
            if (GrouptypesChipGroup.Items[i].IsSelected)
            {
                groupTypes.Add(i);
            }
        }

        SaveTags(CommonSettings.HOME_GROUPTYPES, groupTypes.ToArray());

        // Genres
        List<int> genres = new();

        for (int i = 0; i < GenresChipGroup.Items!.Count; i++)
        {
            if (GenresChipGroup.Items[i].IsSelected)
            {
                genres.Add(i);
            }
        }

        SaveTags(CommonSettings.HOME_GENRES, genres.ToArray());

        // Companies
        List<int> companies = new();

        for (int i = 0; i < CompaniesChipGroup.Items!.Count; i++)
        {
            if (CompaniesChipGroup.Items[i].IsSelected)
            {
                companies.Add(i);
            }
        }

        SaveTags(CommonSettings.HOME_COMPANIES, companies.ToArray());

        // Years
        List<int> years = new();

        for (int i = 0; i < YearsChipGroup.Items!.Count; i++)
        {
            if (YearsChipGroup.Items[i].IsSelected)
            {
                years.Add(i);
            }
        }

        SaveTags(CommonSettings.HOME_YEARS, years.ToArray());

        // Gens
        List<int> gens = new();

        for (int i = 0; i < GenerationsChipGroup.Items!.Count; i++)
        {
            if (GenerationsChipGroup.Items[i].IsSelected)
            {
                gens.Add(i);
            }
        }

        SaveTags(CommonSettings.HOME_GENS, gens.ToArray());

        // Anti options
        Dictionary<string, bool> antiOptions = new();

        var antiOptionsItems = AntiOptionsChipGroup.ItemsSource as ObservableCollection<CustomChipModel>;

        for (int i = 0; i < antiOptionsItems!.Count; i++)
        {
            antiOptions.Add(antiOptionsList[i], antiOptionsItems[i].IsSelected);
        }

        SaveTags(CommonSettings.HOME_ANTI_OPTIONS, antiOptions);

        General.ShowToast("Preferences saved!");
    }

    #region Saving/loading tags
    private void SaveTags(string key, Dictionary<string, bool> tags)
    {
        string json = JsonConvert.SerializeObject(tags, Formatting.Indented);
        Preferences.Set(key, json);
    }

    private void SaveTags(string key, int[] tagIndices)
    {
        string json = JsonConvert.SerializeObject(tagIndices);
        Preferences.Set(key, json);
    }

    private void SaveTag(string key, int tagIndex)
    {
        string json = JsonConvert.SerializeObject(tagIndex);
        Preferences.Set(key, json);
    }

    private Dictionary<string, bool> LoadTagsAsDictionary(string key)
    {
        string json = Preferences.Get(key, "{}");
        return JsonConvert.DeserializeObject<Dictionary<string, bool>>(json) ?? new();
    }

    private int[]? LoadTagsAsIntArray(string key)
    {
        string json = Preferences.Get(key, "[]");
        return JsonConvert.DeserializeObject<int[]>(json);
    }

    private int LoadTagAsInt(string key)
    {
        string json = Preferences.Get(key, "0");
        return JsonConvert.DeserializeObject<int>(json);
    }
    #endregion
}