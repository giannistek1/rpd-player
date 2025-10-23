﻿using CommunityToolkit.Mvvm.ComponentModel;
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

internal partial class Artist : ObservableObject
{
    public int Id { get; set; }

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _altName;

    [ObservableProperty]
    private DateTime _debutDate;

    [ObservableProperty]
    private GroupType _groupType;

    [JsonIgnore]
    [ObservableProperty]
    private bool _showGroupTypeColor = false;

    [ObservableProperty]
    private string _groupTypeColor;

    [ObservableProperty]
    private int _memberCount;

    [ObservableProperty]
    private string _company;

    [ObservableProperty]
    private string _generation = Constants.NOT_KPOP;

    [ObservableProperty]
    private GenType _gen;

    /// <summary> Gets set in SongPart.cs once an album is Korean. </summary>
    [ObservableProperty]
    private bool _isKpopArtist = false;

    /// <summary> Example: "https://github.com/giannistek1/rpd-artists/blob/main/%5BAOA%5D%5BAce of Angels%5D%5B2012-07-30%5D%5BGG%5D%5B8%5D%5BFNC Entertainment%5D.jpg?raw=true" </summary>
    [ObservableProperty]
    private string _imageUrl;

    [JsonIgnore]
    [ObservableProperty]
    private bool _showGroupType = false;

    [JsonIgnore]
    [ObservableProperty]
    private bool _showMemberCount = false;

    [JsonIgnore]
    [ObservableProperty]
    private bool _showCompany = false;

    [ObservableProperty]
    private int _songPartCount = 0;

    [ObservableProperty]
    private int _filteredTotalCount = 0;

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
            Gen = GenType.First;
        }
        else if (DebutDate > Constants.secondGenStartDate && DebutDate < Constants.thirdGenStartDate)
        {
            Generation = Constants.SECOND_GENERATION;
            Gen = GenType.Second;
        }
        else if (DebutDate > Constants.thirdGenStartDate && DebutDate < Constants.fourthGenStartDate)
        {
            Generation = Constants.THIRD_GENERATION;
            Gen = GenType.Third;
        }
        else if (DebutDate > Constants.fourthGenStartDate && DebutDate < Constants.fifthGenStartDate)
        {
            Generation = Constants.FOURTH_GENERATION;
            Gen = GenType.Fourth;
        }
        else if (DebutDate > Constants.fifthGenStartDate)
        {
            Generation = Constants.FIFTH_GENERATION;
            Gen = GenType.Fifth;
        }
    }

    public override string ToString() => Name;
}