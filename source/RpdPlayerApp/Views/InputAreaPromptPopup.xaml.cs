using RpdPlayerApp.Architecture;

namespace RpdPlayerApp.Views;

public partial class InputAreaPromptPopup
{
    public InputAreaPromptPopup(string title, string placeholder, int maxLength = 20)
    {
        InitializeComponent();
        TitleLabel.Text = title;
        InputEditor.Placeholder = placeholder;
        InputEditor.MaxLength = maxLength;
    }

    void OnOkClicked(object sender, EventArgs e)
    {
        Close(new InputPromptResult(InputEditor.Text, false));
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        Close(new InputPromptResult(string.Empty, true));
    }
}