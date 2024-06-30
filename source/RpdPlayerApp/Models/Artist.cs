namespace RpdPlayerApp.Models;

public enum GroupType
{
    BG = 0,
    GG = 1,
    SOLO_MALE = 2,
    SOLO_FEMALE = 3,
    MIX = 4
}

internal class Artist
{
    public string Name { get; set; }
    public GroupType Type { get; set; }
    public int MemberCount { get; set; } = 0;
    public string ImageURL { get; set; }

    public Artist(string name, GroupType groupType, int memberCount = 1, string imageURL = "")
    {
        Name = name;
        Type = groupType;
        MemberCount = memberCount;
        ImageURL = imageURL;
    }
}
