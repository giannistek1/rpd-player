using RpdPlayerApp.Enums;
using RpdPlayerApp.Items;
using RpdPlayerApp.Models;
using RpdPlayerApp.Repositories;
using RpdPlayerApp.ViewModels;
using Syncfusion.Maui.Buttons;
using System.Text;

namespace RpdPlayerApp.Views;

public partial class CategoriesView : ContentView
{
    internal event EventHandler? FilterPressed;

    internal event EventHandler? BackPressed;

    private const string ARTISTS_URL = "https://github.com/giannistek1/rpd-artists/blob/main/";

    public CategoriesView()
    {
        InitializeComponent();

        this.Loaded += OnLoad;
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

        // TODO: firstgens
        //string[] firstGens = [
        //    "[SHINHWA][][1998-03-24][BG][6][SM Entertainment].jpg",
        //];

        //string[] secondGens = [
        //    "%5BBIGBANG%5D%5B%5D%5B2006-08-19%5D%5BBG%5D%5B4%5D%5BYG%20Entertainment%5D.jpg",
        //    "%5BGirls%20Generation%20(SNSD)%5D%5BSNSD%2C%20GG%5D%5B2007-08-05%5D%5BGG%5D%5B8%5D%5BSM%20Entertainment%5D.webp",
        //    "%5B2PM%5D%5BHottest%20time%20of%20the%20day%5D%5B2008-09-04%5D%5BBG%5D%5B6%5D%5BJYP%20Entertainment%5D.jpg",
        //    "%5BSHINee%5D%5B%5D%5B2008-05-25%5D%5BBG%5D%5B5%5D%5BSM%20Entertainment%5D.webp",
        //    "%5BSuper%20Junior%5D%5BSUJU%5D%5B2005-11-06%5D%5BBG%5D%5B10%5D%5BSM%20Entertainment%5D.webp",
        //    "%5B2NE1%5D%5B21%5D%5B2009-05-06%5D%5BGG%5D%5B4%5D%5BYG%20Entertainment%5D.jpg"
        //];

        //string[] thirdGens = [
        //    "[BTS][Bangtan Sonyeondan][2013-06-13][BG][7][Big Hit Entertainment].jpg",
        //    "[EXO][][2012-04-08][BG][9][SM Entertainment].png",
        //    "[Twice][TWICE][2015-10-20][GG][9][JYP Entertainment].webp",
        //    "[Blackpink][BLACKPINK][2016-08-08][GG][4][YG Entertainment].jpg",
        //    "[GOT7][][2014-01-16][BG][7][JYP Entertainment].webp",
        //    "[Red Velvet][RV][2014-08-01][GG][5][SM Entertainment].jpg"
        //];

        //string[] fourthGens = [
        //    "%5BStray%20Kids%5D%5BSKZ%5D%5B2018-03-25%5D%5BBG%5D%5B8%5D%5BJYP%20Entertainment%5D.jpg",
        //    "%5BATEEZ%5D%5BKQ%20Fellaz%5D%5B2018-10-24%5D%5BBG%5D%5B8%5D%5BKQ%20Entertainment%5D.jpeg",
        //    "%5BITZY%5D%5B%5D%5B2019-02-12%5D%5BGG%5D%5B5%5D%5BJYP%20Entertainment%5D.webp",
        //    "%5BTXT%5D%5BTomorrow%20X%20Together%5D%5B2019-03-04%5D%5BBG%5D%5B5%5D%5BBig%20Hit%20Entertainment%5D.jpg",
        //    "%5Baespa%5D%5BAvatar%20x%20experience%5D%5B2020-11-17%5D%5BGG%5D%5B4%5D%5BSM%20Entertainment%5D.jpg"
        //];

        //string[] fifthGens = [
        //    "%5BRIIZE%5D%5B%5D%5B2023-09-04%5D%5BBG%5D%5B7%5D%5BSM%20Entertainment%5D.jpg",
        //    "%5BZEROBASEONE%5D%5BZB1%5D%5B2023-07-10%5D%5BBG%5D%5B9%5D%5BWake%20One%5D.jpg",
        //    "%5BBABYMONSTER%5D%5BBM%5D%5B2024-04-01%5D%5BGG%5D%5B7%5D%5BYG%20Entertainment%5D.jpg",
        //    "%5BKISS%20OF%20LIFE%5D%5BKIOF%5D%5B2023-07-05%5D%5BGG%5D%5B4%5D%5BS2%20Entertainment%5D.jpg"
        //];

        var topFirstGens = ArtistRepository.GetTopArtistsByGen(Architecture.Gen.First);
        var topSecondGens = ArtistRepository.GetTopArtistsByGen(Architecture.Gen.Second);
        var topThirdGens = ArtistRepository.GetTopArtistsByGen(Architecture.Gen.Third);
        var topFourthGens = ArtistRepository.GetTopArtistsByGen(Architecture.Gen.Fourth);
        var topFifthGens = ArtistRepository.GetTopArtistsByGen(Architecture.Gen.Fifth);

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
            new("1st Generation", $"First generation: {firstGenDescription}", firstGenArtist.ImageUrl, SearchFilterMode.Firstgen,
                SongPartRepository.SongParts.Count(s => s.Artist?.Generation == MainViewModel.FIRST_GENERATION && s.Album?.GenreShort == MainViewModel.GenreKpop)),

            new("2nd Generation", $"Second generation: {secondGenDescription}", secondGenArtist.ImageUrl, SearchFilterMode.Secondgen,
                SongPartRepository.SongParts.Count(s => s.Artist?.Generation == MainViewModel.SECOND_GENERATION && s.Album?.GenreShort == MainViewModel.GenreKpop)),

            new("3rd Generation", $"Third generation: {thirdGenDescription}", thirdGenArtist.ImageUrl, SearchFilterMode.Thirdgen,
                SongPartRepository.SongParts.Count(s => s.Artist?.Generation == MainViewModel.THIRD_GENERATION && s.Album?.GenreShort == MainViewModel.GenreKpop)),

            new("4th Generation", $"Fourth generation: {fourthGenDescription}", fourthGenArtist.ImageUrl, SearchFilterMode.Fourthgen,
                SongPartRepository.SongParts.Count(s => s.Artist?.Generation == MainViewModel.FOURTH_GENERATION && s.Album?.GenreShort == MainViewModel.GenreKpop)),

            new("5th Generation", $"Fifth generation: {fifthGenDescription}", fifthGenArtist.ImageUrl, SearchFilterMode.Fifthgen,
                SongPartRepository.SongParts.Count(s => s.Artist?.Generation == MainViewModel.FIFTH_GENERATION && s.Album?.GenreShort == MainViewModel.GenreKpop))
        };

