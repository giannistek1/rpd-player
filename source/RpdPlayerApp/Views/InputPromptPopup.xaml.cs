using CommunityToolkit.Maui.Views;
using RpdPlayerApp.Models;

namespace RpdPlayerApp.Views;

public partial class InputPromptPopup : Popup
{
    public InputPromptPopup(string title, string placeholder)
    {
        InitializeComponent();
        TitleLabel.Text = title;
        InputEntry.Placeholder = placeholder;
    }

    void OnOkClicked(object sender, EventArgs e)
    {
        Close(new InputPromptResult(InputEntry.Text, false));
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        Close(new InputPromptResult(string.Empty, true));
    }
}