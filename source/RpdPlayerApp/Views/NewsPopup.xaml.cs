using CommunityToolkit.Maui.Views;
using RpdPlayerApp.Items;
using RpdPlayerApp.Models;

namespace RpdPlayerApp.Views;

public partial class NewsPopup : Popup
{
    internal List<SongPart> NewsItems { get; set; } = [];

    public NewsPopup()
    {
        InitializeComponent(); // Namespace is needed in XAML for this to be recognized.

        NewsListView.ItemsSource = NewsItems;
    }

    private void CloseImageButtonPressed(object sender, EventArgs e) => Close();
}