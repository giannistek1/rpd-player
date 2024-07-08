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
    public GroupType Type { get; set; }
    public int MemberCount { get; set; }
    public string Company { get; set; }
    public string ImageURL { get; set; }

    public Artist(int id, string name, string altName = "", DateTime debutDate = new DateTime(), GroupType groupType = GroupType.NOT_SET, int memberCount = 1, string company = "", string imageURL = "")
    {
        Id = id;
        Name = name;
        AltName = altName;
        DebutDate = debutDate;
        Type = groupType;
        MemberCount = memberCount;
        Company = company;
        ImageURL = imageURL;
    }
}
