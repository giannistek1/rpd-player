using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using RpdPlayerApp.Views;
using Syncfusion.Maui.Core.Hosting;
using The49.Maui.BottomSheet;
using UraniumUI;


// I think this works compiletime? I don't know why else preferences condition does not work.
namespace RpdPlayerApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            /* fx for: Java.Lang.RuntimeException: Canvas: trying to use a recycled bitmap android.graphics.Bitmap, but may have been fixed in newer versions? */
#if ANDROID
            ImageHandler.Mapper.PrependToMapping(nameof(Microsoft.Maui.IImage.Source), (handler, view) => handler.PlatformView?.Clear());
#endif
            /* Code to remove underline of Android control Entry */
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(Entry), (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
#endif
            });

            builder.UseMauiApp<App>()

            // This fixes the shell title view in iOS not being displayed IF shell toolbar is used
            //                .ConfigureMauiHandlers(handlers =>
            //                {
            //#if IOS
            //                    handlers.AddHandler(typeof(Shell), typeof(RpdPlayerApp.Platforms.iOS.Renderers.CustomShellRenderer));
            //#endif
            //                })

            .UseBottomSheet()
            .UseMauiCommunityToolkit();

            if (true)
            {
                builder.UseSentry(options =>
                {
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
                    options.TracesSampleRate = 0.2;

                    // Sample rate for profiling, applied on top of othe TracesSampleRate, 
                    // e.g. 0.2 means we want to profile 20 % of the captured transactions.
                    // We recommend adjusting this value in production.
                    options.ProfilesSampleRate = 0.2;

                    // Other Sentry options can be set here.
                    options.SetBeforeSend((sentryEvent, hint) =>
                    {
                        // Very annoying log that gets sent every time MediaElement pauses/unpauses. Supposedly fixed in MediaElement v6.0.0 but is present in v4.0.1 - v5.x.x
                        if (sentryEvent.Message is not null && sentryEvent.Message.Formatted.Contains("AndroidX.LocalBroadcastManager.Content.LocalBroadcastManager not supported on Android 13 and above."))
                        {
                            return null; // Don't send this event to Sentry
                        }

                        sentryEvent.ServerName = null; // Never send Server Name to Sentry
                        return sentryEvent;
                    });
                });
            }

            builder.UseMauiCommunityToolkitMediaElement()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddMaterialSymbolsFonts();
                fonts.AddFontAwesomeIconFonts();
            });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddTransient<MainPage>();
            builder.ConfigureSyncfusionCore();

            return builder.Build();
        }
    }
}
