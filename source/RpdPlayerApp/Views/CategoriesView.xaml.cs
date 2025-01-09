using RpdPlayerApp.Architecture;
using RpdPlayerApp.Items;
using RpdPlayerApp.Repositories;
using RpdPlayerApp.ViewModels;
using Syncfusion.Maui.Buttons;

namespace RpdPlayerApp.Views;

public partial class CategoriesView : ContentView
{
    internal event EventHandler? FilterPressed;

    private const string ARTISTS_URL = "https://github.com/giannistek1/rpd-artists/blob/main/";

    public CategoriesView()
	{
		InitializeComponent();

        this.Loaded += OnLoad;

        AllImageButton.Pressed += SetFilter;
        DanceImageButton.Pressed += SetFilter;
        MaleImageButton.Pressed += SetFilter;
        FemaleImageButton.Pressed += SetFilter;
        SoloImageButton.Pressed += SetFilter;
        GroupImageButton.Pressed += SetFilter;
    }

    private void OnLoad(object? sender, EventArgs e)
    {
        // TODO:
        string[] firstGens = [
            "[SHINHWA][][1998-03-24][BG][6][SM Entertainment].jpg",
        ];

        string firstGenArtist = firstGens[new Random().Next(0, firstGens.Length)];

        string[] secondGens = [
            "%5BBIGBANG%5D%5B%5D%5B2006-08-19%5D%5BBG%5D%5B5%5D%5BYG%20Entertainment%5D.jpg",
            "%5BGirls%20Generation%20(SNSD)%5D%5BSNSD%2C%20GG%5D%5B2007-08-05%5D%5BGG%5D%5B8%5D%5BSM%20Entertainment%5D.webp",
            "%5B2PM%5D%5BHottest%20time%20of%20the%20day%5D%5B2008-09-04%5D%5BBG%5D%5B6%5D%5BJYP%20Entertainment%5D.jpg",
            "%5BSHINee%5D%5B%5D%5B2008-05-25%5D%5BBG%5D%5B5%5D%5BSM%20Entertainment%5D.webp",
            "%5BSuper%20Junior%5D%5BSUJU%5D%5B2005-11-06%5D%5BBG%5D%5B10%5D%5BSM%20Entertainment%5D.webp",
            "%5B2NE1%5D%5B21%5D%5B2009-05-06%5D%5BGG%5D%5B4%5D%5BYG%20Entertainment%5D.jpg"
        ];

        string secondGenArtist = secondGens[new Random().Next(0, secondGens.Length)];

        string[] thirdGens = [
            "[BTS][Bangtan Sonyeondan][2013-06-13][BG][7][Big Hit Entertainment].jpg",
            "[EXO][][2012-04-08][BG][9][SM Entertainment].png",
            "[Twice][TWICE][2015-10-20][GG][9][JYP Entertainment].webp",
            "[Blackpink][BLACKPINK][2016-08-08][GG][4][YG Entertainment].jpg",
            "[GOT7][][2014-01-16][BG][7][JYP Entertainment].webp",
            "[Red Velvet][RV][2014-08-01][GG][5][SM Entertainment].jpg"
        ];

        string thirdGenArtist = thirdGens[new Random().Next(0, thirdGens.Length)];

        string[] fourthGens = [
            "%5BStray%20Kids%5D%5BSKZ%5D%5B2018-03-25%5D%5BBG%5D%5B8%5D%5BJYP%20Entertainment%5D.jpg",
            "%5BATEEZ%5D%5BKQ%20Fellaz%5D%5B2018-10-24%5D%5BBG%5D%5B8%5D%5BKQ%20Entertainment%5D.jpeg",
            "%5BITZY%5D%5B%5D%5B2019-02-12%5D%5BGG%5D%5B5%5D%5BJYP%20Entertainment%5D.webp",
            "%5BTXT%5D%5BTomorrow%20X%20Together%5D%5B2019-03-04%5D%5BBG%5D%5B5%5D%5BBig%20Hit%20Entertainment%5D.jpg",
            "%5Baespa%5D%5BAvatar%20x%20experience%5D%5B2020-11-17%5D%5BGG%5D%5B4%5D%5BSM%20Entertainment%5D.jpg"
        ];

        string fourthGenArtist = fourthGens[new Random().Next(0, fourthGens.Length)];

        string[] fifthGens = [
            "%5BRIIZE%5D%5B%5D%5B2023-09-04%5D%5BBG%5D%5B7%5D%5BSM%20Entertainment%5D.jpg",
            "%5BZEROBASEONE%5D%5BZB1%5D%5B2023-07-10%5D%5BBG%5D%5B9%5D%5BWake%20One%5D.jpg",
            "%5BBABYMONSTER%5D%5BBM%5D%5B2024-04-01%5D%5BGG%5D%5B7%5D%5BYG%20Entertainment%5D.jpg",
            "%5BKISS%20OF%20LIFE%5D%5BKIOF%5D%5B2023-07-05%5D%5BGG%5D%5B4%5D%5BS2%20Entertainment%5D.jpg"
        ];

        string fifthGenArtist = fifthGens[new Random().Next(0, fifthGens.Length)];


        GenerationListView.ItemsSource = new List<HomeListViewItem>() {
            new(title: "1st Generation",
                                description: "First generation.",
                                imageUrl: $"{ARTISTS_URL}{firstGenArtist}?raw=true",
                                searchFilterMode: SearchFilterMode.Firstgen,
                                songCount: SongPartRepository.SongParts.Count(s => s.Artist?.Generation == MainViewModel.FIRST_GENERATION && s.Album?.GenreShort == "KR")
                                ),

            new(title: "2nd Generation",
                                description: "Second generation.",
                                imageUrl: $"{ARTISTS_URL}{secondGenArtist}?raw=true",
                                searchFilterMode: SearchFilterMode.Secondgen,
                                songCount: SongPartRepository.SongParts.Count(s => s.Artist?.Generation == MainViewModel.SECOND_GENERATION && s.Album?.GenreShort == "KR")
                                ),

            new(title: "3rd Generation",
                                description: "Third generation.",
                                imageUrl: $"{ARTISTS_URL}{thirdGenArtist}?raw=true",
                                searchFilterMode: SearchFilterMode.Thirdgen,
                                songCount: SongPartRepository.SongParts.Count(s => s.Artist?.Generation == MainViewModel.THIRD_GENERATION && s.Album?.GenreShort == "KR")
                                ),

            new(title: "4th Generation",
                                description: "Fourth generation.",
                                imageUrl: $"{ARTISTS_URL}{fourthGenArtist}?raw=true",
                                searchFilterMode: SearchFilterMode.Fourthgen,
                                songCount: SongPartRepository.SongParts.Count(s => s.Artist?.Generation == MainViewModel.FOURTH_GENERATION && s.Album?.GenreShort == "KR")
                                ),

            new(title: "5th Generation",
                                description: "Fifth generation.",
                                imageUrl: $"{ARTISTS_URL}{fifthGenArtist}?raw=true",
                                searchFilterMode: SearchFilterMode.Fifthgen,
                                songCount : SongPartRepository.SongParts.Count(s => s.Artist?.Generation == MainViewModel.FIFTH_GENERATION && s.Album?.GenreShort == "KR")
                                )
        };

        CompanyListView.ItemsSource = new List<HomeListViewItem>() {
            new(title: "SM Entertainment",
                                description: "SM Entertainment.",
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-sm.png?raw=true",
                                searchFilterMode: SearchFilterMode.SM,
                                songCount: SongPartRepository.SongParts.Count(s => s.Artist?.Company == "SM Entertainment" ||
                                                                                   s.Artist?.Company == "Label V" ||
                                                                                   s.Artist?.Company == "Mystic Story")
                                ),

            new(title: "HYBE Labels",
                                 description: "HYBE Labels, formerly known as Big Hit Entertainment with child company: Source Music",
                                 imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-hybe-labels.webp?raw=true",
                                 searchFilterMode: SearchFilterMode.Hybe,
                                 songCount: SongPartRepository.SongParts.Count(s => s.Artist?.Company == "HYBE Labels" ||
                                                                                    s.Artist?.Company == "Big Hit Entertainment" ||
                                                                                    s.Artist?.Company == "Source Music" ||
                                                                                    s.Artist?.Company == "Pledis Entertainment")
                                 ),

            new(title: "JYP Entertainment",
                                description: "JYP Entertainment.",
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-jyp.jpg?raw=true",
                                searchFilterMode: SearchFilterMode.JYP,
                                songCount: SongPartRepository.SongParts.Count(s => s.Artist?.Company == "JYP Entertainment")
                                ),

            new(title: "YG Entertainment",
                                description: "YG Entertainment.",
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-yg.jpg?raw=true",
                                searchFilterMode: SearchFilterMode.YG,
                                songCount: SongPartRepository.SongParts.Count(s => s.Artist?.Company == "YG Entertainment" ||
                                                                                   s.Artist?.Company == "The Black Label")
                                ),

            new(title: "Kakao Entertainment",
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

            new(title: "Starship Entertainment",
                                description: "Starship Entertainment.",
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-starship.webp?raw=true",
                                searchFilterMode: SearchFilterMode.Starship,
                                songCount: SongPartRepository.SongParts.Count(s => s.Artist ?.Company == "Starship Entertainment")
                                ),

            new(title: "RBW Entertainment",
                                description: "Rainbow Bridge World Entertainment, includes WM entertainment.",
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-rbw.png?raw=true",
                                searchFilterMode: SearchFilterMode.RBW,
                                songCount : SongPartRepository.SongParts.Count(s => s.Artist ?.Company == "RBW Entertainment")
                                ),

            new(title: "Cube Entertainment",
                                description: "Cube Entertainment.",
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-cube.webp?raw=true",
                                searchFilterMode: SearchFilterMode.Cube,
                                songCount: SongPartRepository.SongParts.Count(s => s.Artist ?.Company == "Cube Entertainment")
                                ),

            new(title: "IST Entertainment",
                                description: "IST Entertainment. Formerly known as A Cube, Play A, Play M.",
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-ist.webp?raw=true",
                                searchFilterMode: SearchFilterMode.IST,
                                songCount : SongPartRepository.SongParts.Count(s => s.Artist ?.Company == "IST Entertainment")
                                ),

            new(title: "Pledis Entertainment",
                                description: "Pledis Entertainment.",
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-pledis.webp?raw=true",
                                searchFilterMode: SearchFilterMode.Pledis,
                                songCount: SongPartRepository.SongParts.Count(s => s.Artist ?.Company == "Pledis Entertainment")
                                ),

            new(title: "CJ ENM Music",
                                 description: "CJ ENM Music.",
                                 imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-cjenm.webp?raw=true",
                                 searchFilterMode: SearchFilterMode.CJ_ENM_Music,
                                 songCount: SongPartRepository.SongParts.Count(s => s.Artist?.Company == "AOMG" ||
                                                                                    s.Artist?.Company == "B2M Entertainment" ||
                                                                                    s.Artist?.Company == "Jellyfish Entertainment" ||
                                                                                    s.Artist?.Company == "Wake One" || // Formerly known as MMO Entertainment
                                                                                    s.Artist?.Company == "Stone Music Entertainment" ||
                                                                                    s.Artist?.Company == "Swing Entertainment")
                                 ),

            new(title: "FNC Entertainment",
                                description: "FNC Entertainment.",
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-fnc.png?raw=true",
                                searchFilterMode: SearchFilterMode.FNC,
                                songCount : SongPartRepository.SongParts.Count(s => s.Artist ?.Company == "FNC Entertainment")
                                ),

            new(title: "Woollim Entertainment",
                                description: "Woollim Entertainment.",
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-woollim.webp?raw=true",
                                searchFilterMode: SearchFilterMode.Woollim,
                                songCount : SongPartRepository.SongParts.Count(s => s.Artist ?.Company == "Woollim Entertainment")
                                ),
        };

        GenreListView.ItemsSource = new List<HomeListViewItem>() {
            new(title: "K-pop",
                                description: "Korean pop music.",
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-sk.jpg?raw=true",
                                searchFilterMode: SearchFilterMode.KR,
                                songCount: SongPartRepository.SongParts.Count(s => s.Album?.GenreShort == "KR")
                                ),

            new(title: "J-pop",
                                description: "Japanese pop music.",
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-jp.webp?raw=true",
                                searchFilterMode: SearchFilterMode.JP,
                                songCount: SongPartRepository.SongParts.Count(s => s.Album?.GenreShort == "JP")
                                ),

            new(title: "English pop",
                                description: "English pop music.",
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-us.webp?raw=true",
                                searchFilterMode: SearchFilterMode.EN,
                                songCount: SongPartRepository.SongParts.Count(s => s.Album?.GenreShort == "EN")
                                ),

            new(title: "C-pop",
                                description: "Chinese pop music.",
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-ch.png?raw=true",
                                searchFilterMode: SearchFilterMode.CH,
                                songCount: SongPartRepository.SongParts.Count(s => s.Album?.GenreShort == "CH")
                                ),

            new(title: "T-pop",
                                description: "Thai pop music.",
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-th.webp?raw=true",
                                searchFilterMode: SearchFilterMode.TH,
                                songCount: SongPartRepository.SongParts.Count(s => s.Album?.GenreShort == "TH")
                                )
        };

        KpopYearsListView.ItemsSource = new List<HomeListViewItem>() {
            new(title: "2017",
                                description: "K-pop 2017",
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-sk.jpg?raw=true",
                                searchFilterMode: SearchFilterMode.kpop2017,
                                songCount: SongPartRepository.SongParts.Count(s => s.Album?.ReleaseDate.Year == 2017 && s.Album?.GenreShort == "KR")
                                ),

            new(title: "2018",
                                description: "K-pop 2018",
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-sk.jpg?raw=true",
                                searchFilterMode: SearchFilterMode.kpop2018,
                                songCount: SongPartRepository.SongParts.Count(s => s.Album?.ReleaseDate.Year == 2018 && s.Album?.GenreShort == "KR")
                                ),

            new(title: "2019",
                                description: "K-pop 2019",
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-sk.jpg?raw=true",
                                searchFilterMode: SearchFilterMode.kpop2019,
                                songCount: SongPartRepository.SongParts.Count(s => s.Album?.ReleaseDate.Year == 2019 && s.Album?.GenreShort == "KR")
                                ),

            new(title: "2020",
                                description: "K-pop 2020",
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-sk.jpg?raw=true",
                                searchFilterMode: SearchFilterMode.kpop2020,
                                songCount: SongPartRepository.SongParts.Count(s => s.Album ?.ReleaseDate.Year == 2020 && s.Album ?.GenreShort == "KR")
                                ),

            new(title: "2021",
                                description: "K-pop 2021",
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-sk.jpg?raw=true",
                                searchFilterMode: SearchFilterMode.kpop2021,
                                songCount: SongPartRepository.SongParts.Count(s => s.Album ?.ReleaseDate.Year == 2021 && s.Album ?.GenreShort == "KR")
                                ),

            new(title: "2022",
                                description: "K-pop 2022",
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-sk.jpg?raw=true",
                                searchFilterMode: SearchFilterMode.kpop2022,
                                songCount: SongPartRepository.SongParts.Count(s => s.Album ?.ReleaseDate.Year == 2022 && s.Album ?.GenreShort == "KR")
                                ),

            new(title: "2023",
                                description: "K-pop 2023",
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-sk.jpg?raw=true",
                                searchFilterMode: SearchFilterMode.kpop2023,
                                songCount: SongPartRepository.SongParts.Count(s => s.Album ?.ReleaseDate.Year == 2023 && s.Album ?.GenreShort == "KR")
                                ),

            new(title: "2024",
                                description: "K-pop 2024",
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-sk.jpg?raw=true",
                                searchFilterMode: SearchFilterMode.kpop2024,
                                songCount: SongPartRepository.SongParts.Count(s => s.Album ?.ReleaseDate.Year == 2024 && s.Album ?.GenreShort == "KR")
                                )
        };

        var groupedTitles = from s in SongPartRepository.SongParts
                            group s.Title by s.Title into g
                            select new { Title = g.Key, Titles = g.ToList() };

        GenerationListView.IsVisible = false;
        CompanyListView.IsVisible = false;
        GenreListView.IsVisible = false;
        KpopYearsListView.IsVisible = false;

        GenerationRadioButton.IsChecked = false;
        CompanyRadioButton.IsChecked = false;
        GenreRadioButton.IsChecked = false;
        KpopYearsRadioButton.IsChecked = false;

        GenerationRadioButton.StateChanged += RadioButton_StateChanged;
        CompanyRadioButton.StateChanged += RadioButton_StateChanged;
        GenreRadioButton.StateChanged += RadioButton_StateChanged;
        KpopYearsRadioButton.StateChanged += RadioButton_StateChanged;
    }



    #region Filters
    private void SetFilter(object? sender, EventArgs e)
    {
        if (AllImageButton == sender) { MainViewModel.SearchFilterMode = SearchFilterMode.All; }
        else if (DanceImageButton == sender) { MainViewModel.SearchFilterMode = SearchFilterMode.DanceVideos; }
        else if (MaleImageButton == sender) { MainViewModel.SearchFilterMode = SearchFilterMode.Male; }
        else if (FemaleImageButton == sender) { MainViewModel.SearchFilterMode = SearchFilterMode.Female; }
        else if (SoloImageButton == sender) { MainViewModel.SearchFilterMode = SearchFilterMode.Solo; }
        else if (GroupImageButton == sender) { MainViewModel.SearchFilterMode = SearchFilterMode.Group; }

        //    case "trio": MainViewModel.SearchFilterMode = SearchFilterMode.Trio; break;
        //    case "quadruplet": MainViewModel.SearchFilterMode = SearchFilterMode.Quadruplet; break;
        //    case "quintet": MainViewModel.SearchFilterMode = SearchFilterMode.Quintet; break;
        //    case "sextet": MainViewModel.SearchFilterMode = SearchFilterMode.Sextet; break;
        //    case "septet": MainViewModel.SearchFilterMode = SearchFilterMode.Septet; break;
        //    case "octet": MainViewModel.SearchFilterMode = SearchFilterMode.Octet; break;
        //    case "nonet": MainViewModel.SearchFilterMode = SearchFilterMode.Nonet; break;

        //Toast.Make($"Filter mode: {MainViewModel.SearchFilterMode}", ToastDuration.Short, 14).Show();

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

        //Toast.Make($"Filter mode: {MainViewModel.SearchFilterMode}", ToastDuration.Short, 14).Show();
        FilterPressed?.Invoke(this, e);
    }

    private void RadioButton_StateChanged(object? sender, StateChangedEventArgs e)
    {
        GenerationListView.IsVisible = GenerationRadioButton.IsChecked;
        CompanyListView.IsVisible = CompanyRadioButton.IsChecked;
        GenreListView.IsVisible = GenreRadioButton.IsChecked;
        KpopYearsListView.IsVisible = KpopYearsRadioButton.IsChecked;
    }
    #endregion
}