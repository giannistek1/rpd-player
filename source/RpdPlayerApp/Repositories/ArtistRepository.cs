using RpdPlayerApp.Architecture;
using RpdPlayerApp.Models;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;

namespace RpdPlayerApp.Repository;

internal static class ArtistRepository
{
    private const string ARTISTS_TXT_URL = "https://github.com/giannistek1/rpd-artists/blob/main/artists.txt?raw=true";

    public readonly static ObservableCollection<Artist> Artists = new ObservableCollection<Artist>();

    public static bool GetArtists() => InitArtists(GetStringFromURL());

    public static bool InitArtists(string artistsText)
    {
        // pattern = any number of arbitrary characters between square brackets.
        var pattern = @"\{(.*?)\}";
        var matches = Regex.Matches(artistsText, pattern);

        // artist, alt names, debut date, grouptype, members, company, imageurl
        // matches = 7 * 165 = 4000 
        // 0 1 2 3  4  5  6  Artist 1
        // 7 8 9 10 11 12 13  Artist 2


        for (int i = 0; i < matches.Count / 7; i++)
        {
            int n = 7 * i; // artist number

            Enum.TryParse(matches[n + 3].Groups[1].Value, out GroupType groupType);

            Artists.Add(new Artist(id: i, name: matches[n + 0].Groups[1].Value, 
                altName: matches[n + 1].Groups[1].Value, 
                debutDate: DateTime.ParseExact(matches[n + 2].Groups[1].Value, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                groupType,
                memberCount: Convert.ToInt16(matches[n + 4].Groups[1].Value),
                company: matches[n + 5].Groups[1].Value,
                imageURL: matches[n + 6].Groups[1].Value));
        }

        return Artists.Count > 0;
    }

    internal static Artist? MatchArtist(string artistName)
    {
        return Artists.FirstOrDefault(a => a.Name.ToLower().Equals(artistName.ToLower()));
    }

    private static string GetStringFromURL()
    {
        if (!HelperClass.HasInternetConnection())
            return string.Empty;

        string artistsAsText = string.Empty;

        using (HttpClient client = new HttpClient())
        {
            artistsAsText = client.GetStringAsync(ARTISTS_TXT_URL).Result;
        }
        return artistsAsText;
    }

    //        public readonly static List<string> Artists = new List<string>()
    //        {
    //"2AM"
    //, "2NE1"
    //, "2PM"
    //, "4Minute"
    //, "50/50"
    //, "AB6IX"
    //, "ADYA"
    //, "Aespa"
    //, "After School"
    //, "Ailee"
    //, "AIMERS"
    //, "AOA"
    //, "Apink"
    //, "ASTRO"
    //, "ATBO"
    //, "Ateez"
    //, "B.A.P"
    //, "B.O.Y"
    //, "B1A4"
    //, "Baby V.O.X"
    //, "Babymonster"
    //, "BAE173"
    //, "Baekhyun"
    //, "BEAST"
    //, "Big Bang"
    //, "Billlie"
    //, "Blackpink"
    //, "Blank2y"
    //, "Block B"
    //, "BoA"
    //, "Brown Eyed Girls"
    //, "BtoB"
    //, "BTS"
    //, "Chakra"
    //, "Chungha"
    //, "CIX"
    //, "CLC"
    //, "CLON"
    //, "CNBLUE"
    //, "Cool"
    //, "Cosmic Girls (WJSN)"
    //, "CRAVITY"
    //, "CSR"
    //, "Davichi"
    //, "DAY6"
    //, "DEAN"
    //, "Diva"
    //, "DKB"
    //, "Dreamcatcher"
    //, "DRIPPIN"
    //, "Drunken Tiger"
    //, "E'Last"
    //, "ENHYPEN"
    //, "EPEX"
    //, "Epik High"
    //, "Eric Nam"
    //, "EVERGLOW"
    //, "EXID"
    //, "EXO"
    //, "f(x)"
    //, "Fin.K.L"
    //, "fromis_9"
    //, "FTISLAND"
    //, "g.o.d"
    //, "G-Dragon"
    //, "GFriend"
    //, "Girls' Generation (SNSD)"
    //, "GOT7"
    //, "H.O.T"
    //, "Heize"
    //, "Highlight"
    //, "Hyolyn"
    //, "HyunA"
    //, "iKON"
    //, "ILLIT"
    //, "ILY:1"
    //, "Infinite"
    //, "ITZY"
    //, "IU"
    //, "IVE"
    //, "Jay Park"
    //, "Jessi"
    //, "Jinusean"
    //, "JUST B"
    //, "Kang Daniel"
    //, "KARA"
    //, "KARD"
    //, "KEP1ER"
    //, "Kingdom"
    //, "Kiss of Life"
    //, "Lapillus"
    //, "Lee Hi"
    //, "LOONA"
    //, "Lovelyz"
    //, "LUN8"
    //, "Mamamoo"
    //, "MBLAQ"
    //, "MCND"
    //, "Mino"
    //, "MIRAE"
    //, "Miss A"
    //, "Monsta X"
    //, "NATURE"
    //, "NCT"
    //, "NCT Dream"
    //, "NCT WISH"
    //, "NewJeans"
    //, "NINE.i"
    //, "NMIXX"
    //, "NRG"
    //, "NU'EST"
    //, "Oh My Girl"
    //, "ONEUS"
    //, "ONF"
    //, "P1Harmony"
    //, "Pentagon"
    //, "Purple Kiss"
    //, "Rain"
    //, "Red Velvet"
    //, "Riize"
    //, "Rocket Punch"
    //, "Roo'ra"
    //, "Rosé"
    //, "S.E.S"
    //, "Sam Kim"
    //, "Sechs Kies"
    //, "Secret Number"
    //, "SEVENTEEN"
    //, "SF9"
    //, "SHINee"
    //, "Shinhwa"
    //, "SISTAR"
    //, "Solid"
    //, "SS501"
    //, "STAYC"
    //, "Stray Kids"
    //, "Sunmi"
    //, "Super Junior"
    //, "Taemin"
    //, "Taeyeon"
    //, "T-ara"
    //, "TEMPEST"
    //, "THE BOYZ"
    //, "TNX"
    //, "TO1"
    //, "TOP"
    //, "TREASURE"
    //, "TRI.BE"
    //, "Turbo"
    //, "TVXQ"
    //, "Twice"
    //, "TXT (TOMORROW X TOGETHER)"
    //, "U-KISS"
    //, "VERIVERY"
    //, "VIXX"
    //, "WayV"
    //, "Weeekly"
    //, "WEi"
    //, "Weki Meki"
    //, "WINNER"
    //, "Wonder Girls"
    //, "Wonho"
    //, "Woodz"
    //, "X1 (disbanded)"
    //, "Xdinary Heroes"
    //, "XG"
    //, "YOUNITE"
    //, "ZE"
    //, "Zico"
    //        };

    //public static List<Artist> GetArtists()
    //{
    //    // In the future get artist list from text file like
    //    // {2NE1}{}{2009-05-06}{GG}{4}{YG Entertainment}{https://github.com/giannistek1/rpd-audio/blob/main/[2NE1][][2009-05-06][GG][4][YG Entertainment].jpg?raw=true}

    //    if (Artists.Count == 0)
    //    {
    //        List<string> artistNames = SongPartRepository.SongParts.Select(s => s.ArtistName).Distinct().ToList();
    //        foreach (string name in artistNames)
    //        {
    //            Artists.Add(new Artist(name: name, groupType: GroupType.BG));
    //        }
    //    }
    //    return Artists.ToList();
    //}
}
