using Newtonsoft.Json;
using RpdPlayerApp.Architecture;

namespace RpdPlayerApp.Models;

public enum GroupType
{
    BG = 0,
    GG = 1,
    MIX = 2,
    NOT_SET = 3
}

internal class Artist
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string AltName { get; set; }
    public DateTime DebutDate { get; set; }
    public GroupType GroupType { get; set; }

    [JsonIgnore]
    public bool ShowGroupTypeColor { get; set; } = false;

    public string GroupTypeColor { get; set; }
    public int MemberCount { get; set; }
    public string Company { get; set; }
    public string Generation { get; set; } = Constants.NOT_KPOP;
    public Gen Gen { get; set; }

    /// <summary> Gets set in SongPart.cs once an album is Korean. </summary>
    public bool IsKpopArtist { get; set; } = false;

    /// <summary> Example: "https://github.com/giannistek1/rpd-artists/blob/main/%5BAOA%5D%5BAce of Angels%5D%5B2012-07-30%5D%5BGG%5D%5B8%5D%5BFNC Entertainment%5D.jpg?raw=true" </summary>
    public string ImageUrl { get; set; }

    [JsonIgnore]
    public bool ShowGroupType { get; set; } = false;

    [JsonIgnore]
    public bool ShowMemberCount { get; set; } = false;

    [JsonIgnore]
    public bool ShowCompany { get; set; } = false;

    public int SongPartCount { get; set; } = 0;
    public int FilteredTotalCount { get; set; } = 0;

    public Artist(DateTime debutDate, int id = -1, string name = "", string altName = "", GroupType groupType = GroupType.NOT_SET, int memberCount = 1, string company = "", string imageURL = "")
    {
        Id = id;
        Name = name;
        AltName = altName;
        DebutDate = debutDate;
        GroupType = groupType;
        MemberCount = memberCount;
        Company = company;
        ImageUrl = imageURL;

        GroupTypeColor = groupType switch
        {
            GroupType.BG => Colors.DeepSkyBlue.ToHex(),
            GroupType.GG => Colors.Magenta.ToHex(),
            GroupType.MIX => Colors.Gray.ToHex(),
            GroupType.NOT_SET => Colors.White.ToHex(),

            _ => Colors.White.ToHex(),
        };
    }

    /// <summary> Kpop only. </summary>
    public void DecideGeneration()
    {
        if (DebutDate < Constants.secondGenStartDate)
        {
            Generation = Constants.FIRST_GENERATION;
            Gen = Gen.First;
        }
        else if (DebutDate > Constants.secondGenStartDate && DebutDate < Constants.thirdGenStartDate)
        {
            Generation = Constants.SECOND_GENERATION;
            Gen = Gen.Second;
        }
        else if (DebutDate > Constants.thirdGenStartDate && DebutDate < Constants.fourthGenStartDate)
        {
            Generation = Constants.THIRD_GENERATION;
            Gen = Gen.Third;
        }
        else if (DebutDate > Constants.fourthGenStartDate && DebutDate < Constants.fifthGenStartDate)
        {
            Generation = Constants.FOURTH_GENERATION;
            Gen = Gen.Fourth;
        }
        else if (DebutDate > Constants.fifthGenStartDate)
        {
            Generation = Constants.FIFTH_GENERATION;
            Gen = Gen.Fifth;
        }
    }

    public override string ToString() => Name;
}