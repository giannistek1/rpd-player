using RpdPlayerApp.Architecture;
using System.Collections.ObjectModel;

namespace RpdPlayerApp.Views;

public partial class EditChipOtherOptionsPopup
{
    private readonly ObservableCollection<CustomChipModel> customChips = [];

    public EditChipOtherOptionsPopup(string title, Dictionary<string, bool> options)
    {
        InitializeComponent();

        ChipGroup.SelectionChanged += OnSelectionChanged;

        TitleLabel.Text = title;
        InitializeChipGroup(options);
    }

    private void InitializeChipGroup(Dictionary<string, bool> optionsValues)
    {
        foreach (var option in optionsValues.Keys)
        {
            customChips.Add(new() { Name = option, IsSelected = optionsValues[option] });
        }

        ChipGroup.ItemsSource = customChips;
    }

    private void OnSelectionChanged(object? sender, Syncfusion.Maui.Core.Chips.SelectionChangedEventArgs? e)
    {
        if (e?.AddedItem is not null) ((CustomChipModel)e.AddedItem).IsSelected = true;
        if (e?.RemovedItem is not null) ((CustomChipModel)e.RemovedItem).IsSelected = false;
    }

    private void OnOkClicked(object sender, EventArgs e)
    {
        // Create a copy dictionary from the current chip selections instead of passing reference.
        var selectedOptions = customChips.ToDictionary(chip => chip.Name, chip => chip.IsSelected);
        Close(selectedOptions);
    }
    private void OnCancelClicked(object sender, EventArgs e) => Close(null);
}