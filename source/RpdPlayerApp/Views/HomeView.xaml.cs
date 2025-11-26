using CommunityToolkit.Maui.Views;
using RpdPlayerApp.Architecture;
using RpdPlayerApp.Enums;
using RpdPlayerApp.Managers;
using RpdPlayerApp.Models;
using RpdPlayerApp.Repositories;
using RpdPlayerApp.Services;
using Syncfusion.Maui.Core;
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
        GenerationsImageButton.Clicked += GenerationsImageButtonClicked;
        CompaniesImageButton.Clicked += CompaniesImageButtonClicked;
        YearsImageButton.Clicked += YearsImageButtonClicked;
        OtherOptionsImageButton.Clicked += OtherOptionsImageButtonClicked;

        for (int year = 2012; year <= 2025; year++)
        {
            _yearOptions.Add(year.ToString());
        }

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
        else
        {
            await LoadDataOffline();
            InitSongParts?.Invoke(this, EventArgs.Empty);
        }
    }

    private async Task LoadDataOffline()
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

            InitializeHomeModeSegmentedControl();
            progress++;

            InitializeRpdSettings();
            progress++;

            await FileManager.SaveSongPartsAsync([.. SongPartRepository.SongParts]);
            await FileManager.SaveArtistsAsync([.. ArtistRepository.Artists]);
            await FileManager.SaveAlbumsAsync([.. AlbumRepository.Albums]);
            await FileManager.SaveVideosAsync([.. VideoRepository.Videos]);
            progress++;

            HandleAutoStartRpd();
            progress++;

            // Apply filter last.
            RefreshRpdSizeLabels();
            progress++;

            bool validConnection = General.HasInternetConnection() && !Constants.APIKEY.IsNullOrWhiteSpace() && !Constants.BASE_URL.IsNullOrWhiteSpace();

            IsOnlineImage.Source = validConnection ? IconManager.OnlineIcon : IconManager.OfflineIcon;
            IsOnlineLabel.Text = General.HasInternetConnection() ? "Online" : "Offline";

            VoiceAnnouncementsGrid.IsVisible = false;
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

    private void InitializeRpdSettings()
    {
        UpdateDuration(loadValue: true);
        UpdateTimerValue(loadValue: true);
        UpdateVoiceAnnouncementMode(loadValue: true);
        UpdateGrouptypes(loadValue: true);
        UpdateGenres(loadValue: true);
        UpdateGenerations(loadValue: true);
        UpdateCompanies(loadValue: true);
        UpdateYears(loadValue: true);
        UpdateOtherOptions(loadValue: true);
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
        string hourLabel = durationValue.TotalHours == 1.0 ? "hour" : "hours";
        DurationValueLabel.Text = $"{durationValue.TotalHours} {hourLabel}";
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
            int[] selectedIndices = LoadIntArray(CommonSettings.HOME_GROUPTYPES, [0, 1, 2]);
            RpdSettings!.GroupTypesSelectedIndices = selectedIndices.ToList()!;
        }

        RpdSettings!.GroupTypes = RpdSettings!.GroupTypesSelectedIndices.Select(i => (GroupType)i).ToList();
        List<string> selectedGrouptypes = RpdSettings!.GroupTypesSelectedIndices.Select(i => _groupTypeOptions[i]).ToList();
        string grouptypes;
        if (selectedGrouptypes.Count == 0)
        {
            grouptypes = "None";
        }
        else if (selectedGrouptypes.Count == _groupTypeOptions.Length)
        {
            grouptypes = "All";
        }
        else
        {
            grouptypes = string.Join(", ", selectedGrouptypes);
        }

        GrouptypesValuesLabel.Text = grouptypes;

        if (!loadValue)
        {
            RefreshRpdSizeLabels();
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
        RpdSettings!.Genres = RpdSettings!.GenresSelectedIndices.Select(i => _genreOptions[i]).ToList();
        List<string> selectedGenres = RpdSettings.Genres;
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
            RefreshRpdSizeLabels();
        }
        GenresSelectionChanged();
    }

    string[] _genOptions = ["1", "2", "3", "4", "5", "Non-kpop"];

    private void UpdateGenerations(bool loadValue = false)
    {
        if (loadValue)
        {
            int[] selectedIndices = LoadIntArray(CommonSettings.HOME_GENS, [0, 1, 2, 3, 4, 5]);
            RpdSettings!.GensSelectedIndices = selectedIndices.ToList()!;
        }


        RpdSettings!.Gens = RpdSettings.GensSelectedIndices.Select(i => (GenType)i).ToList();
        List<string> selectedGens = [.. RpdSettings!.GensSelectedIndices.Select(i => _genOptions[i])];
        string gens;
        if (selectedGens.Count == 0)
        {
            gens = "None";
        }
        else if (selectedGens.Count == _genOptions.Length)
        {
            gens = "All";
        }
        else if (selectedGens.Count <= 3)
        {
            gens = string.Join(", ", selectedGens);
        }
        else
        {
            gens = string.Join(", ", selectedGens.Take(3)) + ", ...";
        }

        GenerationsValuesLabel.Text = gens;

        if (!loadValue)
        {
            RefreshRpdSizeLabels();
        }
    }

    private string[] _companyOptions = ["SM", "HYBE", "JYP", "YG", "Others"];
    private void UpdateCompanies(bool loadValue = false)
    {
        if (loadValue)
        {
            int[] selectedIndices = LoadIntArray(CommonSettings.HOME_COMPANIES, [0, 1, 2, 3, 4]);
            RpdSettings!.CompaniesSelectedIndices = selectedIndices.ToList()!;
        }

        // Update RpdSettings value.
        RpdSettings!.Companies.Clear();
        foreach (var i in RpdSettings.CompaniesSelectedIndices)
        {
            switch (_companyOptions[i])
            {
                case "SM": RpdSettings.Companies.AddRange(Constants.SMCompanies); break;
                case "HYBE": RpdSettings.Companies.AddRange(Constants.HybeCompanies); break;
                case "JYP": RpdSettings.Companies.Add("JYP Entertainment"); break;
                case "YG": RpdSettings.Companies.AddRange(Constants.YGCompanies); break;
                case "Others": RpdSettings.Companies.AddRange(RpdSettings.OtherCompanies); break;
            }
        }

        List<string> selectedCompanies = [.. RpdSettings!.CompaniesSelectedIndices.Select(i => _companyOptions[i])];

        string companies;
        if (selectedCompanies.Count == 0)
        {
            companies = "None";
        }
        else if (selectedCompanies.Count == _companyOptions.Length)
        {
            companies = "All";
        }
        else if (selectedCompanies.Count <= 3)
        {
            companies = string.Join(", ", selectedCompanies);
        }
        else
        {
            companies = string.Join(", ", selectedCompanies.Take(3)) + ", ...";
        }

        CompaniesValuesLabel.Text = companies;

        if (!loadValue)
        {
            RefreshRpdSizeLabels();
        }
    }

    private List<string> _yearOptions = ["< 2012"];

    private void UpdateYears(bool loadValue = false)
    {
        if (loadValue)
        {
            List<int> selectedIndices = LoadIntArray(CommonSettings.HOME_YEARS, Constants.SELECTED_YEARS_INDICES_DEFAULT).ToList();
            RpdSettings!.YearsSelectedIndices = selectedIndices.ToList()!;
        }

        // Update RpdSettings value.
        RpdSettings!.Years = RpdSettings.YearsSelectedIndices.Select(i =>
        {
            if (_yearOptions[i] == "< 2012")
            {
                // Return all years from 1998 to 2011 for "< 2012".
                return Enumerable.Range(Constants.LOWEST_YEAR, 2011 - Constants.LOWEST_YEAR + 1);
            }
            else
            {
                // Parse the year string to int.
                return new[] { int.Parse(_yearOptions[i]) };
            }
        }).SelectMany(x => x).ToList();

        List<string> selectedYears = RpdSettings!.YearsSelectedIndices.Select(i => _yearOptions[i]).ToList();
        UpdateYearsUI(selectedYears);

        if (!loadValue)
        {
            RefreshRpdSizeLabels();
        }
    }

    private void UpdateYearsUI(List<string> selectedYears)
    {
        string years;

        if (selectedYears.Count == 0)
        {
            years = "None";
        }
        else if (selectedYears.Count == _yearOptions.Count)
        {
            years = $"{Constants.LOWEST_YEAR} - 2025";
        }
        else if (IsConsecutive(RpdSettings!.YearsSelectedIndices))
        {
            int last = RpdSettings!.YearsSelectedIndices.Count - 1;
            years = $"{_yearOptions[RpdSettings!.YearsSelectedIndices[0]]} - {_yearOptions[RpdSettings!.YearsSelectedIndices[last]]}";
        }
        else if (selectedYears.Count <= 3)
        {
            years = string.Join(", ", selectedYears);
        }
        else
        {
            years = string.Join(", ", selectedYears.Take(3)) + $", (+{selectedYears.Count - 3} more)";
        }
        YearsValuesLabel.Text = years;
    }

    // Other options / anti options.
    private void UpdateOtherOptions(bool loadValue = false)
    {
        if (loadValue)
        {
            if (Preferences.ContainsKey(CommonSettings.HOME_ANTI_OPTIONS))
            {
                Dictionary<string, bool> antiOptionsValuesRaw = LoadTagsAsDictionary(CommonSettings.HOME_ANTI_OPTIONS);
                var antiOptionsValues = new Dictionary<string, bool>
                {
                    { "No last chorus", antiOptionsValuesRaw.TryGetValue("No last chorus", out var lastChorus) ? lastChorus : false },
                    { "No dance breaks", antiOptionsValuesRaw.TryGetValue("No dance breaks", out var danceBreaks) ? danceBreaks : false },
                    { "No tiktoks", antiOptionsValuesRaw.TryGetValue("No tiktoks", out var tiktoks) ? tiktoks : false }
                };

                RpdSettings!.SelectedOtherOptions = antiOptionsValues;
            }
        }

        DebugService.Instance.Debug(string.Join(", ", RpdSettings!.SelectedOtherOptions.Select(x => $"{x.Key}: {x.Value}")));

        ApplyOtherOptions(RpdSettings!.SelectedOtherOptions);

        string otherOptions = string.Join(", ", RpdSettings!.SelectedOtherOptions.Where(x => x.Value).Select(x => x.Key));
        OtherOptionsValuesLabel.Text = string.IsNullOrWhiteSpace(otherOptions) ? "None" : otherOptions;
    }

    private async void TimerImageButtonClicked(object? sender, EventArgs e)
    {
        string action = await Shell.Current.DisplayActionSheet(title: $"Select timer between songs", cancel: "Cancel", destruction: null,
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
        string action = await Shell.Current.DisplayActionSheet(title: $"Select playlist duration", cancel: "Cancel", destruction: null,
            "3 hours",
            "2.5 hours",
            "2 hours",
            "1.5 hours",
            "1 hour",
            "0.5 hours"
        );
        action = action.Replace(" hours", string.Empty).Replace(" hour", string.Empty);

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

    private async void GenerationsImageButtonClicked(object? sender, EventArgs e)
    {
        EditChipOptionsPopup popup = new(title: "Select generations", options: _genOptions, selectedIndices: [.. RpdSettings!.GensSelectedIndices]);
        object? result = await Application.Current!.MainPage!.ShowPopupAsync(popup);

        if (result is null) { return; }
        if (result is SfChipGroup chipGroup)
        {
            RpdSettings?.DetermineGenerations(chipGroup);
        }

        UpdateGenerations();
    }

    private async void CompaniesImageButtonClicked(object? sender, EventArgs e)
    {
        EditChipOptionsPopup popup = new(title: "Select companies", options: _companyOptions, selectedIndices: [.. RpdSettings!.CompaniesSelectedIndices]);
        object? result = await Application.Current!.MainPage!.ShowPopupAsync(popup);

        if (result is null) { return; }
        if (result is SfChipGroup chipGroup)
        {
            RpdSettings?.DetermineCompanies(chipGroup);
        }

        UpdateCompanies();
    }

    private async void YearsImageButtonClicked(object? sender, EventArgs e)
    {
        EditChipOptionsPopup popup = new(title: "Select years", options: _yearOptions.ToArray(), selectedIndices: [.. RpdSettings!.YearsSelectedIndices]);
        object? result = await Application.Current!.MainPage!.ShowPopupAsync(popup);

        if (result is null) { return; }
        if (result is SfChipGroup chipGroup)
        {
            RpdSettings?.DetermineYears(chipGroup);
        }

        UpdateYears();
    }

    private async void OtherOptionsImageButtonClicked(object? sender, EventArgs e)
    {
        EditChipOtherOptionsPopup popup = new(title: "Select options", RpdSettings!.SelectedOtherOptions);
        object? result = await Application.Current!.MainPage!.ShowPopupAsync(popup);

        if (result is null) { return; }
        if (result is Dictionary<string, bool> otherOptionsChips)
        {
            RpdSettings?.DetermineOtherOptions(otherOptionsChips);
        }

        UpdateOtherOptions();
    }

    private void HandleAutoStartRpd()
    {
        if (Preferences.ContainsKey(CommonSettings.START_RPD_AUTOMATIC))
        {
            bool startRpd = Preferences.Get(CommonSettings.START_RPD_AUTOMATIC, false);
            if (startRpd) { StartRpdButtonClicked(this, EventArgs.Empty); }
        }
    }

    /// <summary> Apply filters to RpdSettings.</summary>
    private void RefreshRpdSizeLabels()
    {
        if (SongPartRepository.SongParts is null || SongPartRepository.SongParts.Count == 0) { return; }

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

        var songParts = FilterSongParts();
        if (songParts.Count <= 0)
        {
            General.ShowToast("No songs found! Please change your settings.");
            return;
        }

        AppState.SongParts = songParts;
        PlayRandomSong(songParts);
    }

    private void ApplyOtherOptions(Dictionary<string, bool> antiOptions)
    {
        if (antiOptions is null || antiOptions.Count == 0) { return; }

        RpdSettings?.NumberedPartsBlacklist.Clear();
        RpdSettings?.PartsBlacklist.Clear();

        if (antiOptions["No last chorus"])
        {
            RpdSettings?.NumberedPartsBlacklist.AddRange(["CE2", "C3", "CDB3", "CE3", "CE2", "P3", "PDB3"]);
        }
        else if (antiOptions["No dance breaks"])
        {
            RpdSettings?.PartsBlacklist.AddRange(["CDB", "CDBE", "DB", "DBC", "DBE", "DBO", "PDB", "B", "O"]);
        }
        else if (antiOptions["No tiktoks"])
        {
            RpdSettings?.PartsBlacklist.Add("T");
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

    private void SaveTemplateImageButtonClicked(object sender, EventArgs e)
    {
        Preferences.Set(CommonSettings.HOME_TIMER, (int)RpdSettings!.CountdownMode);
        Preferences.Set(CommonSettings.HOME_VOICES, (int)RpdSettings!.AnnouncementMode);

        SaveTags(CommonSettings.HOME_GROUPTYPES, RpdSettings.GroupTypesSelectedIndices.ToArray());
        SaveTags(CommonSettings.HOME_GENRES, RpdSettings.GenresSelectedIndices.ToArray());
        SaveTags(CommonSettings.HOME_GENS, RpdSettings.GensSelectedIndices.ToArray());
        SaveTags(CommonSettings.HOME_COMPANIES, RpdSettings.CompaniesSelectedIndices.ToArray());
        SaveTags(CommonSettings.HOME_YEARS, RpdSettings.YearsSelectedIndices.ToArray());

        SaveTags(CommonSettings.HOME_ANTI_OPTIONS, RpdSettings.SelectedOtherOptions);

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

    // Helper method to check if indices are consecutive.
    private static bool IsConsecutive(List<int> numbers)
    {
        if (numbers.Count < 2) { return false; }

        for (int i = 1; i < numbers.Count; i++)
        {
            var currentIndexValue = numbers[i];
            var lastIndexValue = numbers[i - 1];
            if (currentIndexValue != lastIndexValue + 1)
            {
                return false;
            }
        }
        return true;
    }
}