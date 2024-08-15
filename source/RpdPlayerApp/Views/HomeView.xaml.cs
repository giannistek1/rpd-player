using CommunityToolkit.Maui.Alerts;
using RpdPlayerApp.Repository;
using RpdPlayerApp.ViewModel;
using System.Collections.Specialized;

namespace RpdPlayerApp.Views;

public partial class HomeView : ContentView
{
    internal EventHandler? FilterPressed;

    public HomeView()
    {
        InitializeComponent();

        VersionLabel.Text = $"v{AppInfo.Current.VersionString}.{AppInfo.Current.BuildString}";

        ArtistRepository.Artists.CollectionChanged += ArtistsCollectionChanged;
        AlbumRepository.Albums.CollectionChanged += AlbumsCollectionChanged;
        SongPartRepository.SongParts.CollectionChanged += SongPartsCollectionChanged;

        // Get
        ArtistRepository.GetArtists(); 
        AlbumRepository.GetAlbums();
        SongPartRepository.GetSongParts();

        var groupedArtists = from s in SongPartRepository.SongParts
                             group s.Artist by s.ArtistName into g
                             orderby g.Count() descending
                             select new { ArtistName = g.Key, ArtistCount = g.Count(), Artists = g.ToList() };

        var groupedTitles = from s in SongPartRepository.SongParts
                            group s.Title by s.Title into g
                            select new { Title = g.Key, Titles = g.ToList() };

        UniqueSongCountLabel.Text = $",  Unique songs: {groupedTitles.Count()}";

        RandomListView.ItemsSource = groupedArtists;
        SentrySdk.CaptureMessage("Hello Sentry");
    }

    private void SongPartsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        SongPartCountLabel.Text = $"SongParts: {SongPartRepository.SongParts.Count}";
    }

    private void ArtistsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        ArtistCountLabel.Text = $"Artists: {ArtistRepository.Artists.Count}";
    }

    private void AlbumsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        AlbumCountLabel.Text = $",  Albums: {AlbumRepository.Albums.Count}";
    }
     
    private void SetFilterFemale(object sender, TappedEventArgs e)
    {
        MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.Female;
        CommunityToolkit.Maui.Alerts.Toast.Make($"Filter mode: {MainViewModel.SearchFilterMode}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        FilterPressed?.Invoke(this, e);
    }

    private void SetFilterMale(object sender, TappedEventArgs e)
    {
        MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.Male;
        CommunityToolkit.Maui.Alerts.Toast.Make($"Filter mode: {MainViewModel.SearchFilterMode}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        FilterPressed?.Invoke(this, e);
    }

    private void SetFilterSM(object sender, TappedEventArgs e)
    {
        MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.SM;
        CommunityToolkit.Maui.Alerts.Toast.Make($"Filter mode: {MainViewModel.SearchFilterMode}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        FilterPressed?.Invoke(this, e);
    }

    private void SetFilterJYP(object sender, TappedEventArgs e)
    {
        MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.JYP;
        CommunityToolkit.Maui.Alerts.Toast.Make($"Filter mode: {MainViewModel.SearchFilterMode}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        FilterPressed?.Invoke(this, e);
    }

    private void SetFilterYG(object sender, TappedEventArgs e)
    {
        MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.YG;
        CommunityToolkit.Maui.Alerts.Toast.Make($"Filter mode: {MainViewModel.SearchFilterMode}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        FilterPressed?.Invoke(this, e);
    }

    private void SetFilterKR(object sender, TappedEventArgs e)
    {
        MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.KR;
        CommunityToolkit.Maui.Alerts.Toast.Make($"Filter mode: {MainViewModel.SearchFilterMode}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        FilterPressed?.Invoke(this, e);
    }

    private void SetFilterJP(object sender, TappedEventArgs e)
    {
        MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.JP;
        CommunityToolkit.Maui.Alerts.Toast.Make($"Filter mode: {MainViewModel.SearchFilterMode}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        FilterPressed?.Invoke(this, e);
    }

    private void SetFilterEN(object sender, TappedEventArgs e)
    {
        MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.EN;
        CommunityToolkit.Maui.Alerts.Toast.Make($"Filter mode: {MainViewModel.SearchFilterMode}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        FilterPressed?.Invoke(this, e);
    }

    private void SetFilterFirstGen(object sender, TappedEventArgs e)
    {
        MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.Firstgen;
        Toast.Make($"Filter mode: {MainViewModel.SearchFilterMode}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        FilterPressed?.Invoke(this, e);
    }

    private void SetFilterSecondGen(object sender, TappedEventArgs e)
    {
        MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.Secondgen;
        Toast.Make($"Filter mode: {MainViewModel.SearchFilterMode}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        FilterPressed?.Invoke(this, e);
    }

    private void SetFilterThirdGen(object sender, TappedEventArgs e)
    {
        MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.Thirdgen;
        CommunityToolkit.Maui.Alerts.Toast.Make($"Filter mode: {MainViewModel.SearchFilterMode}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        FilterPressed?.Invoke(this, e);
    }

    private void SetFilterFourthGen(object sender, TappedEventArgs e)
    {
        MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.Fourthgen;
        CommunityToolkit.Maui.Alerts.Toast.Make($"Filter mode: {MainViewModel.SearchFilterMode}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        FilterPressed?.Invoke(this, e);
    }

    private void SetFilterFifthGen(object sender, TappedEventArgs e)
    {
        MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.Fifthgen;
        CommunityToolkit.Maui.Alerts.Toast.Make($"Filter mode: {MainViewModel.SearchFilterMode}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        FilterPressed?.Invoke(this, e);
    }

    private void RandomListView_ItemTapped(object sender, Syncfusion.Maui.ListView.ItemTappedEventArgs e)
    {
        RandomListView.SelectedItem = null;
    }
}