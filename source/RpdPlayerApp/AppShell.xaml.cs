using RpdPlayerApp.Views;

namespace RpdPlayerApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
        Routing.RegisterRoute(nameof(VideoPage), typeof(VideoPage));
        Routing.RegisterRoute(nameof(FeedbackPage), typeof(FeedbackPage));
    }
}
