using RpdPlayerApp.Architecture;
using System.Collections.ObjectModel;

namespace RpdPlayerApp.Views;

public partial class ImportSegmentResultsPopup
{
    internal ImportSegmentResultsPopup(ObservableCollection<ImportResult> failedImports)
    {
        InitializeComponent();

        FailedImportsLabel.Text = $"Failed imports: {failedImports.Count}";

        CloseButton.Pressed += CloseButtonPressed;
        FailedImportsListView.ItemsSource = failedImports;
    }

    private void CloseButtonPressed(object? sender, EventArgs e) => Close();
}