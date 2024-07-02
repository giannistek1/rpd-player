using CommunityToolkit.Maui.Storage;
using RpdPlayerApp.ViewModel;

namespace RpdPlayerApp.Views;

public partial class MainPage : ContentPage
{
    IFileSaver fileSaver;
    public MainPage(IFileSaver fileSaver)
	{
		InitializeComponent();
        // In case you want to save files to a user chosen place
        this.fileSaver = fileSaver;

        SearchSongPartsView.PlaySongPart += OnPlaySongPart;
        LibraryView.PlaySongPart += OnPlaySongPart;
    }

    private void OnPlaySongPart(object sender, EventArgs e)
    {
        if (MainViewModel.CurrentSongPart is not null)
            AudioPlayerControl.PlayAudio(MainViewModel.CurrentSongPart);
    }
}