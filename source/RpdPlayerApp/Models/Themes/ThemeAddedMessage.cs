using CommunityToolkit.Mvvm.Messaging.Messages;


namespace RpdPlayerApp.Models.Themes;

public class ThemeAddedMessage : ValueChangedMessage<string>
{
    public ThemeAddedMessage(string value) : base(value)
    {
    }
}
