using RpdPlayerApp.Architecture;

namespace RpdPlayerApp.Views;

public partial class InputPromptPopup
{
    private int _minLength = 0;
    public InputPromptPopup(string title, string placeholder, int minLength = 0, int maxLength = 26)
    {
        InitializeComponent();

        TitleLabel.Text = title;
        InputEntry.Placeholder = placeholder;
        _minLength = minLength;
        InputEntry.MaxLength = maxLength;

        InputEntry.Focus();
    }

    private void OnOkClicked(object sender, EventArgs e)
    {
        bool isNullable = (_minLength == 0);

        if (isNullable || InputEntry.Text is not null && InputEntry.Text.Length >= _minLength)
        {
            Close(new InputPromptResult(InputEntry.Text, false));
        }
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        Close(new InputPromptResult(string.Empty, true));
    }
}