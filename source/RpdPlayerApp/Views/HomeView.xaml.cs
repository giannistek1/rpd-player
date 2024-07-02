using RpdPlayerApp.Repository;
using System.Collections.Specialized;

namespace RpdPlayerApp.Views;

public partial class HomeView : ContentView
{
    public HomeView()
    {
        InitializeComponent();

        VersionLabel.Text = $"v{AppInfo.Current.VersionString}.{AppInfo.Current.BuildString}";

        ArtistRepository.Artists.CollectionChanged += ArtistsCollectionChanged;
        AlbumRepository.Albums.CollectionChanged += AlbumsCollectionChanged;
        SongPartRepository.SongParts.CollectionChanged += SongPartsCollectionChanged;

        // Get

        AlbumRepository.GetAlbums();
        SongPartRepository.GetSongParts();
        ArtistRepository.GetArtists(); // First when artists are added


        var groupedArtists = from s in SongPartRepository.SongParts
                             group s.Artist by s.ArtistName into g
                             orderby g.Count() descending
                             select new { ArtistName = g.Key, ArtistCount = g.Count(), Artists = g.ToList() };

        var groupedTitles = from s in SongPartRepository.SongParts
                            group s.Title by s.Title into g
                            select new { Title = g.Key, Titles = g.ToList() };

        UniqueSongCountLabel.Text = $",  Unique songs: {groupedTitles.Count()}";

        RandomListView.ItemsSource = groupedArtists;
        SentrySdk.CaptureMessage("Hello Sentry");
    }

    private void SongPartsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        SongPartCountLabel.Text = $"SongParts: {SongPartRepository.SongParts.Count}";
    }

    private void ArtistsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        ArtistCountLabel.Text = $"Artists: {ArtistRepository.Artists.Count}";
    }

    private void AlbumsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        AlbumCountLabel.Text = $",  Albums: {AlbumRepository.Albums.Count}";
    }

    private void RandomListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        //PlaylistManager.Instance.RemoveSongpartOfCurrentPlaylist((Songpart)RandomListView.SelectedItem);

        RandomListView.SelectedItem = null;
    }
}