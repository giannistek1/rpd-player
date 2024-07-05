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
    public string Name { get; set; }
    public string AltName { get; set; }
    public GroupType Type { get; set; }
    public int MemberCount { get; set; }
    public string Company { get; set; }
    public string ImageURL { get; set; }

    public Artist(string name, string altName = "", GroupType groupType = GroupType.NOT_SET, int memberCount = 1, string company, string imageURL = "")
    {
        Name = name;
        AltName = altName;
        Type = groupType;
        MemberCount = memberCount;
        Company = company;
        ImageURL = imageURL;
    }
}
