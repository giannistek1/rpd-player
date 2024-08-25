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
    // TODO
    public Color GroupTypeColor { get; set; } 
    public int MemberCount { get; set; }
    public string Company { get; set; }
    public string ImageURL { get; set; }

    public bool ShowGroupType { get; set; } = false;
    public bool ShowMemberCount { get; set; } = false;
    public bool ShowCompany { get; set; } = false;


    public int TotalCount { get; set; } = 0;
    public int FilteredTotalCount { get; set; } = 0;

    public Artist(int id, string name, string altName = "", DateTime debutDate = new DateTime(), GroupType groupType = GroupType.NOT_SET, int memberCount = 1, string company = "", string imageURL = "")
    {
        Id = id;
        Name = name;
        AltName = altName;
        DebutDate = debutDate;
        GroupType = groupType;
        MemberCount = memberCount;
        Company = company;
        ImageURL = imageURL;
    }
}
