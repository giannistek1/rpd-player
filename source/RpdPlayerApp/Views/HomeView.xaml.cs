using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using RpdPlayerApp.Architecture;
using RpdPlayerApp.Items;
using RpdPlayerApp.Models;
using RpdPlayerApp.Repositories;
using RpdPlayerApp.ViewModels;
using Syncfusion.Maui.Buttons;
using Syncfusion.Maui.Core;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace RpdPlayerApp.Views;

public partial class HomeView : ContentView
{
    internal event EventHandler? PlaySongPart;
    internal event EventHandler? FilterPressed;
    internal event EventHandler? CreatePlaylistButtonPressed;
    internal event EventHandler? ShowCategories;

    private readonly SettingsPage _settingsPage = new();
    internal MainPage? ParentPage { get; set; }
    internal RpdSettings? RpdSettings { get; set; }

    public HomeView()
    {
        InitializeComponent();

#if DEBUG
        string releaseMode = "D"; // Debug
#else
        string releaseMode = "P"; // Production
#endif
        VersionLabel.Text = $"v{AppInfo.Current.VersionString}.{AppInfo.Current.BuildString}.{releaseMode}";

        ArtistRepository.Artists.CollectionChanged += ArtistsCollectionChanged;
        AlbumRepository.Albums.CollectionChanged += AlbumsCollectionChanged;
        SongPartRepository.SongParts.CollectionChanged += SongPartsCollectionChanged;

        // Get data
        ArtistRepository.GetArtists(); 
        AlbumRepository.GetAlbums();
        VideoRepository.GetVideos();
        SongPartRepository.GetSongParts();

        var groupedTitles = from s in SongPartRepository.SongParts
                            group s.Title by s.Title into g
                            select new { Title = g.Key, Titles = g.ToList() };

        // Todo: Needs to go into the load method
#if DEBUG
        UniqueSongCountLabel.Text = $"{groupedTitles.Count()}";
#endif
#if RELEASE
        UniqueSongCountLabel.IsVisible = false;
        UniqueSongCountImage.IsVisible = false;
#endif

        HomeModeSegmentedControl.ItemsSource = new string[] { "Start RPD", "Generate playlist"};
        HomeModeSegmentedControl.SelectedIndex = 0;
        HomeModeSegmentedControl.SelectionChanged += HomeModeSegmentedControl_SelectionChanged;

        StartModeButton.Text = "Start RPD";
        StartModeButton.Clicked += StartModeButtonClicked;

        DurationChipGroup?.Items?.Add(new SfChip() { Text = "∞", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        DurationChipGroup?.Items?.Add(new SfChip() { Text = "2H", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        DurationChipGroup?.Items?.Add(new SfChip() { Text = "1H", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        DurationChipGroup?.Items?.Add(new SfChip() { Text = "Other", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        DurationChipGroup!.SelectedItem = DurationChipGroup.Items[0];

        GrouptypesChipGroup?.Items?.Add(new SfChip() { Text = "Male", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        GrouptypesChipGroup?.Items?.Add(new SfChip() { Text = "Female", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        GrouptypesChipGroup?.Items?.Add(new SfChip() { Text = "Mixed", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        GrouptypesChipGroup!.SelectedItem = GrouptypesChipGroup.Items;

        GenresChipGroup?.Items?.Add(new SfChip() { Text = "K-pop", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        GenresChipGroup?.Items?.Add(new SfChip() { Text = "J-pop", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        GenresChipGroup?.Items?.Add(new SfChip() { Text = "C-pop", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        GenresChipGroup?.Items?.Add(new SfChip() { Text = "T-pop", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        GenresChipGroup?.Items?.Add(new SfChip() { Text = "Pop", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        GenresChipGroup!.SelectedItem = GenresChipGroup.Items;

        GenerationChipGroup?.Items?.Add(new SfChip() { Text = "1", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        GenerationChipGroup?.Items?.Add(new SfChip() { Text = "2", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        GenerationChipGroup?.Items?.Add(new SfChip() { Text = "3", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        GenerationChipGroup?.Items?.Add(new SfChip() { Text = "4", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        GenerationChipGroup?.Items?.Add(new SfChip() { Text = "5", TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        GenerationChipGroup!.SelectedItem = GenerationChipGroup.Items;
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
        }
    }
    private void CreatePlaylistButtonClicked(object sender, EventArgs e) => CreatePlaylistButtonPressed?.Invoke(sender, e);
    private void SearchByCategoryButtonClicked(object sender, EventArgs e) => ShowCategories?.Invoke(sender, e);
    private void HomeModeSegmentedControl_SelectionChanged(object? sender, Syncfusion.Maui.Buttons.SelectionChangedEventArgs e)
    {
        RpdSettings!.UsingGeneratePlaylist = (HomeModeSegmentedControl.SelectedIndex == 1);
        StartModeButton.Text = (RpdSettings!.UsingGeneratePlaylist) ? "Generate playlist" : "Start RPD";
    }
    private void StartModeButtonClicked(object? sender, EventArgs e)
    {
        if (RpdSettings!.UsingGeneratePlaylist) { GeneratePlaylistButtonClicked();  }
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
                RpdSettings?.GroupTypes.Add(GrouptypesChipGroup.Items[i].Text);
            }
        }

        var songParts = SongPartRepository.SongParts;

        var random = new Random();
        int index = random.Next(SongPartRepository.SongParts.Count);
        SongPart songPart = songParts[index];

        MainViewModel.AutoplayMode = 2; // Shuffle
        MainViewModel.CurrentSongPart = songPart;
        PlaySongPart?.Invoke(sender, EventArgs.Empty);
    }
}