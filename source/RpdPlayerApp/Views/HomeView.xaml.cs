using RpdPlayerApp.Architecture;
using RpdPlayerApp.Items;
using RpdPlayerApp.Models;
using RpdPlayerApp.Repositories;
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

        // TODO:
        string[] firstGens = {
            "[SHINHWA][][1998-03-24][BG][6][SM Entertainment].jpg",
        };

        string firstGenArtist = firstGens[new Random().Next(0, firstGens.Length)];

        string[] secondGens = {
            "[BIGBANG][][2006-08-19][BG][5][YG Entertainment].jpg",
            "[Girls Generation (SNSD)][SNSD][2007-08-05][GG][8][SM Entertainment].webp",
            "[2PM][][2008-09-04][BG][6][JYP Entertainment].jpg",
            "[SHINee][][2008-05-25][BG][5][SM Entertainment].webp",
            "[Super Junior][SUJU][2005-11-06][BG][10][SM Entertainment].webp",
            "[2NE1][][2009-05-06][GG][4][YG Entertainment].jpg"
        };

        string secondGenArtist = secondGens[new Random().Next(0, secondGens.Length)];

        string[] thirdGens = {
            "[BTS][Bangtan Sonyeondan][2013-06-13][BG][7][Big Hit Entertainment].jpg",
            "[EXO][][2012-04-08][BG][9][SM Entertainment].png",
            "[Twice][TWICE][2015-10-20][GG][9][JYP Entertainment].webp",
            "[Blackpink][BLACKPINK][2016-08-08][GG][4][YG Entertainment].jpg",
            "[GOT7][][2014-01-16][BG][7][JYP Entertainment].webp",
            "[Red Velvet][RV][2014-08-01][GG][5][SM Entertainment].jpg"
        };

        string thirdGenArtist = thirdGens[new Random().Next(0, thirdGens.Length)];

        string[] fourthGens = {
            "[Stray Kids][SKZ][2018-03-25][BG][8][JYP Entertainment].jpg",
            "[ATEEZ][][2018-10-24][BG][8][KQ Entertainment].jpeg",
            "[ITZY][][2019-02-12][GG][5][JYP Entertainment].webp",
            "[TXT][Tomorrow X Together][2019-03-04][BG][5][Big Hit Entertainment].jpg",
            "[aespa][][2020-11-17][GG][4][SM Entertainment].jpg"
        };

        string fourthGenArtist = fourthGens[new Random().Next(0, fourthGens.Length)];

        string[] fifthGens = {
            "[RIIZE][][2023-09-04][BG][7][SM Entertainment].jpg",
            "[ZEROBASEONE][ZB1][2023-07-10][BG][9][Wake One].jpg",
            "[BABYMONSTER][][2024-04-01][GG][7][YG Entertainment].jpg",
            "[KISS OF LIFE][][2023-07-05][GG][4][S2 Entertainment].jpg"
        };

        string fifthGenArtist = fifthGens[new Random().Next(0, fifthGens.Length)];

        // Get
        ArtistRepository.GetArtists(); 
        AlbumRepository.GetAlbums();
        VideoRepository.GetVideos();
        SongPartRepository.GetSongParts();

        FemaleImage.Source = ImageSource.FromUri(new Uri("https://github.com/giannistek1/rpd-images/blob/main/home-female.jpg?raw=true"));
        MaleImage.Source = ImageSource.FromUri(new Uri("https://github.com/giannistek1/rpd-images/blob/main/home-male.png?raw=true"));

        SoloImage.Source = ImageSource.FromUri(new Uri("https://github.com/giannistek1/rpd-images/blob/main/home-solo.png?raw=true"));
        GroupImage.Source = ImageSource.FromUri(new Uri("https://github.com/giannistek1/rpd-images/blob/main/home-group.jpg?raw=true"));

        AllImage.Source = ImageSource.FromUri(new Uri("https://github.com/giannistek1/rpd-images/blob/main/home-all.png?raw=true"));
        DancePracticeImage.Source = ImageSource.FromUri(new Uri("https://github.com/giannistek1/rpd-images/blob/main/home-dance-practice.jpg?raw=true"));

        GenerationListView.ItemsSource = new List<HomeListViewItem>() { 
            new HomeListViewItem(title: "1st Generation", 
                                description: "First generation.", 
                                imageUrl: $"https://github.com/giannistek1/rpd-artists/blob/main/{firstGenArtist}?raw=true", 
                                searchFilterMode: SearchFilterMode.Firstgen, 
                                songCount: SongPartRepository.SongParts.Count(s => s.Artist?.Generation == MainViewModel.FIRST_GENERATION && s.Album?.GenreShort == "KR")
                                ), 

            new HomeListViewItem(title: "2nd Generation", 
                                description: "Second generation.", 
                                imageUrl: $"https://github.com/giannistek1/rpd-artists/blob/main/{secondGenArtist}?raw=true", 
                                searchFilterMode: SearchFilterMode.Secondgen,
                                songCount: SongPartRepository.SongParts.Count(s => s.Artist?.Generation == MainViewModel.SECOND_GENERATION && s.Album?.GenreShort == "KR")
                                ),

            new HomeListViewItem(title: "3rd Generation", 
                                description: "Third generation.", 
                                imageUrl: $"https://github.com/giannistek1/rpd-artists/blob/main/{thirdGenArtist}?raw=true", 
                                searchFilterMode: SearchFilterMode.Thirdgen,
                                songCount: SongPartRepository.SongParts.Count(s => s.Artist?.Generation == MainViewModel.THIRD_GENERATION && s.Album?.GenreShort == "KR")
                                ), 

            new HomeListViewItem(title: "4th Generation", 
                                description: "Fourth generation.", 
                                imageUrl: $"https://github.com/giannistek1/rpd-artists/blob/main/{fourthGenArtist}?raw=true", 
                                searchFilterMode: SearchFilterMode.Fourthgen,
                                songCount: SongPartRepository.SongParts.Count(s => s.Artist?.Generation == MainViewModel.FOURTH_GENERATION && s.Album?.GenreShort == "KR")
                                ),

            new HomeListViewItem(title: "5th Generation", 
                                description: "Fifth generation.", 
                                imageUrl: $"https://github.com/giannistek1/rpd-artists/blob/main/{fifthGenArtist}?raw=true", 
                                searchFilterMode: SearchFilterMode.Fifthgen, 
                                songCount : SongPartRepository.SongParts.Count(s => s.Artist?.Generation == MainViewModel.FIFTH_GENERATION && s.Album?.GenreShort == "KR")
                                ) 
        };

        CompanyListView.ItemsSource = new List<HomeListViewItem>() {
            new HomeListViewItem(title: "SM Entertainment",
                                description: "SM Entertainment.",
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-sm.png?raw=true",
                                searchFilterMode: SearchFilterMode.SM,
                                songCount: SongPartRepository.SongParts.Count(s => s.Artist?.Company == "SM Entertainment")
                                ),

            new HomeListViewItem(title: "HYBE Labels",
                                 description: "HYBE Labels, formerly known as Big Hit Entertainment with child company: Source Music",
                                 imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-hybe-labels.webp?raw=true",
                                 searchFilterMode: SearchFilterMode.Hybe,
                                 songCount: SongPartRepository.SongParts.Count(s => s.Artist?.Company == "HYBE Labels" ||
                                                                                    s.Artist?.Company == "Big Hit Entertainment" ||
                                                                                    s.Artist?.Company == "Source Music")
                                 ),

            new HomeListViewItem(title: "JYP Entertainment",
                                description: "JYP Entertainment.",
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-jyp.jpg?raw=true", 
                                searchFilterMode: SearchFilterMode.JYP,
                                songCount: SongPartRepository.SongParts.Count(s => s.Artist?.Company == "YG Entertainment")
                                ),

            new HomeListViewItem(title: "YG Entertainment", 
                                description: "YG Entertainment.", 
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-yg.jpg?raw=true", 
                                searchFilterMode: SearchFilterMode.YG,
                                songCount: SongPartRepository.SongParts.Count(s => s.Artist?.Company == "YG Entertainment" || 
                                                                                   s.Artist?.Company == "The Black Label")
                                ),

            new HomeListViewItem(title: "Kakao Entertainment",
                                 description: "Kakao Entertainment. Has many child companies: IST Entertainment, Starship Entertainment, EDAM Entertainment, Bluedot Entertainment, High Up Entertainment, Antenna and FLEX M.",
                                 imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-kakao.webp?raw=true",
                                 searchFilterMode: SearchFilterMode.Kakao_Entertainment,
                                 songCount: SongPartRepository.SongParts.Count(s => s.Artist?.Company == "IST Entertainment" || // Went through a lot of renaming: A Cube -> Play A -> PLay M
                                                                                    s.Artist?.Company == "Starship Entertainment" ||
                                                                                    s.Artist?.Company == "EDAM Entertainment" ||
                                                                                    s.Artist?.Company == "Bluedot Entertainment" ||
                                                                                    s.Artist?.Company == "High Up Entertainment" ||
                                                                                    s.Artist?.Company == "Antenna" ||
                                                                                    s.Artist?.Company == "FLEX M")
                                 ),

            new HomeListViewItem(title: "Starship Entertainment",
                                description: "Starship Entertainment.",
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-starship.webp?raw=true",
                                searchFilterMode: SearchFilterMode.Starship,
                                songCount: SongPartRepository.SongParts.Count(s => s.Artist ?.Company == "Starship Entertainment")
                                ),

            new HomeListViewItem(title: "RBW Entertainment",
                                description: "Rainbow Bridge World Entertainment, includes WM entertainment.",
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-rbw.png?raw=true",
                                searchFilterMode: SearchFilterMode.RBW,
                                songCount : SongPartRepository.SongParts.Count(s => s.Artist ?.Company == "RBW Entertainment")
                                ),

            new HomeListViewItem(title: "Cube Entertainment", 
                                description: "Cube Entertainment.", 
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-cube.webp?raw=true", 
                                searchFilterMode: SearchFilterMode.Cube, 
                                songCount: SongPartRepository.SongParts.Count(s => s.Artist ?.Company == "Cube Entertainment")
                                ),

            new HomeListViewItem(title: "IST Entertainment",
                                description: "IST Entertainment. Formerly known as A Cube, Play A, Play M.",
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-ist.webp?raw=true",
                                searchFilterMode: SearchFilterMode.IST,
                                songCount : SongPartRepository.SongParts.Count(s => s.Artist ?.Company == "IST Entertainment")
                                ),

            new HomeListViewItem(title: "Pledis Entertainment", 
                                description: "Pledis Entertainment.", 
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-pledis.webp?raw=true", 
                                searchFilterMode: SearchFilterMode.Pledis, 
                                songCount: SongPartRepository.SongParts.Count(s => s.Artist ?.Company == "Pledis Entertainment")
                                ),

            new HomeListViewItem(title: "CJ ENM Music",
                                 description: "CJ ENM Music.",
                                 imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-cjenm.webp?raw=true",
                                 searchFilterMode: SearchFilterMode.CJ_ENM_Music,
                                 songCount: SongPartRepository.SongParts.Count(s => s.Artist?.Company == "AOMG" ||
                                                                                    s.Artist?.Company == "B2M Entertainment" ||
                                                                                    s.Artist?.Company == "Jellyfish Entertainment" ||
                                                                                    s.Artist?.Company == "Wake One" || // Formerly known as MMO Entertainment
                                                                                    s.Artist?.Company == "Stone Music Entertainment")
                                 ),

            new HomeListViewItem(title: "FNC Entertainment",
                                description: "FNC Entertainment.",
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-fnc.png?raw=true",
                                searchFilterMode: SearchFilterMode.FNC,
                                songCount : SongPartRepository.SongParts.Count(s => s.Artist ?.Company == "FNC Entertainment")
                                ),

            new HomeListViewItem(title: "Woollim Entertainment",
                                description: "Woollim Entertainment.",
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-woollim.webp?raw=true",
                                searchFilterMode: SearchFilterMode.Woollim,
                                songCount : SongPartRepository.SongParts.Count(s => s.Artist ?.Company == "Woollim Entertainment")
                                ),
        };

        GenreListView.ItemsSource = new List<HomeListViewItem>() {
            new HomeListViewItem(title: "K-pop", 
                                description: "Korean pop music.", 
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-sk.jpg?raw=true", 
                                searchFilterMode: SearchFilterMode.KR,
                                songCount: SongPartRepository.SongParts.Count(s => s.Album?.GenreShort == "KR")
                                ),

            new HomeListViewItem(title: "J-pop", 
                                description: "Japanese pop music.", 
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-jp.webp?raw=true", 
                                searchFilterMode: SearchFilterMode.JP,
                                songCount: SongPartRepository.SongParts.Count(s => s.Album?.GenreShort == "JP")
                                ),

            new HomeListViewItem(title: "English pop", 
                                description: "English pop music.", 
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-us.webp?raw=true", 
                                searchFilterMode: SearchFilterMode.EN,
                                songCount: SongPartRepository.SongParts.Count(s => s.Album?.GenreShort == "EN")
                                ),

            new HomeListViewItem(title: "C-pop", 
                                description: "Chinese pop music.", 
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-ch.png?raw=true", 
                                searchFilterMode: SearchFilterMode.CH,
                                songCount: SongPartRepository.SongParts.Count(s => s.Album?.GenreShort == "CH")
                                ),

            new HomeListViewItem(title: "T-pop", 
                                description: "Thai pop music.", 
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-th.webp?raw=true", 
                                searchFilterMode: SearchFilterMode.TH,
                                songCount: SongPartRepository.SongParts.Count(s => s.Album?.GenreShort == "TH")
                                )
        };

        KpopYearsListView.ItemsSource = new List<HomeListViewItem>() {
            new HomeListViewItem(title: "2019",
                                description: "K-pop 2019",
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-sk.jpg?raw=true",
                                searchFilterMode: SearchFilterMode.kpop2019,
                                songCount: SongPartRepository.SongParts.Count(s => s.Album?.ReleaseDate.Year == 2019)
                                ),

            new HomeListViewItem(title: "2020",
                                description: "K-pop 2020",
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-sk.jpg?raw=true",
                                searchFilterMode: SearchFilterMode.kpop2020,
                                songCount: SongPartRepository.SongParts.Count(s => s.Album?.ReleaseDate.Year == 2020)
                                ),

            new HomeListViewItem(title: "2021",
                                description: "K-pop 2021",
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-sk.jpg?raw=true",
                                searchFilterMode: SearchFilterMode.kpop2021,
                                songCount: SongPartRepository.SongParts.Count(s => s.Album?.ReleaseDate.Year == 2021)
                                ),

            new HomeListViewItem(title: "2022",
                                description: "K-pop 2022",
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-sk.jpg?raw=true",
                                searchFilterMode: SearchFilterMode.kpop2022,
                                songCount: SongPartRepository.SongParts.Count(s => s.Album?.ReleaseDate.Year == 2022)
                                ),

            new HomeListViewItem(title: "2023",
                                description: "K-pop 2023",
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-sk.jpg?raw=true",
                                searchFilterMode: SearchFilterMode.kpop2023,
                                songCount: SongPartRepository.SongParts.Count(s => s.Album?.ReleaseDate.Year == 2023)
                                ),

            new HomeListViewItem(title: "2024",
                                description: "K-pop 2024",
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-sk.jpg?raw=true",
                                searchFilterMode: SearchFilterMode.kpop2024,
                                songCount: SongPartRepository.SongParts.Count(s => s.Album?.ReleaseDate.Year == 2024)
                                )
        };

        var groupedTitles = from s in SongPartRepository.SongParts
                            group s.Title by s.Title into g
                            select new { Title = g.Key, Titles = g.ToList() };

        UniqueSongCountLabel.Text = $", Unique songs: {groupedTitles.Count()}";
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
        if (e.Parameter is null) { return; } 

        string filterMode = e.Parameter.ToString();

        switch (filterMode)
        {
            case "none":
            case "all":MainViewModel.SearchFilterMode = SearchFilterMode.All; break;
            case "hasdancevideo": MainViewModel.SearchFilterMode = SearchFilterMode.DanceVideos; break;

            case "female": MainViewModel.SearchFilterMode = SearchFilterMode.Female; break;
            case "male": MainViewModel.SearchFilterMode = SearchFilterMode.Male; break;
            case "mixed": MainViewModel.SearchFilterMode = SearchFilterMode.Mixed; break;

            case "solo": MainViewModel.SearchFilterMode = SearchFilterMode.Solo; break;
            case "group": MainViewModel.SearchFilterMode = SearchFilterMode.Group; break;
            case "trio": MainViewModel.SearchFilterMode = SearchFilterMode.Trio; break;
            case "quadruplet": MainViewModel.SearchFilterMode = SearchFilterMode.Quadruplet; break;
            case "quintet": MainViewModel.SearchFilterMode = SearchFilterMode.Quintet; break;
            case "sextet": MainViewModel.SearchFilterMode = SearchFilterMode.Sextet; break;
            case "septet": MainViewModel.SearchFilterMode = SearchFilterMode.Septet; break;
            case "octet": MainViewModel.SearchFilterMode = SearchFilterMode.Octet; break;
            case "nonet": MainViewModel.SearchFilterMode = SearchFilterMode.Nonet; break;
        }

        //CommunityToolkit.Maui.Alerts.Toast.Make($"Filter mode: {MainViewModel.SearchFilterMode}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();

        FilterPressed?.Invoke(this, e);
    }

    private void CategoryListViewItemTapped(object sender, Syncfusion.Maui.ListView.ItemTappedEventArgs e)
    {
        if (e.ItemType != Syncfusion.Maui.ListView.ItemType.Record)
            return;

        HomeListViewItem item = (HomeListViewItem)e.DataItem;

        if (item is not null)
        {
            MainViewModel.SearchFilterMode = item.SearchFilterMode;
        }

        //CommunityToolkit.Maui.Alerts.Toast.Make($"Filter mode: {MainViewModel.SearchFilterMode}", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
        FilterPressed?.Invoke(this, e);
    }
    #endregion

    private void FeedbackButton_Pressed(object sender, EventArgs e)
    {

    }

    private async void SettingsButton_Pressed(object sender, EventArgs e)
    {
        // TODO: Bug: If you open settingspage, listviewitems and audioplayercontrol break
        if (Navigation.NavigationStack.Count < 2)
        {
            await Navigation.PushAsync(new SettingsPage(), true);
        }
    }
}