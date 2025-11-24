using Syncfusion.Maui.Core;
using System.Collections.ObjectModel;

namespace RpdPlayerApp.Views;

public partial class EditChipOptionsPopup
{
    public EditChipOptionsPopup(string title, string[] options, int[] selectedIndices)
    {
        InitializeComponent();

        TitleLabel.Text = title;
        InitializeChipGroup(options, selectedIndices);
    }

    private void InitializeChipGroup(string[] options, int[] selectedIndices)
    {
        // Fill options.
        foreach (var option in options)
        {
            ChipGroup?.Items?.Add(new SfChip() { Text = option, TextColor = (Color)Application.Current!.Resources["PrimaryTextColor"] });
        }

        // Load selection
        if (selectedIndices != null)
        {
            var selectedItems = new ObservableCollection<SfChip>();
            foreach (var index in selectedIndices)
            {
                selectedItems.Add(ChipGroup!.Items![index]);
            }
            ChipGroup!.SelectedItem = selectedItems;
        }
    }

    private void OnOkClicked(object sender, EventArgs e) => Close(ChipGroup);
    private void OnCancelClicked(object sender, EventArgs e) => Close(null);
}