using RpdPlayerApp.Architecture;
using RpdPlayerApp.Enums;
using RpdPlayerApp.Items;
using RpdPlayerApp.Managers;
using RpdPlayerApp.Models;
using RpdPlayerApp.Repositories;

namespace RpdPlayerApp.Views;

public partial class CategoriesView : ContentView
{
    internal event EventHandler? FilterPressed;

    internal event EventHandler? BackPressed;

    public CategoriesView()
    {
        InitializeComponent();
        Loaded += OnLoad;
    }

    private void OnLoad(object? sender, EventArgs e)
    {
        BackImageButton.Pressed += BackImageButton_Pressed;
        AllImageButton.Pressed += SetFilter;
        DanceImageButton.Pressed += SetFilter;
        MaleImageButton.Pressed += SetFilter;
        FemaleImageButton.Pressed += SetFilter;
        SoloImageButton.Pressed += SetFilter;
        GroupImageButton.Pressed += SetFilter;

        //    "[SHINHWA][][1998-03-24][BG][6][SM Entertainment].jpg",

        //    "%5BBIGBANG%5D%5B%5D%5B2006-08-19%5D%5BBG%5D%5B4%5D%5BYG%20Entertainment%5D.jpg",
        //    "%5BGirls%20Generation%20(SNSD)%5D%5BSNSD%2C%20GG%5D%5B2007-08-05%5D%5BGG%5D%5B8%5D%5BSM%20Entertainment%5D.webp",
        //    "%5B2PM%5D%5BHottest%20time%20of%20the%20day%5D%5B2008-09-04%5D%5BBG%5D%5B6%5D%5BJYP%20Entertainment%5D.jpg",
        //    "%5BSHINee%5D%5B%5D%5B2008-05-25%5D%5BBG%5D%5B5%5D%5BSM%20Entertainment%5D.webp",
        //    "%5BSuper%20Junior%5D%5BSUJU%5D%5B2005-11-06%5D%5BBG%5D%5B10%5D%5BSM%20Entertainment%5D.webp",
        //    "%5B2NE1%5D%5B21%5D%5B2009-05-06%5D%5BGG%5D%5B4%5D%5BYG%20Entertainment%5D.jpg"

        //    "[BTS][Bangtan Sonyeondan][2013-06-13][BG][7][Big Hit Entertainment].jpg",
        //    "[EXO][][2012-04-08][BG][9][SM Entertainment].png",
        //    "[Twice][TWICE][2015-10-20][GG][9][JYP Entertainment].webp",
        //    "[Blackpink][BLACKPINK][2016-08-08][GG][4][YG Entertainment].jpg",
        //    "[GOT7][][2014-01-16][BG][7][JYP Entertainment].webp",
        //    "[Red Velvet][RV][2014-08-01][GG][5][SM Entertainment].jpg"

        //    "%5BStray%20Kids%5D%5BSKZ%5D%5B2018-03-25%5D%5BBG%5D%5B8%5D%5BJYP%20Entertainment%5D.jpg",
        //    "%5BATEEZ%5D%5BKQ%20Fellaz%5D%5B2018-10-24%5D%5BBG%5D%5B8%5D%5BKQ%20Entertainment%5D.jpeg",
        //    "%5BITZY%5D%5B%5D%5B2019-02-12%5D%5BGG%5D%5B5%5D%5BJYP%20Entertainment%5D.webp",
        //    "%5BTXT%5D%5BTomorrow%20X%20Together%5D%5B2019-03-04%5D%5BBG%5D%5B5%5D%5BBig%20Hit%20Entertainment%5D.jpg",
        //    "%5Baespa%5D%5BAvatar%20x%20experience%5D%5B2020-11-17%5D%5BGG%5D%5B4%5D%5BSM%20Entertainment%5D.jpg"

        //    "%5BRIIZE%5D%5B%5D%5B2023-09-04%5D%5BBG%5D%5B7%5D%5BSM%20Entertainment%5D.jpg",
        //    "%5BZEROBASEONE%5D%5BZB1%5D%5B2023-07-10%5D%5BBG%5D%5B9%5D%5BWake%20One%5D.jpg",
        //    "%5BBABYMONSTER%5D%5BBM%5D%5B2024-04-01%5D%5BGG%5D%5B7%5D%5BYG%20Entertainment%5D.jpg",
        //    "%5BKISS%20OF%20LIFE%5D%5BKIOF%5D%5B2023-07-05%5D%5BGG%5D%5B4%5D%5BS2%20Entertainment%5D.jpg"

        var topFirstGens = ArtistRepository.GetTopArtistsForGen(Architecture.GenType.First);
        var topSecondGens = ArtistRepository.GetTopArtistsForGen(Architecture.GenType.Second);
        var topThirdGens = ArtistRepository.GetTopArtistsForGen(Architecture.GenType.Third);
        var topFourthGens = ArtistRepository.GetTopArtistsForGen(Architecture.GenType.Fourth);
        var topFifthGens = ArtistRepository.GetTopArtistsForGen(Architecture.GenType.Fifth);

        var firstGenDescription = MakeArtistsDescription(topFirstGens);
        var secondGenDescription = MakeArtistsDescription(topSecondGens);
        var thirdGenDescription = MakeArtistsDescription(topThirdGens);
        var fourthGenDescription = MakeArtistsDescription(topFourthGens);
        var fifthGenDescription = MakeArtistsDescription(topFifthGens);

        var firstGenArtist = ArtistRepository.GetRandomArtist(topFirstGens);
        var secondGenArtist = ArtistRepository.GetRandomArtist(topSecondGens);
        var thirdGenArtist = ArtistRepository.GetRandomArtist(topThirdGens);
        var fourthGenArtist = ArtistRepository.GetRandomArtist(topFourthGens);
        var fifthGenArtist = ArtistRepository.GetRandomArtist(topFifthGens);

        GenerationListView.ItemsSource = new List<HomeListViewItem>
        {
            new("1st Generation", firstGenArtist.ImageUrl, SearchFilterModeValue.Firstgen, $"{firstGenDescription}",
                songCount: SongPartRepository.GetSongPartsByGeneration(Constants.FIRST_GENERATION).Count,
                artistCount: SongPartRepository.GetSongPartsByGeneration(Constants.FIRST_GENERATION).DistinctBy(s => new { s.ArtistName }).Count(),
                artistName: firstGenArtist.Name),

            new("2nd Generation", secondGenArtist.ImageUrl, SearchFilterModeValue.Secondgen, $"{secondGenDescription}",
                songCount: SongPartRepository.GetSongPartsByGeneration(Constants.SECOND_GENERATION).Count,
                artistCount: SongPartRepository.GetSongPartsByGeneration(Constants.SECOND_GENERATION).DistinctBy(s => new { s.ArtistName }).Count(),
                artistName: secondGenArtist.Name),

            new("3rd Generation", thirdGenArtist.ImageUrl, SearchFilterModeValue.Thirdgen, $"{thirdGenDescription}",
                songCount: SongPartRepository.GetSongPartsByGeneration(Constants.THIRD_GENERATION).Count,
                artistCount: SongPartRepository.GetSongPartsByGeneration(Constants.THIRD_GENERATION).DistinctBy(s => new { s.ArtistName }).Count(),
                artistName: thirdGenArtist.Name),

            new("4th Generation", fourthGenArtist.ImageUrl, SearchFilterModeValue.Fourthgen, $"{fourthGenDescription}",
                songCount: SongPartRepository.GetSongPartsByGeneration(Constants.FOURTH_GENERATION).Count,
                artistCount: SongPartRepository.GetSongPartsByGeneration(Constants.FOURTH_GENERATION).DistinctBy(s => new { s.ArtistName }).Count(),
                artistName: fourthGenArtist.Name),

            new("5th Generation", fifthGenArtist.ImageUrl, SearchFilterModeValue.Fifthgen, $"{fifthGenDescription}",
                songCount: SongPartRepository.GetSongPartsByGeneration(Constants.FIFTH_GENERATION).Count,
                artistCount: SongPartRepository.GetSongPartsByGeneration(Constants.FIFTH_GENERATION).DistinctBy(s => new { s.ArtistName }).Count(), artistName: fifthGenArtist.Name)
        };

        var topSmArtists = ArtistRepository.GetTopArtistsForCompanies(Constants.SMCompanies);
        var topHybeArtists = ArtistRepository.GetTopArtistsForCompanies(Constants.HybeCompanies);
        var topJypArtists = ArtistRepository.GetTopArtistsForCompanies(["JYP Entertainment"]);
        var topYgArtists = ArtistRepository.GetTopArtistsForCompanies(Constants.YGCompanies);
        var topKakaoArtists = ArtistRepository.GetTopArtistsForCompanies(Constants.KakaoCompanies);
        var topCjenmArtists = ArtistRepository.GetTopArtistsForCompanies(Constants.CjenmCompanies);
        var topRbwArtists = ArtistRepository.GetTopArtistsForCompanies(Constants.RbwCompanies);
        var topStarshipArtists = ArtistRepository.GetTopArtistsForCompanies(["Starship Entertainment"]);
        var topCubeArtists = ArtistRepository.GetTopArtistsForCompanies(["Cube Entertainment"]);
        var topIstArtists = ArtistRepository.GetTopArtistsForCompanies(["IST Entertainment"]);
        var topPledisArtists = ArtistRepository.GetTopArtistsForCompanies(["Pledis Entertainment"]);
        var topFncArtists = ArtistRepository.GetTopArtistsForCompanies(["FNC Entertainment"]);
        var topWoollimArtists = ArtistRepository.GetTopArtistsForCompanies(["Woollim Entertainment"]);

        var smDescription = MakeArtistsDescription(topSmArtists);
        var hybeDescription = MakeArtistsDescription(topHybeArtists);
        var jypDescription = MakeArtistsDescription(topJypArtists);
        var ygDescription = MakeArtistsDescription(topYgArtists);
        var kakaoDescription = MakeArtistsDescription(topKakaoArtists);
        var cjenmDescription = MakeArtistsDescription(topCjenmArtists);
        var rbwArtists = MakeArtistsDescription(topRbwArtists);
        var starshipArtists = MakeArtistsDescription(topStarshipArtists);
        var cubeArtists = MakeArtistsDescription(topCubeArtists);
        var istArtists = MakeArtistsDescription(topIstArtists);
        var pledisArtists = MakeArtistsDescription(topPledisArtists);
        var fncArtists = MakeArtistsDescription(topFncArtists);
        var woollimArtists = MakeArtistsDescription(topWoollimArtists);

        CompanyListView.ItemsSource = new List<HomeListViewItem>() {
            new(title: "SM Entertainment", imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-sm.png?raw=true",
                topArtists: $"{smDescription}",
                childCompanies: "Label V, Mystic Story",
                searchFilterMode: SearchFilterModeValue.SM,
                songCount: SongPartRepository.SongParts.Count(s => Constants.SMCompanies.Contains(s.Artist.Company)),
                artistCount: SongPartRepository.SongParts.Where(s => Constants.SMCompanies.Contains(s.Artist.Company)).DistinctBy(s => new { s.ArtistName }).Count()),

            new(title: "HYBE Labels",
                topArtists: $"{hybeDescription}",
                oldNames: "Formerly: Big Hit Entertainment.",
                childCompanies: "Owns: Source Music and Pledis Entertainment.",
                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-hybe-labels.webp?raw=true",
                searchFilterMode: SearchFilterModeValue.Hybe,
                songCount: SongPartRepository.SongParts.Count(s => Constants.HybeCompanies.Contains(s.Artist.Company)),
                artistCount: SongPartRepository.SongParts.Where(s => Constants.HybeCompanies.Contains(s.Artist.Company)).DistinctBy(s => new { s.ArtistName }).Count()),

            new(title: "JYP Entertainment",
                topArtists: $"{jypDescription}",
                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-jyp.jpg?raw=true",
                searchFilterMode: SearchFilterModeValue.JYP,
                songCount: SongPartRepository.SongParts.Count(s => s.Artist?.Company == "JYP Entertainment"),
                artistCount: SongPartRepository.SongParts.Where(s => s.Artist?.Company == "JYP Entertainment").DistinctBy(s => new { s.ArtistName }).Count()),

            new(title: "YG Entertainment",
                topArtists : $"{ygDescription}",
                childCompanies : "Owns: The Black Label",
                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-yg.jpg?raw=true",
                searchFilterMode: SearchFilterModeValue.YG,
                songCount: SongPartRepository.SongParts.Count(s => Constants.YGCompanies.Contains(s.Artist.Company)),
                artistCount: SongPartRepository.SongParts.Where(s => Constants.YGCompanies.Contains(s.Artist.Company)).DistinctBy(s => new { s.ArtistName }).Count()),

            new(title: "Kakao Entertainment",
                topArtists: $"{kakaoDescription}",
                childCompanies : "Owns: IST Entertainment, Starship Entertainment, EDAM Entertainment, Bluedot Entertainment, High Up Entertainment, Antenna and FLEX M.",
                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-kakao.webp?raw=true",
                searchFilterMode: SearchFilterModeValue.Kakao_Entertainment,
                songCount: SongPartRepository.SongParts.Count(s => Constants.KakaoCompanies.Contains(s.Artist.Company)),
                artistCount: SongPartRepository.SongParts.Where(s => Constants.KakaoCompanies.Contains(s.Artist.Company)).DistinctBy(s => new { s.ArtistName }).Count()),

            new(title: "Starship Entertainment",
                topArtists: $"{starshipArtists}",
                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-starship.webp?raw=true",
                searchFilterMode: SearchFilterModeValue.Starship,
                songCount: SongPartRepository.SongParts.Count(s => s.Artist?.Company == "Starship Entertainment"),
                artistCount: SongPartRepository.SongParts.Where(s => s.Artist?.Company == "Starship Entertainment").DistinctBy(s => new { s.ArtistName }).Count()),

            new(title: "RBW Entertainment",
                topArtists: $"{rbwArtists}",
                childCompanies: $"WM entertainment and DSP Media (Daesung enterprise).",
                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-rbw.png?raw=true",
                searchFilterMode: SearchFilterModeValue.RBW,
                songCount : SongPartRepository.SongParts.Count(s => Constants.RbwCompanies.Contains(s.Artist.Company)),
                 artistCount: SongPartRepository.SongParts.Where(s => Constants.RbwCompanies.Contains(s.Artist.Company)).DistinctBy(s => new { s.ArtistName }).Count()),

            new(title: "Cube Entertainment",
                topArtists: $"{cubeArtists}",
                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-cube.webp?raw=true",
                searchFilterMode: SearchFilterModeValue.Cube,
                songCount: SongPartRepository.SongParts.Count(s => s.Artist?.Company == "Cube Entertainment"),
                artistCount: SongPartRepository.SongParts.Where(s => s.Artist?.Company == "Cube Entertainment").DistinctBy(s => new { s.ArtistName }).Count()),

            new(title: "IST Entertainment",
                topArtists: $"{istArtists}",
                oldNames: $"Formerly: A Cube, Play A, Play M.",
                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-ist.webp?raw=true",
                searchFilterMode: SearchFilterModeValue.IST,
                songCount : SongPartRepository.SongParts.Count(s => s.Artist?.Company == "IST Entertainment"),
                artistCount: SongPartRepository.SongParts.Where(s => s.Artist?.Company == "IST Entertainment").DistinctBy(s => new { s.ArtistName }).Count()),

            new(title: "Pledis Entertainment",
                topArtists: $"{pledisArtists}",
                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-pledis.webp?raw=true",
                searchFilterMode: SearchFilterModeValue.Pledis,
                songCount: SongPartRepository.SongParts.Count(s => s.Artist?.Company == "Pledis Entertainment"),
                artistCount: SongPartRepository.SongParts.Where(s => s.Artist?.Company == "Pledis Entertainment").DistinctBy(s => new { s.ArtistName }).Count()),

            new(title: "CJ ENM Music",
                topArtists: $"{cjenmDescription}",
                childCompanies: "Owns: AOMG, B2M Entertainment, Jellyfish Entertainment, Wake One, Stone Music Entertainment, Swing Entertainment",
                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-cjenm.webp?raw=true",
                searchFilterMode: SearchFilterModeValue.CJ_ENM_Music,
                songCount: SongPartRepository.SongParts.Count(s => Constants.CjenmCompanies.Contains(s.Artist.Company)),
                artistCount: SongPartRepository.SongParts.Where(s => Constants.CjenmCompanies.Contains(s.Artist.Company)).DistinctBy(s => new { s.ArtistName }).Count()),

            new(title: "FNC Entertainment",
                topArtists: $"{fncArtists}",
                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-fnc.png?raw=true",
                searchFilterMode: SearchFilterModeValue.FNC,
                songCount : SongPartRepository.SongParts.Count(s => s.Artist?.Company == "FNC Entertainment"),
                artistCount: SongPartRepository.SongParts.Where(s =>  s.Artist?.Company == "FNC Entertainment").DistinctBy(s => new { s.ArtistName }).Count()),

            new(title: "Woollim Entertainment",
                topArtists: $"{woollimArtists}",
                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-woollim.webp?raw=true",
                searchFilterMode: SearchFilterModeValue.Woollim,
                songCount : SongPartRepository.SongParts.Count(s => s.Artist?.Company == "Woollim Entertainment"),
                artistCount: SongPartRepository.SongParts.Where(s =>  s.Artist?.Company == "Woollim Entertainment").DistinctBy(s => new { s.ArtistName }).Count()),
        };

        // TODO: GenreShort Jpop, Pop, C-pop, T-pop instead of JP, EN, CH etc
        GenreListView.ItemsSource = new List<HomeListViewItem>() {
            new(title: "K-pop",
                description: "Korean pop music.",
                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-sk.jpg?raw=true",
                searchFilterMode: SearchFilterModeValue.Kpop,
                songCount: SongPartRepository.SongParts.Count(s => s.Album?.GenreShort == Constants.GenreKpop),
                artistCount: SongPartRepository.SongParts.Where(s => s.Album?.GenreShort == Constants.GenreKpop).DistinctBy(s => new { s.ArtistName }).Count()),


            new(title: "J-pop",
                description: "Japanese pop music.",
                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-jp.webp?raw=true",
                searchFilterMode: SearchFilterModeValue.Jpop,
                songCount: SongPartRepository.SongParts.Count(s => s.Album?.GenreShort == "JP"),
                artistCount: SongPartRepository.SongParts.Where(s => s.Album?.GenreShort == "JP").DistinctBy(s => new { s.ArtistName }).Count()),

            new(title: "English pop",
                description: "English pop music.",
                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-us.webp?raw=true",
                searchFilterMode: SearchFilterModeValue.EN,
                songCount: SongPartRepository.SongParts.Count(s => s.Album?.GenreShort == "EN"),
                artistCount: SongPartRepository.SongParts.Where(s => s.Album?.GenreShort == "EN").DistinctBy(s => new { s.ArtistName }).Count()),

            new(title: "C-pop",
                description: "Chinese pop music.",
                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-ch.png?raw=true",
                searchFilterMode: SearchFilterModeValue.Cpop,
                songCount: SongPartRepository.SongParts.Count(s => s.Album?.GenreShort == "CH"),
                artistCount: SongPartRepository.SongParts.Where(s => s.Album?.GenreShort == "CH").DistinctBy(s => new { s.ArtistName }).Count()),

            new(title: "T-pop",
                description: "Thai pop music.",
                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-th.webp?raw=true",
                searchFilterMode: SearchFilterModeValue.Tpop,
                songCount: SongPartRepository.SongParts.Count(s => s.Album?.GenreShort == "TH"),
                artistCount: SongPartRepository.SongParts.Where(s => s.Album?.GenreShort == "TH").DistinctBy(s => new { s.ArtistName }).Count())
        };

        KpopYearsListView.ItemsSource = new List<HomeListViewItem>() {
            new(title: "< 2012",
                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-sk.jpg?raw=true",
                searchFilterMode: SearchFilterModeValue.KpopSoonerThan2012,
                songCount: SongPartRepository.SongParts.Count(s => s.Album?.ReleaseDate.Year < 2012 && s.Album?.GenreShort == Constants.GenreKpop)),
            HomeListViewItem.Create(2012, SearchFilterModeValue.kpop2012),
            HomeListViewItem.Create(2013, SearchFilterModeValue.kpop2013),
            HomeListViewItem.Create(2014, SearchFilterModeValue.kpop2014),
            HomeListViewItem.Create(2015, SearchFilterModeValue.kpop2015),
            HomeListViewItem.Create(2016, SearchFilterModeValue.kpop2016),
            HomeListViewItem.Create(2017, SearchFilterModeValue.kpop2017),
            HomeListViewItem.Create(2018, SearchFilterModeValue.kpop2018),
            HomeListViewItem.Create(2019, SearchFilterModeValue.kpop2019),
            HomeListViewItem.Create(2020, SearchFilterModeValue.kpop2020),
            HomeListViewItem.Create(2021, SearchFilterModeValue.kpop2021),
            HomeListViewItem.Create(2022, SearchFilterModeValue.kpop2022),
            HomeListViewItem.Create(2023, SearchFilterModeValue.kpop2023),
            HomeListViewItem.Create(2024, SearchFilterModeValue.kpop2024),
            HomeListViewItem.Create(2025, SearchFilterModeValue.kpop2025)
        };

        GenerationListView.IsVisible = true;
        CompanyListView.IsVisible = false;
        GenreListView.IsVisible = false;
        KpopYearsListView.IsVisible = false;

        OtherCategoriesSegmentedControl.ItemsSource = new string[] { "Gens", "Companies", "Genres", "K-pop years" };
        OtherCategoriesSegmentedControl.SelectionChanged += OtherCategoriesSegmentedControl_SelectionChanged;
        OtherCategoriesSegmentedControl.SelectedIndex = 0;
    }
    private string MakeArtistsDescription(List<Artist> artists) => string.Join(", ", artists.Select(a => a.Name)) + " ...";

