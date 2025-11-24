using RpdPlayerApp.Architecture;

namespace RpdPlayerApp.Views;

public partial class InputPromptPopup
{
    public InputPromptPopup(string title, string placeholder, int maxLength = 20)
    {
        InitializeComponent();

        TitleLabel.Text = title;
        InputEntry.Placeholder = placeholder;
        InputEntry.MaxLength = maxLength;
    }

    private void OnOkClicked(object sender, EventArgs e) => Close(new InputPromptResult(InputEntry.Text, false));

    private void OnCancelClicked(object sender, EventArgs e) => Close(new InputPromptResult(string.Empty, true));
}