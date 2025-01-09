using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using RpdPlayerApp.Architecture;
using RpdPlayerApp.Items;
using RpdPlayerApp.Models;
using RpdPlayerApp.Repositories;
using RpdPlayerApp.ViewModels;
using Syncfusion.Maui.Buttons;
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
        UniqueSongCountLabel.Text = $", Unique songs: {groupedTitles.Count()}";
        GeneratePlaylistSwitch.StateChanged += GeneratePlaylistSwitch_StateChanged;
        GeneratePlaylistSwitch.IsOn = false;
    }

    private void GeneratePlaylistSwitch_StateChanged(object? sender, SwitchStateChangedEventArgs e) => RpdSettings!.UsingGeneratePlaylist = (bool)GeneratePlaylistSwitch.IsOn!;

    private void SongPartsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => SongPartCountLabel.Text = $"SongParts: {SongPartRepository.SongParts.Count}";

    private void ArtistsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => ArtistCountLabel.Text = $"Artists: {ArtistRepository.Artists.Count}";

    private void AlbumsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => AlbumCountLabel.Text = $",  Albums: {AlbumRepository.Albums.Count}";

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
    private void GeneratePlaylistButtonClicked(object sender, EventArgs e) => Toast.Make($"Not implemented yet!", ToastDuration.Short, 14).Show();

    private void StartRpdButtonClicked(object sender, EventArgs e)
    {
        if (!HelperClass.HasInternetConnection())
            return;

        var songParts = SongPartRepository.SongParts;

        var random = new Random();
        int index = random.Next(SongPartRepository.SongParts.Count);
        SongPart songPart = songParts[index];

        MainViewModel.AutoplayMode = 2; // Shuffle
        MainViewModel.CurrentSongPart = songPart;
        PlaySongPart?.Invoke(sender, e);
    }
    private void SearchByCategoryButtonClicked(object sender, EventArgs e) => ShowCategories?.Invoke(sender, e);
}