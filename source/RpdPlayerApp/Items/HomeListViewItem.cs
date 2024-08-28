using RpdPlayerApp.Architecture;

namespace RpdPlayerApp.Items
{
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
    }
}
