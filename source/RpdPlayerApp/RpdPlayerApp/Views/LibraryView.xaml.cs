using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;
using RpdPlayerApp.Models;
using System.Collections.Specialized;

namespace RpdPlayerApp.Views;

public partial class LibraryView : ContentPage
{
    public int SongCount { get; set; } = 0;
    public LibraryView()
    {
        InitializeComponent();

        PlaylistManager.Instance.CurrentPlaylist.CollectionChanged += CurrentPlaylistCollectionChanged;

        CurrentPlaylistListView.ItemsSource = PlaylistManager.Instance.CurrentPlaylist;
    }

    private void CurrentPlaylistCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        CountLabel.Text = PlaylistManager.Instance.GetCurrentPlaylistSongCount().ToString();
    }

    private void CurrentPlaylistListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        PlaylistManager.Instance.RemoveSongpartOfCurrentPlaylist((SongPart)CurrentPlaylistListView.SelectedItem);

        CurrentPlaylistListView.SelectedItem = null;
    }

    private void ClearButton_Clicked(object sender, EventArgs e)
    {
        PlaylistManager.Instance.ClearCurrentPlaylist();
    }

    private void SwipeItem_Invoked(object sender, EventArgs e)
    {
        NetworkAccess accessType = Connectivity.Current.NetworkAccess;

        if (accessType != NetworkAccess.Internet)
        {
            Toast.Make($"No internet connection!", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
            return;
        }

        string url = ((MenuItem)sender).CommandParameter.ToString();
        if (url != string.Empty)
        {
            audioMediaElement.Source = MediaSource.FromUri(url);
            audioMediaElement.Play();
        }
    }

    private void ContentPage_Disappearing(object sender, EventArgs e)
    {
        if (audioMediaElement.CurrentState == CommunityToolkit.Maui.Core.Primitives.MediaElementState.Playing)
            audioMediaElement.Stop();
    }
}