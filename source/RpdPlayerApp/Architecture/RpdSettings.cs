using RpdPlayerApp.Enums;
using RpdPlayerApp.Managers;
using RpdPlayerApp.Models;
using RpdPlayerApp.Repositories;
using RpdPlayerApp.Services;
using Syncfusion.Maui.Core;

namespace RpdPlayerApp.Architecture;

public class RpdSettings
{
    public RpdSettings()
    {
        // Default value.
        Years = Enumerable.Range(Constants.LOWEST_YEAR, DateTime.Now.Year - Constants.LOWEST_YEAR + 1).ToList();
    }

    /// <summary> When false, mode is StartRpd. </summary>
    internal static bool UsingGeneratePlaylist { get; set; } = false;

    internal static CountdownModeValue CountdownMode { get; set; } = CountdownModeValue.Off;
    internal static AnnouncementModeValue AnnouncementMode { get; set; } = AnnouncementModeValue.Off;
    internal static TimeSpan Duration { get; set; } = TimeSpan.FromHours(1);

    internal static List<int> GroupTypesSelectedIndices { get; set; } = [0, 1, 2];
    internal static List<GroupType> GroupTypes { get; set; } = [GroupType.BG, GroupType.GG, GroupType.MIX];

    internal static List<int> GenresSelectedIndices { get; set; } = [0, 1, 2, 3, 4, 5];
    internal static List<string> Genres { get; set; } = ["K-pop", "K-RnB", "J-pop", "C-pop", "T-pop", "Pop"];
    internal static List<int> GensSelectedIndices { get; set; } = [0, 1, 2, 3, 4, 5];
    internal static List<GenType> Gens { get; set; } = [GenType.First, GenType.Second, GenType.Third, GenType.Fourth, GenType.Fifth, GenType.NotKpop];
    internal static List<int> CompaniesSelectedIndices { get; set; } = [0, 1, 2, 3, 4];
    /// <summary> Selected companies  </summary>
    internal static List<string> Companies { get; set; } = [];
    internal static List<string> OtherCompanies { get; set; } = [];

    internal static List<int> YearsSelectedIndices { get; set; } = [.. Constants.SELECTED_YEARS_INDICES_DEFAULT];
    internal static List<int> Years { get; set; } = [];

    internal static Dictionary<string, bool> SelectedOtherOptions { get; set; } = new Dictionary<string, bool>
    {
        { "No last chorus", false },
        { "No dance breaks", false },
        { "No tiktoks", false }
    };
    internal static List<string> NumberedPartsBlacklist { get; set; } = [];
    internal static List<string> PartsBlacklist { get; set; } = [];

