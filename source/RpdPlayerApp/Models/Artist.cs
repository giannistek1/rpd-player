﻿using RpdPlayerApp.Architecture;
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
    public string ImageURL { get; set; }

    public bool ShowGroupType { get; set; } = false;
    public bool ShowMemberCount { get; set; } = false;
    public bool ShowCompany { get; set; } = false;


    public int TotalCount { get; set; } = 0;
    public int FilteredTotalCount { get; set; } = 0;

    public Artist(int id = -1, string name = "", string altName = "", DateTime debutDate = new(), GroupType groupType = GroupType.NOT_SET, int memberCount = 1, string company = "", string imageURL = "")
    {
        Id = id;
        Name = name;
        AltName = altName;
        DebutDate = debutDate;
        GroupType = groupType;
        MemberCount = memberCount;
        Company = company;
        ImageURL = imageURL;

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
        
        switch(groupType)
        {
            case GroupType.BG: GroupTypeColor = Colors.DeepSkyBlue.ToHex(); break;
            case GroupType.GG: GroupTypeColor = Colors.Magenta.ToHex(); break;
            case GroupType.MIX: GroupTypeColor = Colors.Gray.ToHex(); break;
            case GroupType.NOT_SET: GroupTypeColor = Colors.White.ToHex(); break;
            
            default: GroupTypeColor = Colors.White.ToHex(); break;
        }
    }

    public override string ToString()
    {
        return Name;
    }
}
