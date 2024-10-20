using CommunityToolkit.Mvvm.Messaging.Messages;

namespace RpdPlayerApp.Models.Themes;

public class ThemeChangedMessage : ValueChangedMessage<string>
{
    public ThemeChangedMessage(string value) : base(value)
    {
    }
}
