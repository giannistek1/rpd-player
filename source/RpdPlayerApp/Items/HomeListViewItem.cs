using RpdPlayerApp.Architecture;
using RpdPlayerApp.Repositories;
using RpdPlayerApp.ViewModels;

namespace RpdPlayerApp.Items;

internal class HomeListViewItem
{
    public string Title { get; set; }
    public string Description { get; set; }
    public SearchFilterMode SearchFilterMode { get; set; }

    public string ImageURL { get; set; }
    public int SongCount { get; set; }

    public HomeListViewItem(string title, string description, string imageUrl, SearchFilterMode searchFilterMode, int songCount = 0)
    {
        Title = title;
        Description = description;
        ImageURL = imageUrl;
        SearchFilterMode = searchFilterMode;
        SongCount = songCount;
    }

    // TODO: Add extra custom description with famous songs from the year like "Growl, Whiplash, etc"
    internal static HomeListViewItem Create(int year, SearchFilterMode searchFilterMode)
    {
        return new(title: year.ToString(), description: $"K-pop {year}",
                   imageUrl: $"https://github.com/giannistek1/rpd-images/blob/main/home-sk.jpg?raw=true",
                   searchFilterMode: searchFilterMode,
                   songCount: SongPartRepository.SongParts.Count(s => s.Album?.ReleaseDate.Year == year && s.Album?.GenreShort == MainViewModel.GenreKpop));
    }
}
