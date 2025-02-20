using RpdPlayerApp.Enums;
using RpdPlayerApp.Repositories;
using RpdPlayerApp.ViewModels;

namespace RpdPlayerApp.Items;

internal class HomeListViewItem
{
    public string Title { get; set; }
    public string ImageURL { get; set; }
    public string Description { get; set; }
    public string TopArtists { get; set; }
    public string ChildCompanies { get; set; }
    public string OldNames { get; set; }

    /// <summary> Shows artist in gen. </summary>
    public string ArtistName { get; set; }
    public SearchFilterMode SearchFilterMode { get; set; }

    public int SongCount { get; set; }
    public int ArtistCount { get; set; }

    public HomeListViewItem(string title, string imageUrl, SearchFilterMode searchFilterMode, string description = "", string topArtists = "", string childCompanies = "", string oldNames = "", int songCount = 0, int artistCount = 0, string artistName = "")
    {
        Title = title;
        ImageURL = imageUrl;
        Description = description;
        SearchFilterMode = searchFilterMode;
        TopArtists = topArtists;
        ChildCompanies = childCompanies;
        OldNames = oldNames;
        SongCount = songCount;
        ArtistCount = artistCount;
        ArtistName = artistName;
    }

    // TODO: Add extra custom description with famous songs from the year like "Growl, Whiplash, etc"
    internal static HomeListViewItem Create(int year, SearchFilterMode searchFilterMode)
    {
        return new(title: year.ToString(),
                   imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-sk.jpg?raw=true",
                   searchFilterMode: searchFilterMode,
                   songCount: SongPartRepository.SongParts.Count(s => s.Album?.ReleaseDate.Year == year && s.Album?.GenreShort == MainViewModel.GenreKpop));
    }
}
