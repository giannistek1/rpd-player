using UraniumUI.Icons.MaterialSymbols;

namespace RpdPlayerApp.Architecture;

internal static class IconManager
{
    internal static FontImageSource AutoplayIcon = new FontImageSource
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Autoplay,
        Color = (Color)Application.Current!.Resources["IconColor"]
    }; 
    internal static FontImageSource BackIcon = new FontImageSource
    {
        FontFamily = "MaterialRounded",
        #if IOS
            Glyph = MaterialRounded.Arrow_back_ios,
        #else
            Glyph = MaterialRounded.Arrow_back,
        #endif
        Color = (Color)Application.Current!.Resources["IconColor"]
    };
    internal static FontImageSource OffIcon = new FontImageSource
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Block,
        Color = (Color)Application.Current!.Resources["IconColor"]
    };
    internal static FontImageSource PauseIcon = new FontImageSource
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Pause,
        Color = (Color)Application.Current!.Resources["IconColor"]
    };
    internal static FontImageSource PlayIcon = new FontImageSource
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Play_arrow,
        Color = (Color)Application.Current!.Resources["IconColor"]
    };
    internal static FontImageSource RepeatOneIcon = new FontImageSource
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Repeat_one,
        Color = (Color)Application.Current!.Resources["IconColor"]
    };
    internal static FontImageSource ShuffleIcon = new FontImageSource
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Shuffle,
        Color = (Color)Application.Current!.Resources["IconColor"]
    };
    internal static FontImageSource TimerOffIcon = new FontImageSource
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Timer_off,
        Color = (Color)Application.Current!.Resources["IconColor"]
    };
    internal static FontImageSource Timer3Icon = new FontImageSource()
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Timer_3_alt_1,
        Color = (Color)Application.Current!.Resources["IconColor"]
    };
    internal static FontImageSource Timer5Icon = new FontImageSource()
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Timer_5,
        Color = (Color)Application.Current!.Resources["IconColor"]
    };
    internal static FontImageSource VoiceIcon = new FontImageSource
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Voice_selection,
        Color = (Color)Application.Current!.Resources["IconColor"]
    };
    internal static FontImageSource VoiceOffIcon = new FontImageSource
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Voice_selection_off,
        Color = (Color)Application.Current!.Resources["IconColor"]
    };

    internal static FontImageSource ToolbarAddIcon = new FontImageSource()
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Add,
        Color = (Color)Application.Current!.Resources["ToolbarIconColor"]
    };
    internal static FontImageSource ToolbarBackIcon = new FontImageSource()
    {
        FontFamily = "MaterialRounded",
#if IOS
        Glyph = MaterialRounded.Arrow_back_ios,
        #else
            Glyph = MaterialRounded.Arrow_back,
        #endif
        Color = (Color)Application.Current!.Resources["ToolbarIconColor"]
    };
    internal static FontImageSource ToolbarCasinoIcon = new FontImageSource()
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Casino,
        Color = (Color)Application.Current!.Resources["ToolbarIconColor"]
    };
    internal static FontImageSource ToolbarCollapseAllIcon = new FontImageSource()
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Collapse_all,
        Color = (Color)Application.Current!.Resources["ToolbarIconColor"]
    };
    internal static FontImageSource ToolbarClearIcon = new FontImageSource()
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Delete,
        Color = (Color)Application.Current!.Resources["ToolbarIconColor"]
    };
    internal static FontImageSource ToolbarCloudIcon = new FontImageSource
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Cloud,
        Color = (Color)Application.Current!.Resources["ToolbarIconColor"]
    };
    internal static FontImageSource ToolbarCloudOffIcon = new FontImageSource
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Cloud_off,
        Color = (Color)Application.Current!.Resources["ToolbarIconColor"]
    };
    internal static FontImageSource ToolbarExpandAllIcon = new FontImageSource()
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Expand_all,
        Color = (Color)Application.Current!.Resources["ToolbarIconColor"]
    };
    internal static FontImageSource ToolbarMoreItemsIcon = new FontImageSource()
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.More_vert,
        Color = (Color)Application.Current!.Resources["ToolbarIconColor"]
    };
    internal static FontImageSource ToolbarPlayIcon = new FontImageSource()
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Play_arrow,
        Color = (Color)Application.Current!.Resources["ToolbarIconColor"]
    };
    internal static FontImageSource ToolbarRateReviewIcon = new FontImageSource
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Rate_review,
        Color = (Color)Application.Current!.Resources["ToolbarIconColor"]
    };
    internal static FontImageSource ToolbarSaveIcon = new FontImageSource()
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Save,
        Color = (Color)Application.Current!.Resources["ToolbarIconColor"]
    };
    internal static FontImageSource ToolbarSettingsIcon = new FontImageSource()
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Settings,
        Color = (Color)Application.Current!.Resources["ToolbarIconColor"]
    };
    internal static FontImageSource ToolbarSortIcon = new FontImageSource()
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Sort,
        Color = (Color)Application.Current!.Resources["ToolbarIconColor"]
    };
    internal static FontImageSource ToolbarVideoIcon = new FontImageSource
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Videocam,
        Color = (Color)Application.Current!.Resources["ToolbarIconColor"]
    };
    internal static FontImageSource ToolbarVideoOffIcon = new FontImageSource
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Videocam_off,
        Color = (Color)Application.Current!.Resources["ToolbarIconColor"]
    };

    /// <summary>
    /// When theme updates
    /// </summary>
    internal static void RefreshIcons()
    {
        AutoplayIcon = new FontImageSource
        {
            FontFamily = "MaterialRounded",
            Glyph = MaterialRounded.Autoplay,
            Color = (Color)Application.Current!.Resources["IconColor"]
        };
        BackIcon = new FontImageSource
        {
            FontFamily = "MaterialRounded",
            #if IOS
            Glyph = MaterialRounded.Arrow_back_ios,
            #else
                    Glyph = MaterialRounded.Arrow_back,
            #endif
            Color = (Color)Application.Current!.Resources["IconColor"]
        };
        OffIcon = new FontImageSource
        {
            FontFamily = "MaterialRounded",
            Glyph = MaterialRounded.Block,
            Color = (Color)Application.Current!.Resources["IconColor"]
        };
        PauseIcon = new FontImageSource
        {
            FontFamily = "MaterialRounded",
            Glyph = MaterialRounded.Pause,
            Color = (Color)Application.Current!.Resources["IconColor"]
        };
        PlayIcon = new FontImageSource
        {
            FontFamily = "MaterialRounded",
            Glyph = MaterialRounded.Play_arrow,
            Color = (Color)Application.Current!.Resources["IconColor"]
        };
        RepeatOneIcon = new FontImageSource
        {
            FontFamily = "MaterialRounded",
            Glyph = MaterialRounded.Repeat_one,
            Color = (Color)Application.Current!.Resources["IconColor"]
        };
        ShuffleIcon = new FontImageSource
        {
            FontFamily = "MaterialRounded",
            Glyph = MaterialRounded.Shuffle,
            Color = (Color)Application.Current!.Resources["IconColor"]
        };
        TimerOffIcon = new FontImageSource
        {
            FontFamily = "MaterialRounded",
            Glyph = MaterialRounded.Timer_off,
            Color = (Color)Application.Current!.Resources["IconColor"]
        };
        Timer3Icon = new FontImageSource()
        {
            FontFamily = "MaterialRounded",
            Glyph = MaterialRounded.Timer_3_alt_1,
            Color = (Color)Application.Current!.Resources["IconColor"]
        };
        Timer5Icon = new FontImageSource
        {
            FontFamily = "MaterialRounded",
            Glyph = MaterialRounded.Timer_5,
            Color = (Color)Application.Current!.Resources["IconColor"]
        };
        VoiceIcon = new FontImageSource
        {
            FontFamily = "MaterialRegular",
            Glyph = MaterialRounded.Voice_selection,
            Color = (Color)Application.Current!.Resources["IconColor"]
        };
        VoiceOffIcon = new FontImageSource
        {
            FontFamily = "MaterialRegular",
            Glyph = MaterialRounded.Voice_selection_off,
            Color = (Color)Application.Current!.Resources["IconColor"]
        };

        ToolbarAddIcon = new FontImageSource
        {
            FontFamily = "MaterialRounded",
            Glyph = MaterialRounded.Add,
            Color = (Color)Application.Current!.Resources["ToolbarIconColor"]
        };
        ToolbarBackIcon = new FontImageSource
        {
            FontFamily = "MaterialRounded",
            #if IOS
            Glyph = MaterialRounded.Arrow_back_ios,
            #else
                Glyph = MaterialRounded.Arrow_back,
            #endif
            Color = (Color)Application.Current!.Resources["ToolbarIconColor"]
        };
        ToolbarCasinoIcon = new FontImageSource
        {
            FontFamily = "MaterialRounded",
            Glyph = MaterialRounded.Casino,
            Color = (Color)Application.Current!.Resources["ToolbarIconColor"]
        };
        ToolbarCollapseAllIcon = new FontImageSource
        {
            FontFamily = "MaterialRounded",
            Glyph = MaterialRounded.Collapse_all,
            Color = (Color)Application.Current!.Resources["ToolbarIconColor"]
        };
        ToolbarClearIcon = new FontImageSource
        {
            FontFamily = "MaterialRounded",
            Glyph = MaterialRounded.Delete,
            Color = (Color)Application.Current!.Resources["ToolbarIconColor"]
        };
        ToolbarCloudIcon = new FontImageSource
        {
            FontFamily = "MaterialRounded",
            Glyph = MaterialRounded.Cloud,
            Color = (Color)Application.Current!.Resources["ToolbarIconColor"]
        };
        ToolbarCloudOffIcon = new FontImageSource
        {
            FontFamily = "MaterialRounded",
            Glyph = MaterialRounded.Cloud_off,
            Color = (Color)Application.Current!.Resources["ToolbarIconColor"]
        };
        ToolbarExpandAllIcon = new FontImageSource
        {
            FontFamily = "MaterialRounded",
            Glyph = MaterialRounded.Expand_all,
            Color = (Color)Application.Current!.Resources["ToolbarIconColor"]
        };
        ToolbarMoreItemsIcon = new FontImageSource
        {
            FontFamily = "MaterialRounded",
            Glyph = MaterialRounded.More_vert,
            Color = (Color)Application.Current!.Resources["ToolbarIconColor"]
        };
        ToolbarPlayIcon = new FontImageSource
        {
            FontFamily = "MaterialRounded",
            Glyph = MaterialRounded.Play_arrow,
            Color = (Color)Application.Current!.Resources["ToolbarIconColor"]
        };
        ToolbarRateReviewIcon = new FontImageSource
        {
            FontFamily = "MaterialRounded",
            Glyph = MaterialRounded.Rate_review,
            Color = (Color)Application.Current!.Resources["ToolbarIconColor"]
        };
        ToolbarSaveIcon = new FontImageSource
        {
            FontFamily = "MaterialRounded",
            Glyph = MaterialRounded.Save,
            Color = (Color)Application.Current!.Resources["ToolbarIconColor"]
        };
        ToolbarSettingsIcon = new FontImageSource()
        {
            FontFamily = "MaterialRounded",
            Glyph = MaterialRounded.Settings,
            Color = (Color)Application.Current!.Resources["ToolbarIconColor"]
        };
        ToolbarSortIcon = new FontImageSource
        {
            FontFamily = "MaterialRounded",
            Glyph = MaterialRounded.Sort,
            Color = (Color)Application.Current!.Resources["ToolbarIconColor"]
        }; 
        ToolbarVideoIcon = new FontImageSource
        {
            FontFamily = "MaterialRegular",
            Glyph = MaterialRounded.Videocam,
            Color = (Color)Application.Current!.Resources["ToolbarIconColor"]
        };
        ToolbarVideoOffIcon = new FontImageSource
        {
            FontFamily = "MaterialRegular",
            Glyph = MaterialRounded.Videocam_off,
            Color = (Color)Application.Current!.Resources["ToolbarIconColor"]
        };
    }
}
