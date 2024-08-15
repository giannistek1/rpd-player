using CommunityToolkit.Maui.Alerts;
using RpdPlayerApp.Models;
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

        FemaleImage.Source = ImageSource.FromUri(new Uri("https://github.com/giannistek1/rpd-images/blob/main/home-female.jpg?raw=true"));
        MaleImage.Source = ImageSource.FromUri(new Uri("https://github.com/giannistek1/rpd-images/blob/main/home-male.png?raw=true"));
        
        FirstGenImage.Source = ImageSource.FromUri(new Uri("https://github.com/giannistek1/rpd-artists/blob/main/[BTS][Bangtan Sonyeondan][2013-06-13][BG][7][Big Hit Entertainment].jpg?raw=true"));
        SecondGenImage.Source = ImageSource.FromUri(new Uri("https://github.com/giannistek1/rpd-artists/blob/main/[BTS][Bangtan Sonyeondan][2013-06-13][BG][7][Big Hit Entertainment].jpg?raw=true"));
        ThirdGenImage.Source = ImageSource.FromUri(new Uri("https://github.com/giannistek1/rpd-artists/blob/main/[BTS][Bangtan Sonyeondan][2013-06-13][BG][7][Big Hit Entertainment].jpg?raw=true"));
        FourthGenImage.Source = ImageSource.FromUri(new Uri("https://github.com/giannistek1/rpd-artists/blob/main/[BTS][Bangtan Sonyeondan][2013-06-13][BG][7][Big Hit Entertainment].jpg?raw=true"));
        FifthGenImage.Source = ImageSource.FromUri(new Uri("https://github.com/giannistek1/rpd-artists/blob/main/[BTS][Bangtan Sonyeondan][2013-06-13][BG][7][Big Hit Entertainment].jpg?raw=true"));

        HybeImage.Source = ImageSource.FromUri(new Uri("https://github.com/giannistek1/rpd-images/blob/main/home-hybe-labels.webp?raw=true"));
        JYPImage.Source = ImageSource.FromUri(new Uri("https://github.com/giannistek1/rpd-images/blob/main/home-jyp.jpg?raw=true"));
        YGImage.Source = ImageSource.FromUri(new Uri("https://github.com/giannistek1/rpd-images/blob/main/home-yg.jpg?raw=true"));
        SMImage.Source = ImageSource.FromUri(new Uri("https://github.com/giannistek1/rpd-images/blob/main/home-sm.png?raw=true"));
        CubeImage.Source = ImageSource.FromUri(new Uri("https://github.com/giannistek1/rpd-images/blob/main/home-cube.webp?raw=true"));

        KRImage.Source = ImageSource.FromUri(new Uri("https://github.com/giannistek1/rpd-images/blob/main/home-sk.jpg?raw=true"));
        JPImage.Source = ImageSource.FromUri(new Uri("https://github.com/giannistek1/rpd-images/blob/main/home-jp.webp?raw=true"));
        ENImage.Source = ImageSource.FromUri(new Uri("https://github.com/giannistek1/rpd-images/blob/main/home-us.webp?raw=true"));

        SoloImage.Source = ImageSource.FromUri(new Uri("https://github.com/giannistek1/rpd-images/blob/main/home-solo.png?raw=true"));
        GroupImage.Source = ImageSource.FromUri(new Uri("https://github.com/giannistek1/rpd-images/blob/main/home-group.jpg?raw=true"));

        AllImage.Source = ImageSource.FromUri(new Uri("https://github.com/giannistek1/rpd-artists/blob/main/[BTS][Bangtan Sonyeondan][2013-06-13][BG][7][Big Hit Entertainment].jpg?raw=true"));

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
    #region Filters
    private void SetFilterFemale(object sender, TappedEventArgs e)
    {
        MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.Female;
        CommunityToolkit.Maui.Alerts.Toast.Make($"Filter mode: {MainViewModel.SearchFilterMode}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        FilterPressed?.Invoke(this, e);
    }

    private void SetFilterMale(object sender, TappedEventArgs e)
    {
        MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.Male;
        Toast.Make($"Filter mode: {MainViewModel.SearchFilterMode}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        FilterPressed?.Invoke(this, e);
    }


    private void SetFilterHybe(object sender, TappedEventArgs e)
    {
        MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.Hybe;
        Toast.Make($"Filter mode: {MainViewModel.SearchFilterMode}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        FilterPressed?.Invoke(this, e);
    }

    private void SetFilterSM(object sender, TappedEventArgs e)
    {
        MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.SM;
        Toast.Make($"Filter mode: {MainViewModel.SearchFilterMode}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        FilterPressed?.Invoke(this, e);
    }

    private void SetFilterJYP(object sender, TappedEventArgs e)
    {
        MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.JYP;
        Toast.Make($"Filter mode: {MainViewModel.SearchFilterMode}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        FilterPressed?.Invoke(this, e);
    }

    private void SetFilterYG(object sender, TappedEventArgs e)
    {
        MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.YG;
        CommunityToolkit.Maui.Alerts.Toast.Make($"Filter mode: {MainViewModel.SearchFilterMode}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        FilterPressed?.Invoke(this, e);
    }

    private void SetFilterCube(object sender, TappedEventArgs e)
    {
        MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.Cube;
        Toast.Make($"Filter mode: {MainViewModel.SearchFilterMode}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        FilterPressed?.Invoke(this, e);
    }

    private void SetFilterKR(object sender, TappedEventArgs e)
    {
        MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.KR;
        Toast.Make($"Filter mode: {MainViewModel.SearchFilterMode}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        FilterPressed?.Invoke(this, e);
    }

    private void SetFilterJP(object sender, TappedEventArgs e)
    {
        MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.JP;
        Toast.Make($"Filter mode: {MainViewModel.SearchFilterMode}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        FilterPressed?.Invoke(this, e);
    }

    private void SetFilterEN(object sender, TappedEventArgs e)
    {
        MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.EN;
        Toast.Make($"Filter mode: {MainViewModel.SearchFilterMode}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
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
        Toast.Make($"Filter mode: {MainViewModel.SearchFilterMode}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        FilterPressed?.Invoke(this, e);
    }

    private void SetFilterFourthGen(object sender, TappedEventArgs e)
    {
        MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.Fourthgen;
        Toast.Make($"Filter mode: {MainViewModel.SearchFilterMode}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        FilterPressed?.Invoke(this, e);
    }

    private void SetFilterFifthGen(object sender, TappedEventArgs e)
    {
        MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.Fifthgen;
        Toast.Make($"Filter mode: {MainViewModel.SearchFilterMode}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        FilterPressed?.Invoke(this, e);
    }

    private void SetFilterSolo(object sender, TappedEventArgs e)
    {
        MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.Solo;
        Toast.Make($"Filter mode: {MainViewModel.SearchFilterMode}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        FilterPressed?.Invoke(this, e);
    }

    private void SetFilterDuo(object sender, TappedEventArgs e)
    {
        MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.Duo;
        Toast.Make($"Filter mode: {MainViewModel.SearchFilterMode}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        FilterPressed?.Invoke(this, e);
    }

    private void SetFilterGroup(object sender, TappedEventArgs e)
    {
        MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.Group;
        Toast.Make($"Filter mode: {MainViewModel.SearchFilterMode}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        FilterPressed?.Invoke(this, e);
    }

    private void SetFilterNone(object sender, TappedEventArgs e)
    {
        MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.None;
        Toast.Make($"Filter mode: {MainViewModel.SearchFilterMode}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        FilterPressed?.Invoke(this, e);
    }
    #endregion

    private void RandomListView_ItemTapped(object sender, Syncfusion.Maui.ListView.ItemTappedEventArgs e)
    {
        RandomListView.SelectedItem = null;
    }
}