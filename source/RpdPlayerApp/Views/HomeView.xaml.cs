using CommunityToolkit.Maui.Views;
using RpdPlayerApp.Architecture;
using RpdPlayerApp.Enums;
using RpdPlayerApp.Managers;
using RpdPlayerApp.Models;
using RpdPlayerApp.Repositories;
using RpdPlayerApp.Services;
using Syncfusion.Maui.Core;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text.Json;

namespace RpdPlayerApp.Views;

public partial class HomeView : ContentView
{
    //internal event EventHandler? PlaySongPart;
    internal event EventHandler? InitSongParts;
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

        TimerImageButton.Clicked += TimerImageButtonClicked;
        DurationImageButton.Clicked += DurationImageButtonClicked;
        VoiceAnnouncementImageButton.Clicked += VoiceAnnouncementImageButtonClicked;
        GrouptypesImageButton.Clicked += GrouptypesImageButtonClicked;
        GenresImageButton.Clicked += GenresImageButtonClicked;

        _ = LoadInitialDataAsync();

        Loaded += OnLoad;
    }

    private async Task LoadInitialDataAsync()
    {
        if (General.HasInternetConnection())
        {
            ArtistRepository.GetArtists();
            AlbumRepository.GetAlbums();
            VideoRepository.GetVideos();
            SongPartRepository.GetSongParts();
        }
        else // Offline
        {
            var loadedArtists = await FileManager.LoadArtistsAsync();
            ArtistRepository.Artists.Clear();
            foreach (Artist artist in loadedArtists)
            {
                artist.InitPostProperties();
                ArtistRepository.Artists.Add(artist);
            }

            var loadedAlbums = await FileManager.LoadAlbumsAsync();
            AlbumRepository.Albums.Clear();
            foreach (Album album in loadedAlbums)
            {
                album.InitPostProperties();
                AlbumRepository.Albums.Add(album);
            }

            var loadedVideos = await FileManager.LoadVideosAsync();
            VideoRepository.Videos.Clear();
            foreach (Video video in loadedVideos)
            {
                VideoRepository.Videos.Add(video);
            }

            var loadedSongParts = await FileManager.LoadSongPartsAsync();
            SongPartRepository.SongParts.Clear();
            foreach (SongPart part in loadedSongParts)
            {
                part.InitPostProperties();
                SongPartRepository.SongParts.Add(part);
            }

            InitSongParts?.Invoke(null, EventArgs.Empty);
        }
    }

    private async void OnLoad(object? sender, EventArgs e)
    {
        // For debugging.
        int progress = 0;
        try
        {
            _ = HandleSongPartsDifference();
            progress++;

            SetVersionLabel();
            progress++;
#if RELEASE 
            UniqueSongCountImage.IsVisible = false;
            UniqueSongCountLabel.IsVisible = false;
#endif
            InitializeCompanies();
            progress++;

            InitializeChipGroups();
            progress++;

            await FileManager.SaveSongPartsAsync([.. SongPartRepository.SongParts]);
            await FileManager.SaveArtistsAsync([.. ArtistRepository.Artists]);
            await FileManager.SaveAlbumsAsync([.. AlbumRepository.Albums]);
            await FileManager.SaveVideosAsync([.. VideoRepository.Videos]);
            progress++;

            HandleAutoStartRpd();
            progress++;

            ChipGroupSelectionChanged(null, null);
            progress++;

            bool validConnection = General.HasInternetConnection() && !Constants.APIKEY.IsNullOrWhiteSpace() && !Constants.BASE_URL.IsNullOrWhiteSpace();

            IsOnlineImage.Source = validConnection ? IconManager.OnlineIcon : IconManager.OfflineIcon;
            IsOnlineLabel.Text = General.HasInternetConnection() ? "Online" : "Offline";
        }
        catch (Exception ex)
        {
            DebugService.Instance.Debug($"HomeView - {progress}: {ex.Message}");
        }
    }


    private async Task HandleSongPartsDifference()
    {
        if (SongPartRepository.SongParts is null || SongPartRepository.SongParts.Count == 0) { return; }

        var oldSongList = await FileManager.LoadSongPartsAsync();

        if (oldSongList is not null)
        {
            var newSongList = SongPartRepository.SongParts.ToList();
            var differentNewSongs = FindDifferentNewSongs(newSongList, oldSongList);
            UpdateNewsBadge(differentNewSongs);
        }
    }

    private List<SongPart> FindDifferentNewSongs(List<SongPart> newSongList, List<SongPart> oldSongList)
    {
#if RELEASE
        // Add new songs based on audioURL.
        var differentNewSongs = newSongList.Where(item1 => !oldSongList.Any(item2 => item1.AudioURL == item2.AudioURL)).ToList();
        foreach (var newSongPart in newSongList)
        {
            if (!newSongPart.HasVideo) continue;

            var oldSong = oldSongList.FirstOrDefault(s => s?.AudioURL == newSongPart.AudioURL);
            if (oldSong is not null && oldSong.HasVideo != newSongPart.HasVideo)
            {
                newSongPart.NewVideoAvailable = true;
            }
        }

        // Add new songs based on video availability.
        var videoDiffs = newSongList
            .Where(item1 => !oldSongList.Any(item2 => item1.HasVideo == item2.HasVideo))
            .Except(differentNewSongs)
            .ToList();

        differentNewSongs.AddRange(videoDiffs);
#else
        var differentNewSongs = newSongList.Where(s => s.ArtistName!.Equals("ATEEZ", StringComparison.OrdinalIgnoreCase)).ToList();
#endif
        return differentNewSongs;
    }

    internal void UpdateNewsBadge(List<SongPart> differentNewSongs)
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
        if (ArtistRepository.Artists is null || ArtistRepository.Artists.Count == 0) { DebugService.Instance.Debug("Initialize Companies has no artists."); return; }

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
        UpdateDuration(loadValue: true);
        UpdateTimerValue(loadValue: true);
        UpdateVoiceAnnouncementMode(loadValue: true);
        UpdateGrouptypes(loadValue: true);
        UpdateGenres(loadValue: true);
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

    private void UpdateDuration(bool loadValue = false)
    {
        if (loadValue)
        {
            double hours = LoadTagAsDouble(CommonSettings.HOME_DURATION, 1.0);
            RpdSettings!.Duration = TimeSpan.FromHours(hours);
        }

        TimeSpan durationValue = RpdSettings!.Duration;
        DurationValueLabel.Text = $"{durationValue.TotalHours}";
    }

    private void UpdateTimerValue(bool loadValue = false)
    {
        if (loadValue)
        {
            int selectedIndex = LoadTagAsInt(CommonSettings.HOME_TIMER);
            if (Enum.IsDefined(typeof(CountdownModeValue), selectedIndex))
            {
                RpdSettings!.CountdownMode = (CountdownModeValue)selectedIndex;
            }
        }

        string timerValue = RpdSettings.GetCountdownModeText(RpdSettings!.CountdownMode);
        TimerValueLabel.Text = $"{timerValue}";
    }

    private void UpdateVoiceAnnouncementMode(bool loadValue = false)
    {
        if (loadValue)
        {
            int selectedIndex = LoadTagAsInt(CommonSettings.HOME_VOICES);
            if (Enum.IsDefined(typeof(AnnouncementModeValue), selectedIndex))
            {
                RpdSettings!.AnnouncementMode = (AnnouncementModeValue)selectedIndex;
            }
        }

        string announcementValue = RpdSettings.GetAnnouncementModeText(RpdSettings!.AnnouncementMode);
        VoiceAnnouncementsValueLabel.Text = $"{announcementValue}";
    }

    string[] _groupTypeOptions = ["Male", "Female", "Mixed"];
    private void UpdateGrouptypes(bool loadValue = false)
    {
        if (loadValue)
        {
            int[] selectedIndices = LoadIntArray(CommonSettings.HOME_GROUPTYPES, new int[] { 0, 1, 2 });
            RpdSettings!.GroupTypesSelectedIndices = selectedIndices.ToList()!;
        }

        string grouptypes = string.Join(", ", RpdSettings!.GroupTypesSelectedIndices.Select(i => _groupTypeOptions[i]));
        GrouptypesValuesLabel.Text = (string.IsNullOrWhiteSpace(grouptypes) ? "None" : grouptypes);

        if (!loadValue)
        {
            ChipGroupSelectionChanged(null, null);
        }
    }

    private string[] _genreOptions = ["K-pop", "K-RnB", "J-pop", "C-pop", "T-pop", "Pop"];
    private void UpdateGenres(bool loadValue = false)
    {
        if (loadValue)
        {
            int[] selectedIndices = LoadIntArray(CommonSettings.HOME_GENRES, [0, 1, 2, 3, 4, 5]);
            RpdSettings!.GenresSelectedIndices = selectedIndices.ToList()!;
        }

        var selectedGenres = RpdSettings!.GenresSelectedIndices.Select(i => _genreOptions[i]).ToList();
        string genres;
        if (selectedGenres.Count == 0)
        {
            genres = "None";
        }
        else if (selectedGenres.Count == _genreOptions.Length)
        {
            genres = "All";
        }
        else if (selectedGenres.Count <= 3)
        {
            genres = string.Join(", ", selectedGenres);
        }
        else
        {
            genres = string.Join(", ", selectedGenres.Take(3)) + ", ...";
        }

        GenresValuesLabel.Text = genres;

        if (!loadValue)
        {
            ChipGroupSelectionChanged(null, null);
        }
        GenresSelectionChanged();
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
            int[]? selectedIndices = LoadIntArray(CommonSettings.HOME_GENS, [0, 1, 2, 3, 4, 5]);

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
            int[]? selectedIndices = LoadIntArray(CommonSettings.HOME_COMPANIES, [0, 1, 2, 3, 4]);

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
            int[]? selectedIndices = LoadIntArray(CommonSettings.HOME_YEARS, [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14]);

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
        ObservableCollection<CustomChipModel> customChips = [];
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

    private async void TimerImageButtonClicked(object? sender, EventArgs e)
    {
        string action = await Shell.Current.DisplayActionSheet(title: $"Timer", cancel: "Cancel", destruction: null,
            "Off",
            "3s",
            "5s",
            "Custom (Pro)"
        );

        switch (action)
        {
            case "Off":
                RpdSettings!.CountdownMode = CountdownModeValue.Off;
                break;
            case "3s":
                RpdSettings!.CountdownMode = CountdownModeValue.Short;
                break;
            case "5s":
                RpdSettings!.CountdownMode = CountdownModeValue.Long;
                break;
            case "Custom (Pro)":
                RpdSettings!.CountdownMode = CountdownModeValue.Custom;
                break;
        }

        UpdateTimerValue();
    }

    private async void DurationImageButtonClicked(object? sender, EventArgs e)
    {
        string action = await Shell.Current.DisplayActionSheet(title: $"Playlist duration in hours", cancel: "Cancel", destruction: null,
            "3",
            "2.5",
            "2",
            "1.5",
            "1",
            "0.5"
        );

        // Parse the selected string as hours and assign directly.
        if (double.TryParse(action,
                            System.Globalization.NumberStyles.AllowDecimalPoint,
                            System.Globalization.CultureInfo.InvariantCulture,
                            out var hours))
        {
            RpdSettings!.Duration = TimeSpan.FromHours(hours);
        }

        UpdateDuration();
    }

    private async void VoiceAnnouncementImageButtonClicked(object? sender, EventArgs e)
    {
        string action = await Shell.Current.DisplayActionSheet(title: $"Voice announcement", cancel: "Cancel", destruction: null,
            "Off",
            "Always",
            "Dancebreaks",
            "Artist",
            "Specific",
            "Grouptype (BG/GG/MIX)"
        );

        switch (action)
        {
            case "Off":
                RpdSettings!.AnnouncementMode = AnnouncementModeValue.Off;
                break;
            case "Always":
                RpdSettings!.AnnouncementMode = AnnouncementModeValue.AlwaysSongPart;
                break;
            case "Dancebreaks":
                RpdSettings!.AnnouncementMode = AnnouncementModeValue.DancebreakOnly;
                break;
            case "Artist":
                RpdSettings!.AnnouncementMode = AnnouncementModeValue.Artist;
                break;
            case "Specific":
                RpdSettings!.AnnouncementMode = AnnouncementModeValue.Specific;
                break;
            case "Grouptype (BG/GG/MIX)":
                RpdSettings!.AnnouncementMode = AnnouncementModeValue.GroupType;
                break;
        }

        UpdateVoiceAnnouncementMode();
    }

    private async void GrouptypesImageButtonClicked(object? sender, EventArgs e)
    {
        EditChipOptionsPopup popup = new(title: "Select grouptypes", options: _groupTypeOptions, selectedIndices: RpdSettings!.GroupTypesSelectedIndices.ToArray());
        object? result = await Application.Current!.MainPage!.ShowPopupAsync(popup);

        if (result is null) { return; }
        if (result is SfChipGroup chipGroup)
        {
            RpdSettings?.DetermineGroupTypes(chipGroup);
        }

        UpdateGrouptypes();
    }

    private async void GenresImageButtonClicked(object? sender, EventArgs e)
    {
        EditChipOptionsPopup popup = new(title: "Select genres", options: _genreOptions, selectedIndices: [.. RpdSettings!.GenresSelectedIndices]);
        object? result = await Application.Current!.MainPage!.ShowPopupAsync(popup);

        if (result is null) { return; }
        if (result is SfChipGroup chipGroup)
        {
            RpdSettings?.DetermineGenres(chipGroup);
        }

        UpdateGenres();
    }

    private void HandleAutoStartRpd()
    {
        if (Preferences.ContainsKey(CommonSettings.START_RPD_AUTOMATIC))
        {
            bool startRpd = Preferences.Get(CommonSettings.START_RPD_AUTOMATIC, false);
            if (startRpd) { StartRpdButtonClicked(this, EventArgs.Empty); }
        }
    }

    /// <summary> Apply chipgroup selection to RPDSettings.</summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ChipGroupSelectionChanged(object? sender, Syncfusion.Maui.Core.Chips.SelectionChangedEventArgs? e)
    {
        if (SongPartRepository.SongParts is null || SongPartRepository.SongParts.Count == 0) { return; }

        DebugService.Instance.Debug("ChipGroupSelectionChanged invoked.");

        //RpdSettings?.DetermineGroupTypes(GrouptypesChipGroup);
        //RpdSettings?.DetermineGenres(GenresChipGroup);
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

    private void GenresSelectionChanged()
    {
        GenerationsGrid.IsVisible = RpdSettings!.GenresSelectedIndices.Contains(0); // K-pop
        CompaniesGrid.IsVisible = RpdSettings.GenresSelectedIndices.Contains(0);
    }

    internal void RefreshThemeColors()
    {
        //RefreshChipGroupColors(GrouptypesChipGroup);
        //RefreshChipGroupColors(GenresChipGroup);
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

        DurationGrid.IsVisible = RpdSettings!.UsingGeneratePlaylist;
    }

    private async void StartModeButtonClicked(object? sender, EventArgs e)
    {
        if (RpdSettings!.UsingGeneratePlaylist) { await GeneratePlaylistButtonClicked(); }
        else { StartRpdButtonClicked(sender, e); }
    }

    private async Task GeneratePlaylistButtonClicked()
    {
        if (!General.HasRepositoryData()) { return; }

        if (RpdSettings!.Duration <= TimeSpan.Zero)
        {
            General.ShowToast("Choose a duration.");
            return;
        }

        AppState.CountdownMode = RpdSettings!.CountdownMode;
        //AppState.AnnouncementMode = RpdSettings!.AnnouncementMode;

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

        InputPromptResult result = await General.ShowInputPromptAsync("Playlist name:", generatedName);
        if (result.IsCanceled) { return; }
        else if (result.Text.IsNullOrWhiteSpace()) { result.Text = generatedName; }
        await PlaylistsManager.GeneratePlaylistFromSongParts(result.Text, songParts, (int)RpdSettings!.Duration.TotalMinutes);
    }

    private void StartRpdButtonClicked(object? sender, EventArgs e)
    {
        if (!General.HasInternetConnection()) { return; }

        AppState.CountdownMode = RpdSettings!.CountdownMode;

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

    private void ApplyAntiOptions()
    {
        var antiOptionsItems = AntiOptionsChipGroup.ItemsSource as ObservableCollection<CustomChipModel>;
        if (antiOptionsItems is null) { return; }

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
        if (SongPartRepository.SongParts is null || SongPartRepository.SongParts.Count == 0) { return []; }

        var songParts = SongPartRepository.SongParts.Where(s => RpdSettings!.GroupTypes.Contains(s.Artist.GroupType))
                                                    .Where(s => RpdSettings!.Genres.Contains(s.Album.GenreFull))
                                                    .Where(s => !RpdSettings!.NumberedPartsBlacklist.Contains(s.PartNameShortWithNumber))
                                                    .Where(s => !RpdSettings!.PartsBlacklist.Contains(s.PartNameShort))
                                                    .Where(s => RpdSettings!.Years.Contains(s.Album.ReleaseDate.Year))
                                                    .ToList();

        if (RpdSettings!.GenresSelectedIndices.Contains(0)) // K-pop
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
        Preferences.Set(CommonSettings.HOME_TIMER, (int)RpdSettings!.CountdownMode);

        // Voices
        Preferences.Set(CommonSettings.HOME_VOICES, (int)RpdSettings!.AnnouncementMode);

        // TODO: METHODS
        // GroupTypes
        SaveTags(CommonSettings.HOME_GROUPTYPES, RpdSettings.GroupTypesSelectedIndices.ToArray());

        // Genres
        SaveTags(CommonSettings.HOME_GENRES, RpdSettings.GenresSelectedIndices.ToArray());

        // Companies
        List<int> companies = [];

        for (int i = 0; i < CompaniesChipGroup.Items!.Count; i++)
        {
            if (CompaniesChipGroup.Items[i].IsSelected)
            {
                companies.Add(i);
            }
        }

        SaveTags(CommonSettings.HOME_COMPANIES, companies.ToArray());

        // Years
        List<int> years = [];

        for (int i = 0; i < YearsChipGroup.Items!.Count; i++)
        {
            if (YearsChipGroup.Items[i].IsSelected)
            {
                years.Add(i);
            }
        }

        SaveTags(CommonSettings.HOME_YEARS, years.ToArray());

        // Gens
        List<int> gens = [];

        for (int i = 0; i < GenerationsChipGroup.Items!.Count; i++)
        {
            if (GenerationsChipGroup.Items[i].IsSelected)
            {
                gens.Add(i);
            }
        }

        SaveTags(CommonSettings.HOME_GENS, gens.ToArray());

        // Anti options
        Dictionary<string, bool> antiOptions = [];

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
        string json = JsonSerializer.Serialize(tags, new JsonSerializerOptions { WriteIndented = true });
        Preferences.Set(key, json);
    }

    private void SaveTags(string key, int[] tagIndices)
    {
        string json = JsonSerializer.Serialize(tagIndices);
        Preferences.Set(key, json);
    }

    private void SaveTag(string key, int tagIndex)
    {
        string json = JsonSerializer.Serialize(tagIndex);
        Preferences.Set(key, json);
    }

    private Dictionary<string, bool> LoadTagsAsDictionary(string key)
    {
        string json = Preferences.Get(key, "{}");
        return JsonSerializer.Deserialize<Dictionary<string, bool>>(json) ?? new();
    }

    public int[] LoadIntArray(string key, int[] defaultValue)
    {
        if (!Preferences.ContainsKey(key))
            return defaultValue;

        var json = Preferences.Get(key, "[]");
        return JsonSerializer.Deserialize<int[]>(json) ?? defaultValue;
    }

    private int LoadTagAsInt(string key, int defaultValue = 0)
    {
        return Preferences.Get(key, defaultValue);
    }

    private double LoadTagAsDouble(string key, double defaultValue = 0.0)
    {
        return Preferences.Get(key, defaultValue);
    }
    #endregion
}