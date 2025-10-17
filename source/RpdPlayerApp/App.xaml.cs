using CommunityToolkit.Mvvm.Messaging;
using RpdPlayerApp.Models.Themes;
using RpdPlayerApp.Services;
using System.Text.Json;

namespace RpdPlayerApp
{
    public partial class App : Application
    {
        public App()
        {
            // https://www.syncfusion.com/account/manage-trials/trial-history to see what version license is active
            // Claim license make sure to "Select all".
            // Register Syncfusion license
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NMaF5cXmBCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWX5feXVRQ2lZVEB0XUc=");

            InitializeComponent();

            MainPage = new AppShell();

            InitTheme();

            _ = SupabaseService.InitializeAsync();
        }

        private void InitTheme()
        {
            // Subscribe on ThemeChangedMessage to loadtheme
            WeakReferenceMessenger.Default.Register<ThemeChangedMessage>(this, (r, m) =>
            {
                LoadTheme(m.Value);
            });

            var theme = Preferences.Get("theme", "System");

            LoadTheme(theme);
        }

        private void LoadTheme(string theme)
        {
            if (!MainThread.IsMainThread)
            {
                MainThread.BeginInvokeOnMainThread(() => LoadTheme(theme));
                return;
            }

            if (theme == "System")
            {
                theme = Current!.PlatformAppTheme.ToString();
            }

            // Don't forget to add in ThemeViewModel.cs!
            ResourceDictionary? dictionary = theme switch
            {
                "Dark" => new Resources.Styles.Dark(),
                "Light" => new Resources.Styles.Light(),
                "Halloween" => new Resources.Styles.Halloween(),
                "Christmas" => new Resources.Styles.Christmas(),
                "Custom" => LoadCustomTheme(),
                _ => null
            };

            if (dictionary != null)
            {
                Resources.MergedDictionaries.Clear();

                Resources.MergedDictionaries.Add(dictionary);
            }
        }

        private ResourceDictionary LoadCustomTheme()
        {
            var json = Preferences.Get("CustomTheme", null);

            var data = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

            var dictionary = new ResourceDictionary();

            foreach (var item in data)
            {
                dictionary.Add(item.Key, Color.FromArgb(item.Value));
            }

            return dictionary;
        }
    }
}
