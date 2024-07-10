using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Storage;
using Microsoft.Extensions.Logging;
using RpdPlayerApp.Views;
using UraniumUI;

namespace RpdPlayerApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseUraniumUI()
                .UseUraniumUIMaterial()
                .UseSentry(options => {
                        // The DSN is the only required setting.
                        options.Dsn = "https://6ebbf9f141d92c4913f52e13810622fb@o4507512238637056.ingest.de.sentry.io/4507512245256272";

                        // Use debug mode if you want to see what the SDK is doing.
                        // Debug messages are written to stdout with Console.Writeline,
                        // and are viewable in your IDE's debug console or with 'adb logcat', etc.
                        // This option is not recommended when deploying your application.
#if !RELEASE
                        options.Debug = true;
#endif

                        // Set TracesSampleRate to 1.0 to capture 100% of transactions for performance monitoring.
                        // We recommend adjusting this value in production.
                        options.TracesSampleRate = 1.0;

                        // Sample rate for profiling, applied on top of othe TracesSampleRate, 
                        // e.g. 0.2 means we want to profile 20 % of the captured transactions.
                        // We recommend adjusting this value in production.
                        options.ProfilesSampleRate = 1.0;

                        // Other Sentry options can be set here.
                    })
                .UseMauiCommunityToolkitMediaElement()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddMaterialIconFonts();
                    fonts.AddMaterialSymbolsFonts();
                    fonts.AddFontAwesomeIconFonts();
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<IFileSaver>(FileSaver.Default);
            builder.Services.AddTransient<MainPage>();

            return builder.Build();
        }
    }
}