        // TODO: Starship, rbw, pledis, woollim, etc
        var topSmArtists = ArtistRepository.GetTopArtistsByCompanies(MainViewModel.SMCompanies);
        var topHybeArtists = ArtistRepository.GetTopArtistsByCompanies(MainViewModel.HybeCompanies);
        var topJypArtists = ArtistRepository.GetTopArtistsByCompanies(new List<string> { "JYP Entertainment" });
        var topYgArtists = ArtistRepository.GetTopArtistsByCompanies(MainViewModel.YGCompanies);
        var topKakaoArtists = ArtistRepository.GetTopArtistsByCompanies(MainViewModel.KakaoCompanies);
        var topCjenmArtists = ArtistRepository.GetTopArtistsByCompanies(MainViewModel.CjenmCompanies);

        var smDescription = MakeArtistsDescription(topSmArtists);
        var hybeDescription = MakeArtistsDescription(topHybeArtists);
        var jypDescription = MakeArtistsDescription(topJypArtists);
        var ygDescription = MakeArtistsDescription(topYgArtists);
        var kakaoDescription = MakeArtistsDescription(topKakaoArtists);
        var cjenmDescription = MakeArtistsDescription(topCjenmArtists);

        CompanyListView.ItemsSource = new List<HomeListViewItem>() {
            new(title: "SM Entertainment",
                                description: $"SM Entertainment: {smDescription}",
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-sm.png?raw=true",
                                searchFilterMode: SearchFilterMode.SM,
                                songCount: SongPartRepository.SongParts.Count(s => MainViewModel.SMCompanies.Contains(s.Artist.Company))
                                ),

            new(title: "HYBE Labels",
                                 description: $"HYBE Labels, formerly known as Big Hit Entertainment with child companies: Source Music and Pledis Entertainment. {hybeDescription}",
                                 imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-hybe-labels.webp?raw=true",
                                 searchFilterMode: SearchFilterMode.Hybe,
                                 songCount: SongPartRepository.SongParts.Count(s => MainViewModel.HybeCompanies.Contains(s.Artist.Company))
                                 ),

            new(title: "JYP Entertainment",
                                description: $"JYP Entertainment: {jypDescription}",
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-jyp.jpg?raw=true",
                                searchFilterMode: SearchFilterMode.JYP,
                                songCount: SongPartRepository.SongParts.Count(s => s.Artist?.Company == "JYP Entertainment")
                                ),

            new(title: "YG Entertainment",
                                description: $"YG Entertainment: {ygDescription}",
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-yg.jpg?raw=true",
                                searchFilterMode: SearchFilterMode.YG,
                                songCount: SongPartRepository.SongParts.Count(s => MainViewModel.YGCompanies.Contains(s.Artist.Company))
                                ),

            new(title: "Kakao Entertainment",
                                 description: $"Kakao Entertainment. Has many child companies: IST Entertainment, Starship Entertainment, EDAM Entertainment, Bluedot Entertainment, High Up Entertainment, Antenna and FLEX M. {kakaoDescription}",
                                 imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-kakao.webp?raw=true",
                                 searchFilterMode: SearchFilterMode.Kakao_Entertainment,
                                 songCount: SongPartRepository.SongParts.Count(s => MainViewModel.KakaoCompanies.Contains(s.Artist.Company))
                                 ),

            new(title: "Starship Entertainment",
                                description: $"Starship Entertainment.",
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-starship.webp?raw=true",
                                searchFilterMode: SearchFilterMode.Starship,
                                songCount: SongPartRepository.SongParts.Count(s => s.Artist ?.Company == "Starship Entertainment")
                                ),

            new(title: "RBW Entertainment",
                                description: $"Rainbow Bridge World Entertainment, includes WM entertainment and DSP Media (Daesung enterprise).",
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-rbw.png?raw=true",
                                searchFilterMode: SearchFilterMode.RBW,
                                songCount : SongPartRepository.SongParts.Count(s => s.Artist ?.Company == "RBW Entertainment" ||
                                                                                    s.Artist ?.Company == "WM Entertainment" ||
                                                                                    s.Artist ?.Company == "DSP Media")
                                ),

            new(title: "Cube Entertainment",
                                description: $"Cube Entertainment.",
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-cube.webp?raw=true",
                                searchFilterMode: SearchFilterMode.Cube,
                                songCount: SongPartRepository.SongParts.Count(s => s.Artist ?.Company == "Cube Entertainment")
                                ),

            new(title: "IST Entertainment",
                                description: $"IST Entertainment. Formerly known as A Cube, Play A, Play M.",
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-ist.webp?raw=true",
                                searchFilterMode: SearchFilterMode.IST,
                                songCount : SongPartRepository.SongParts.Count(s => s.Artist ?.Company == "IST Entertainment")
                                ),

            new(title: "Pledis Entertainment",
                                description: $"Pledis Entertainment.",
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-pledis.webp?raw=true",
                                searchFilterMode: SearchFilterMode.Pledis,
                                songCount: SongPartRepository.SongParts.Count(s => s.Artist ?.Company == "Pledis Entertainment")
                                ),

            new(title: "CJ ENM Music",
                                 description: $"CJ ENM Music: {cjenmDescription}",
                                 imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-cjenm.webp?raw=true",
                                 searchFilterMode: SearchFilterMode.CJ_ENM_Music,
                                 songCount: SongPartRepository.SongParts.Count(s => MainViewModel.CjenmCompanies.Contains(s.Artist.Company))
                                 ),

            new(title: "FNC Entertainment",
                                description: $"FNC Entertainment.",
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-fnc.png?raw=true",
                                searchFilterMode: SearchFilterMode.FNC,
                                songCount : SongPartRepository.SongParts.Count(s => s.Artist ?.Company == "FNC Entertainment")
                                ),

            new(title: "Woollim Entertainment",
                                description: $"Woollim Entertainment.",
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-woollim.webp?raw=true",
                                searchFilterMode: SearchFilterMode.Woollim,
                                songCount : SongPartRepository.SongParts.Count(s => s.Artist ?.Company == "Woollim Entertainment")
                                ),
        };

