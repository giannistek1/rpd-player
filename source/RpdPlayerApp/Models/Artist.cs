using RpdPlayerApp.Architecture;
using RpdPlayerApp.ViewModels;

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
    public bool ShowGroupTypeColor { get; set; } = false;
    public string GroupTypeColor { get; set; } 
    public int MemberCount { get; set; }
    public string Company { get; set; }
    public string Generation { get; set; } = MainViewModel.NOT_KPOP;
    public Gen Gen { get; set; }

    public bool IsKpopArtist { get; set; } = false; // Gets set in SongPart.cs once an album is Korean.
    public string ImageURL { get; set; }

    public bool ShowGroupType { get; set; } = false;
    public bool ShowMemberCount { get; set; } = false;
    public bool ShowCompany { get; set; } = false;


    public int TotalCount { get; set; } = 0;
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
        ImageURL = imageURL;

        GroupTypeColor = groupType switch
        {
            GroupType.BG => Colors.DeepSkyBlue.ToHex(),
            GroupType.GG => Colors.Magenta.ToHex(),
            GroupType.MIX => Colors.Gray.ToHex(),
            GroupType.NOT_SET => Colors.White.ToHex(),

            _ => Colors.White.ToHex(),
        };
    }
    
    public void DecideGeneration()
    {
        // Kpop only
        if (DebutDate < MainViewModel.secondGenStartDate)
        {
            Generation = MainViewModel.FIRST_GENERATION;
            Gen = Gen.First;
        }
        else if (DebutDate > MainViewModel.secondGenStartDate && DebutDate < MainViewModel.thirdGenStartDate)
        {
            Generation = MainViewModel.SECOND_GENERATION;
            Gen = Gen.Second;
        }
        else if (DebutDate > MainViewModel.thirdGenStartDate && DebutDate < MainViewModel.fourthGenStartDate)
        {
            Generation = MainViewModel.THIRD_GENERATION;
            Gen = Gen.Third;
        }
        else if (DebutDate > MainViewModel.fourthGenStartDate && DebutDate < MainViewModel.fifthGenStartDate)
        {
            Generation = MainViewModel.FOURTH_GENERATION;
            Gen = Gen.Fourth;
        }
        else if (DebutDate > MainViewModel.fifthGenStartDate)
        {
            Generation = MainViewModel.FIFTH_GENERATION;
            Gen = Gen.Fifth;
        }
    }

    public override string ToString()
    {
        return Name;
    }
}
