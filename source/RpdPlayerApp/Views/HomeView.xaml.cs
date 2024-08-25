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

        // TODO:
        string[] firstGens = {
            "[SHINHWA][][1998-03-24][BG][6][SM Entertainment].jpg",
        };

        string firstGenArtist = firstGens[new Random().Next(0, firstGens.Length)];

        FirstGenImage.Source = ImageSource.FromUri(new Uri($"https://github.com/giannistek1/rpd-artists/blob/main/{firstGenArtist}?raw=true"));

        string[] secondGens = {
            "[BIGBANG][][2006-08-19][BG][5][YG Entertainment].jpg",
            "[Girls Generation (SNSD)][SNSD][2007-08-05][GG][8][SM Entertainment].webp",
            "[2PM][][2008-09-04][BG][6][JYP Entertainment].jpg",
            "[SHINee][][2008-05-25][BG][5][SM Entertainment].webp",
            "[Super Junior][SUJU][2005-11-06][BG][10][SM Entertainment].webp",
            "[2NE1][][2009-05-06][GG][4][YG Entertainment].jpg"
        };

        string secondGenArtist = secondGens[new Random().Next(0, secondGens.Length)];
        SecondGenImage.Source = ImageSource.FromUri(new Uri($"https://github.com/giannistek1/rpd-artists/blob/main/{secondGenArtist}?raw=true"));

        string[] thirdGens = {
            "[BTS][Bangtan Sonyeondan][2013-06-13][BG][7][Big Hit Entertainment].jpg",
            "[EXO][][2012-04-08][BG][9][SM Entertainment].png",
            "[Twice][TWICE][2015-10-20][GG][9][JYP Entertainment].webp",
            "[Blackpink][BLACKPINK][2016-08-08][GG][4][YG Entertainment].jpg",
            "[GOT7][][2014-01-16][BG][7][JYP Entertainment].webp",
            "[Red Velvet][][2014-08-01][GG][5][SM Entertainment].jpg"
        };

        string thirdGenArtist = thirdGens[new Random().Next(0, thirdGens.Length)];
        ThirdGenImage.Source = ImageSource.FromUri(new Uri($"https://github.com/giannistek1/rpd-artists/blob/main/{thirdGenArtist}?raw=true"));
        
        string[] fourthGens = { 
            "[Stray Kids][SKZ][2018-03-25][BG][8][JYP Entertainment].jpg",
            "[ATEEZ][][2018-10-24][BG][8][KQ Entertainment].jpeg", 
            "[ITZY][][2019-02-12][GG][5][JYP Entertainment].webp",
            "[TXT][Tomorrow X Together][2019-03-04][BG][5][Big Hit Entertainment].jpg",
            "[aespa][][2020-11-17][GG][4][SM Entertainment].jpg" 
        };

        string fourthGenArtist = fourthGens[new Random().Next(0, fourthGens.Length)];
        FourthGenImage.Source = ImageSource.FromUri(new Uri($"https://github.com/giannistek1/rpd-artists/blob/main/{fourthGenArtist}?raw=true"));

        string[] fifthGens = {
            "[RIIZE][][2023-09-04][BG][7][SM Entertainment].jpg",
            "[ZEROBASEONE][ZB1][2023-07-10][BG][9][Wake One].jpg",
            "[BABYMONSTER][][2024-04-01][GG][7][YG Entertainment].jpg",
            "[KISS OF LIFE][][2023-07-05][GG][4][S2 Entertainment].jpg"
        };

        string fifthGenArtist = fifthGens[new Random().Next(0, fifthGens.Length)];
        FifthGenImage.Source = ImageSource.FromUri(new Uri($"https://github.com/giannistek1/rpd-artists/blob/main/{fifthGenArtist}?raw=true"));

        HybeImage.Source = ImageSource.FromUri(new Uri("https://github.com/giannistek1/rpd-images/blob/main/home-hybe-labels.webp?raw=true"));
        JYPImage.Source = ImageSource.FromUri(new Uri("https://github.com/giannistek1/rpd-images/blob/main/home-jyp.jpg?raw=true"));
        YGImage.Source = ImageSource.FromUri(new Uri("https://github.com/giannistek1/rpd-images/blob/main/home-yg.jpg?raw=true"));
        SMImage.Source = ImageSource.FromUri(new Uri("https://github.com/giannistek1/rpd-images/blob/main/home-sm.png?raw=true"));
        CubeImage.Source = ImageSource.FromUri(new Uri("https://github.com/giannistek1/rpd-images/blob/main/home-cube.webp?raw=true"));
        FncImage.Source = ImageSource.FromUri(new Uri("https://github.com/giannistek1/rpd-images/blob/main/home-fnc.png?raw=true"));
        PledisImage.Source = ImageSource.FromUri(new Uri("https://github.com/giannistek1/rpd-images/blob/main/home-pledis.webp?raw=true"));
        StarshipImage.Source = ImageSource.FromUri(new Uri("https://github.com/giannistek1/rpd-images/blob/main/home-starship.webp?raw=true"));

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
    private void SetFilter(object sender, TappedEventArgs e)
    {
        string filterMode = e.Parameter.ToString();

        switch(filterMode)
        {
            case "none": MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.None; break;

            case "female": MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.Female; break;
            case "male": MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.Male; break;

            case "firstgen": MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.Firstgen; break;
            case "secondgen": MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.Secondgen; break;
            case "thirdgen": MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.Thirdgen; break;
            case "fourthgen": MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.Fourthgen; break;
            case "fifthgen": MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.Fifthgen; break;

            case "hybe": MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.Hybe; break;
            case "jyp": MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.JYP; break;
            case "yg": MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.YG; break;
            case "sm": MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.SM; break;
            case "cube": MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.Cube; break;
            case "fnc": MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.FNC; break;
            case "pledis": MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.Pledis; break;
            case "starship": MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.Starship; break;

            case "solo": MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.Solo; break;
            case "group": MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.Group; break;
            case "trio": MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.Trio; break;
            case "quadruplet": MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.Quadruplet; break;
            case "quintet": MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.Quintet; break;
            case "sextet": MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.Sextet; break;
            case "septet": MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.Septet; break;
            case "octet": MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.Octet; break;
            case "nonet": MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.Nonet; break;

            case "kr": MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.KR; break;
            case "jp": MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.JP; break;
            case "en": MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.EN; break;
            case "th": MainViewModel.SearchFilterMode = Architecture.SearchFilterMode.TH; break;
        }

        CommunityToolkit.Maui.Alerts.Toast.Make($"Filter mode: {MainViewModel.SearchFilterMode}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        FilterPressed?.Invoke(this, e);
    }
    #endregion

    double originalWidth = 70.0;
    double originalHeight = 70.0;
    private void ZoomImage(object sender, TappedEventArgs e)
    {
        Image image = (Image)e.Parameter;
        
        if ((int)image.Width == (int)originalWidth)
        {
            image.WidthRequest = 170;
            image.HeightRequest = 170;
        }
        else
        {
            image.WidthRequest = originalWidth;
            image.HeightRequest = originalHeight;
        }
    }
}