    internal void DetermineGroupTypes(SfChipGroup grouptypesChipGroup)
    {
        GroupTypes.Clear();
        GroupTypesSelectedIndices.Clear();
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
                GroupTypesSelectedIndices.Add(i);
            }
        }
    }

    internal void DetermineGenres(SfChipGroup genresChipGroup)
    {
        Genres.Clear();
        GenresSelectedIndices.Clear();
        for (var i = 0; i < genresChipGroup?.Items?.Count; i++)
        {
            if (genresChipGroup.Items[i].IsSelected)
            {
                string genre = genresChipGroup.Items[i].Text;
                Genres.Add(genre);
                GenresSelectedIndices.Add(i);
            }
        }
    }

    internal void DetermineGenerations(SfChipGroup generationsChipGroup)
    {
        Gens.Clear();
        GensSelectedIndices.Clear();
        for (var i = 0; i < generationsChipGroup?.Items?.Count; i++)
        {
            if (generationsChipGroup.Items[i].IsSelected)
            {
                GenType gen = generationsChipGroup.Items[i].Text switch
                {
                    "1" => GenType.First,
                    "2" => GenType.Second,
                    "3" => GenType.Third,
                    "4" => GenType.Fourth,
                    "5" => GenType.Fifth,
                    _ => GenType.NotKpop
                };
                Gens.Add(gen);
                GensSelectedIndices.Add(i);
            }
        }
    }

    internal void DetermineCompanies(SfChipGroup companiesChipGroup)
    {
        Companies.Clear();
        CompaniesSelectedIndices.Clear();
        for (var i = 0; i < companiesChipGroup?.Items?.Count; i++)
        {
            if (companiesChipGroup.Items[i].IsSelected)
            {
                switch (companiesChipGroup.Items[i].Text)
                {
                    case "SM": Companies.AddRange(Constants.SMCompanies); break;
                    case "HYBE": Companies.AddRange(Constants.HybeCompanies); break;
                    case "JYP": Companies.Add("JYP Entertainment"); break;
                    case "YG": Companies.AddRange(Constants.YGCompanies); break;
                    case "Others": Companies.AddRange(OtherCompanies); break;
                }
                CompaniesSelectedIndices.Add(i);
            }
        }
    }

    internal void DetermineYears(SfChipGroup yearsChipGroup)
    {
        Years.Clear();
        YearsSelectedIndices.Clear();
        for (var i = 0; i < yearsChipGroup?.Items?.Count; i++)
        {
            if (yearsChipGroup.Items[i].IsSelected)
            {
                if (yearsChipGroup.Items[i].Text.Equals("< 2012", StringComparison.OrdinalIgnoreCase))
                {
                    for (int year = Constants.LOWEST_YEAR; year <= 2011; year++)
                    {
                        Years.Add(year);
                    }
                }
                else
                {
                    Years.Add(Convert.ToInt32(yearsChipGroup.Items[i].Text));
                }
                YearsSelectedIndices.Add(i);
            }
        }
    }

    internal static string GetCountdownModeText(CountdownModeValue mode) => mode switch
    {
        CountdownModeValue.Off => "Off",
        CountdownModeValue.Short => "3s",
        CountdownModeValue.Long => "5s",
        CountdownModeValue.Custom => "Custom (Pro)",
        _ => "Off"
    };

    internal static string GetAnnouncementModeText(AnnouncementModeValue mode) => mode switch
    {
        AnnouncementModeValue.Off => "Off",
        AnnouncementModeValue.AlwaysSongPart => "Always",
        AnnouncementModeValue.DancebreakOnly => "On dancebreak",
        AnnouncementModeValue.Artist => "By artist",
        AnnouncementModeValue.Specific => "Specific",
        AnnouncementModeValue.GroupType => "By grouptype (BG/GG/MIX)",
        _ => "Off"
    };

    internal static List<SongPart> FilterSongParts()
    {
        if (SongPartRepository.SongParts is null || SongPartRepository.SongParts.Count == 0) { return []; }

        var songParts = SongPartRepository.SongParts.Where(s => GroupTypes.Contains(s.Artist.GroupType))
                                                    .Where(s => Genres.Contains(s.Album.GenreFull))
                                                    .Where(s => !NumberedPartsBlacklist.Contains(s.PartNameShortWithNumber))
                                                    .Where(s => !PartsBlacklist.Contains(s.PartNameShort))
                                                    .Where(s => Years.Contains(s.Album.ReleaseDate.Year))
                                                    .ToList();

        if (GenresSelectedIndices.Contains(0)) // K-pop
        {
            songParts = songParts.Where(s => Gens.Contains(s.Artist.Gen))
                                 .Where(s => Companies.Contains(s.Artist.Company)).ToList();
        }

        return songParts;
    }

    // TODO: Does this belong here?
    internal static void PlayRandomSong(List<SongPart> songParts)
    {
        int index = General.Rng.Next(songParts.Count);
        SongPart songPart = songParts[index];

        AppState.AutoplayMode = AutoplayModeValue.Shuffle;

        AudioManager.ChangeAndStartSong(songPart);
    }

    /// <summary> Fills in companies. </summary>
    internal static void InitializeCompanies()
    {
        if (ArtistRepository.Artists is null || ArtistRepository.Artists.Count == 0) { DebugService.Instance.Warn("Initialize Companies has no artists."); return; }

        Constants.AllCompanies = ArtistRepository.Artists.Select(artist => artist.Company).Distinct().ToList();
        Constants.MainCompanies = Constants.YGCompanies.Concat(Constants.HybeCompanies)
                                                     .Concat(Constants.SMCompanies)
                                                     .Append("JYP Entertainment")
                                                     .ToList();

        OtherCompanies = Constants.AllCompanies.Except(Constants.MainCompanies).ToList();
        Companies = Constants.AllCompanies;
    }
}