        // TODO: Top kpop, jpop, cpop etc
        GenreListView.ItemsSource = new List<HomeListViewItem>() {
            new(title: "K-pop",
                                description: "Korean pop music.",
                                imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-sk.jpg?raw=true",
                                searchFilterMode: SearchFilterMode.KR,
                                songCount: SongPartRepository.SongParts.Count(s => s.Album?.GenreShort == MainViewModel.GenreKpop)
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
            HomeListViewItem.Create(2012, SearchFilterMode.kpop2012),
            HomeListViewItem.Create(2013, SearchFilterMode.kpop2013),
            HomeListViewItem.Create(2014, SearchFilterMode.kpop2014),
            HomeListViewItem.Create(2015, SearchFilterMode.kpop2015),
            HomeListViewItem.Create(2016, SearchFilterMode.kpop2016),
            HomeListViewItem.Create(2017, SearchFilterMode.kpop2017),
            HomeListViewItem.Create(2018, SearchFilterMode.kpop2018),
            HomeListViewItem.Create(2019, SearchFilterMode.kpop2019),
            HomeListViewItem.Create(2020, SearchFilterMode.kpop2020),
            HomeListViewItem.Create(2021, SearchFilterMode.kpop2021),
            HomeListViewItem.Create(2022, SearchFilterMode.kpop2022),
            HomeListViewItem.Create(2023, SearchFilterMode.kpop2023),
            HomeListViewItem.Create(2024, SearchFilterMode.kpop2024),
            HomeListViewItem.Create(2025, SearchFilterMode.kpop2025)
        };

        GenerationListView.IsVisible = false;
        CompanyListView.IsVisible = false;
        GenreListView.IsVisible = false;
        KpopYearsListView.IsVisible = false;

        OtherCategoriesSegmentedControl.ItemsSource = new string[] { "Gens", "Companies", "Genres", "K-pop years" };
        OtherCategoriesSegmentedControl.SelectionChanged += OtherCategoriesSegmentedControl_SelectionChanged;
        OtherCategoriesSegmentedControl.SelectedIndex = 0;
    }
    private string MakeArtistsDescription(List<Artist> artists) => string.Join(", ", artists.Select(a => a.Name)) + " and more...";

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
        if (AllImageButton == sender) { MainViewModel.SearchFilterMode = SearchFilterMode.All; }
        else if (DanceImageButton == sender) { MainViewModel.SearchFilterMode = SearchFilterMode.DanceVideos; }
        else if (MaleImageButton == sender) { MainViewModel.SearchFilterMode = SearchFilterMode.Male; }
        else if (FemaleImageButton == sender) { MainViewModel.SearchFilterMode = SearchFilterMode.Female; }
        else if (SoloImageButton == sender) { MainViewModel.SearchFilterMode = SearchFilterMode.Solo; }
        else if (GroupImageButton == sender) { MainViewModel.SearchFilterMode = SearchFilterMode.Group; }

        // case "trio": MainViewModel.SearchFilterMode = SearchFilterMode.Trio; break; case
        // "quadruplet": MainViewModel.SearchFilterMode = SearchFilterMode.Quadruplet; break; case
        // "quintet": MainViewModel.SearchFilterMode = SearchFilterMode.Quintet; break; case
        // "sextet": MainViewModel.SearchFilterMode = SearchFilterMode.Sextet; break; case "septet":
        // MainViewModel.SearchFilterMode = SearchFilterMode.Septet; break; case "octet":
        // MainViewModel.SearchFilterMode = SearchFilterMode.Octet; break; case "nonet":
        // MainViewModel.SearchFilterMode = SearchFilterMode.Nonet; break;

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

    #endregion Filters
}