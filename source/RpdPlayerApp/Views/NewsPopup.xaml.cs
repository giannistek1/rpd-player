using CommunityToolkit.Maui.Views;
using RpdPlayerApp.Items;

namespace RpdPlayerApp.Views;

public partial class NewsPopup : Popup
{
	internal List<NewsItem> NewsItems { get; set; } = [];

	public NewsPopup()
	{
		InitializeComponent(); // Namespace is needed in XAML for this to be recognized.

		NewsListView.ItemsSource = NewsItems;
	}

    private void CloseImageButton_Pressed(object sender, EventArgs e) => Close();
}