    private void BackImageButton_Pressed(object? sender, EventArgs e) => BackPressed?.Invoke(sender, e);

    private void OtherCategoriesSegmentedControl_SelectionChanged(object? sender, Syncfusion.Maui.Buttons.SelectionChangedEventArgs e)
    {
        GenerationListView.IsVisible = (OtherCategoriesSegmentedControl.SelectedIndex == 0);
        CompanyListView.IsVisible = (OtherCategoriesSegmentedControl.SelectedIndex == 1);
        GenreListView.IsVisible = (OtherCategoriesSegmentedControl.SelectedIndex == 2);
        KpopYearsListView.IsVisible = (OtherCategoriesSegmentedControl.SelectedIndex == 3);
    }

    #region Filters

    private void SetFilter(object? sender, EventArgs e)
    {
        if (AllImageButton == sender) { AppState.SearchFilterMode = SearchFilterModeValue.All; }
        else if (DanceImageButton == sender) { AppState.SearchFilterMode = SearchFilterModeValue.DanceVideos; }
        else if (MaleImageButton == sender) { AppState.SearchFilterMode = SearchFilterModeValue.Male; }
        else if (FemaleImageButton == sender) { AppState.SearchFilterMode = SearchFilterModeValue.Female; }
        else if (SoloImageButton == sender) { AppState.SearchFilterMode = SearchFilterModeValue.Solo; }
        else if (GroupImageButton == sender) { AppState.SearchFilterMode = SearchFilterModeValue.Group; }

        // case "trio": AppState.SearchFilterMode = SearchFilterMode.Trio; break; case
        // "quadruplet": AppState.SearchFilterMode = SearchFilterMode.Quadruplet; break; case
        // "quintet": AppState.SearchFilterMode = SearchFilterMode.Quintet; break; case
        // "sextet": AppState.SearchFilterMode = SearchFilterMode.Sextet; break; case "septet":
        // AppState.SearchFilterMode = SearchFilterMode.Septet; break; case "octet":
        // AppState.SearchFilterMode = SearchFilterMode.Octet; break; case "nonet":
        // AppState.SearchFilterMode = SearchFilterMode.Nonet; break;

        //Toast.Make($"Filter mode: {AppState.SearchFilterMode}", ToastDuration.Short, 14).Show();

        FilterPressed?.Invoke(this, e);
    }

    private void CategoryListViewItemTapped(object sender, Syncfusion.Maui.ListView.ItemTappedEventArgs e)
    {
        if (e.ItemType != Syncfusion.Maui.ListView.ItemType.Record)
            return;

        HomeListViewItem item = (HomeListViewItem)e.DataItem;

        if (item is not null)
        {
            AppState.SearchFilterMode = item.SearchFilterMode;
        }

        //Toast.Make($"Filter mode: {AppState.SearchFilterMode}", ToastDuration.Short, 14).Show();
        FilterPressed?.Invoke(this, e);
    }

    #endregion Filters
}