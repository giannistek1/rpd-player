﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using RpdPlayerApp.Models.Themes;

namespace RpdPlayerApp.ViewModels;

public partial class ThemeViewModel : ObservableObject
{
    [ObservableProperty]
    private List<Theme> _themes;

    [ObservableProperty]
    private Theme _selectedTheme;

    public ThemeViewModel()
    {
        // Don't forget to add in App.xaml.cs!
        var defaultThemes = new List<Theme>()
        {
            new Theme("Same as system", "System"),
            new Theme("Dark", "Dark"),
            new Theme("Light", "Light"),
            new Theme("Halloween", "Halloween"),
            new Theme("Christmas", "Christmas")
            //new Theme("Blue","Blue"),
            //new Theme("Red", "Red")
        };

        _themes = new List<Theme>(defaultThemes);

        // Load custom theme
        var hasCustom = Preferences.ContainsKey("CustomTheme");
        if (hasCustom)
        {
            _themes.Add(new Theme("Custom theme", "Custom"));
        }

        var theme = Preferences.Get("theme", "System");

        _selectedTheme = _themes.Single(x => x.Key == theme);

        WeakReferenceMessenger.Default.Register<ThemeAddedMessage>(this, (r, m) =>
        {
            _selectedTheme = null;

            if (m.Value == "Custom")
            {
                var customTheme = _themes.SingleOrDefault(x => x.Key == "Custom");

                if (customTheme == null)
                {
                    customTheme = new Theme("Custom theme", "Custom");

                    _themes.Add(customTheme);
                }

                _selectedTheme = customTheme;
            }
        });
    }

    partial void OnSelectedThemeChanged(Theme value)
    {
        if (value == null)
        {
            return;
        }

        Preferences.Set("theme", value.Key);

        WeakReferenceMessenger.Default.Send(new ThemeChangedMessage(value.Key));
    }
}