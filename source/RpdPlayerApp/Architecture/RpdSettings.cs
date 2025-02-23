﻿using RpdPlayerApp.Models;
using RpdPlayerApp.ViewModels;
using Syncfusion.Maui.Core;

namespace RpdPlayerApp.Architecture;

internal class RpdSettings
{
    public RpdSettings()
    {
    }

    /// <summary> When false, mode is StartRpd. </summary>
    internal bool UsingGeneratePlaylist { get; set; } = false;

    internal TimeSpan Duration { get; set; }
    internal List<GroupType> GroupTypes { get; set; } = [];
    internal List<string> Genres { get; set; } = [];
    internal List<Gen> Gens { get; set; } = [];
    internal List<string> Companies { get; set; } = [];

    internal List<string> OtherCompanies { get; set; } = [];
    internal List<int> Years { get; set; } = [];
    internal List<string> NumberedPartsBlacklist { get; set; } = [];
    internal List<string> PartsBlacklist { get; set; } = [];

    internal void DetermineGroupTypes(SfChipGroup grouptypesChipGroup)
    {
        GroupTypes.Clear();
        for (var i = 0; i < grouptypesChipGroup?.Items?.Count; i++)
        {
            if (grouptypesChipGroup.Items[i].IsSelected)
            {
                GroupType groupType = grouptypesChipGroup.Items[i].Text switch
                {
                    "Male" => GroupType.BG,
                    "Female" => GroupType.GG,
                    "Mixed" => GroupType.MIX,
                    _ => GroupType.NOT_SET
                };
                GroupTypes.Add(groupType);
            }
        }
    }

    internal void DetermineGenres(SfChipGroup genresChipGroup)
    {
        Genres.Clear();
        for (var i = 0; i < genresChipGroup?.Items?.Count; i++)
        {
            if (genresChipGroup.Items[i].IsSelected)
            {
                string genre = genresChipGroup.Items[i].Text;
                Genres.Add(genre);
            }
        }
    }

    internal void DetermineGens(SfChipGroup generationsChipGroup)
    {
        Gens.Clear();
        for (var i = 0; i < generationsChipGroup?.Items?.Count; i++)
        {
            if (generationsChipGroup.Items[i].IsSelected)
            {
                Gen gen = generationsChipGroup.Items[i].Text switch
                {
                    "1" => Gen.First,
                    "2" => Gen.Second,
                    "3" => Gen.Third,
                    "4" => Gen.Fourth,
                    "5" => Gen.Fifth,
                    _ => Gen.NotKpop
                };
                Gens.Add(gen);
            }
        }
    }

    internal void DetermineCompanies(SfChipGroup companiesChipGroup)
    {
        Companies.Clear();
        for (var i = 0; i < companiesChipGroup?.Items?.Count; i++)
        {
            if (companiesChipGroup.Items[i].IsSelected)
            {
                switch (companiesChipGroup.Items[i].Text)
                {
                    case "SM": Companies.AddRange(MainViewModel.SMCompanies); break;
                    case "HYBE": Companies.AddRange(MainViewModel.HybeCompanies); break;
                    case "JYP": Companies.Add("JYP Entertainment"); break;
                    case "YG": Companies.AddRange(MainViewModel.YGCompanies); break;
                    case "Others": Companies.AddRange(OtherCompanies); break;
                }
            }
        }
    }

    internal void DetermineYears(SfChipGroup yearsChipGroup)
    {
        Years.Clear();
        for (var i = 0; i < yearsChipGroup?.Items?.Count; i++)
        {
            if (yearsChipGroup.Items[i].IsSelected)
            {
                if (yearsChipGroup.Items[i].Text.Equals("< 2012", StringComparison.OrdinalIgnoreCase))
                {
                    for (int year = 1998; year <= 2011; year++)
                    {
                        Years.Add(year);
                    }
                }
                else
                {
                    Years.Add(Convert.ToInt32(yearsChipGroup.Items[i].Text));
                }
            }
        }
    }
}