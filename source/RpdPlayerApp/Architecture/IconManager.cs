
using UraniumUI.Icons.MaterialSymbols;

namespace RpdPlayerApp.Architecture;
/// <summary>
/// Fontfamilies: "MaterialRounded, MaterialOutline, MaterialSharp"
/// MaterialRegular is outdated and from UranumUI.Icons.MaterialIcons
/// </summary>
internal static class IconManager
{
#pragma warning disable S2223 // Non-constant static fields should not be visible
    internal static FontImageSource AutoplayIcon = new()
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Autoplay,
        Color = (Color)Application.Current!.Resources["IconColor"]
    };

    internal static FontImageSource BackIcon = new()
    {
        FontFamily = "MaterialRounded",
#if IOS
            Glyph = MaterialRounded.Arrow_back_ios,
#else
        Glyph = MaterialRounded.Arrow_back,
#endif
        Color = (Color)Application.Current!.Resources["IconColor"]
    };

    internal static FontImageSource OffIcon = new()
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Block,
        Color = (Color)Application.Current!.Resources["IconOffColor"]
    };
    internal static FontImageSource PauseIcon = new()
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Pause,
        Color = (Color)Application.Current!.Resources["IconColor"]
    };
    internal static FontImageSource PlayIcon = new()
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Play_arrow,
        Color = (Color)Application.Current!.Resources["IconColor"]
    };
    internal static FontImageSource RepeatOneIcon = new()
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Repeat_one,
        Color = (Color)Application.Current!.Resources["IconColor"]
    };
    internal static FontImageSource ShuffleIcon = new()
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Shuffle,
        Color = (Color)Application.Current!.Resources["IconColor"]
    };

    internal static FontImageSource TimerOffIcon = new()
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Timer_off,
        Color = (Color)Application.Current!.Resources["IconOffColor"]
    };
    internal static FontImageSource Timer3Icon = new()
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Timer_3_alt_1,
        Color = (Color)Application.Current!.Resources["IconColor"]
    };
    internal static FontImageSource Timer5Icon = new()
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Timer_5,
        Color = (Color)Application.Current!.Resources["IconColor"]
    };

    internal static FontImageSource VoiceIcon = new()
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Voice_selection,
        Color = (Color)Application.Current!.Resources["IconColor"]
    };
    internal static FontImageSource VoiceOffIcon = new()
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Voice_selection_off,
        Color = (Color)Application.Current!.Resources["IconOffColor"]
    };

    internal static FontImageSource FavoriteIcon = new()
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Favorite,
        Color = (Color)Application.Current!.Resources["IconColor"]
    };

    internal static FontImageSource FavoritedIcon = new()
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Favorite,
        Color = (Color)Application.Current!.Resources["FavoritedIconColor"]
    };

    internal static FontImageSource CrossIcon = new()
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Close,
        Color = (Color)Application.Current!.Resources["IconColor"]
    };

    // Toolbar icons
    internal static FontImageSource ToolbarAddIcon = new()
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Add,
        Color = (Color)Application.Current!.Resources["ToolbarIconColor"]
    };
    internal static FontImageSource ToolbarBackIcon = new()
    {
        FontFamily = "MaterialRounded",
#if IOS
        Glyph = MaterialRounded.Arrow_back_ios,
#else
        Glyph = MaterialRounded.Arrow_back,
#endif
        Color = (Color)Application.Current!.Resources["ToolbarIconColor"]
    };
    internal static FontImageSource ToolbarCasinoIcon = new()
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Casino,
        Color = (Color)Application.Current!.Resources["ToolbarIconColor"]
    };
    internal static FontImageSource ToolbarCollapseAllIcon = new()
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Collapse_all,
        Color = (Color)Application.Current!.Resources["ToolbarIconColor"]
    };
    internal static FontImageSource ToolbarClearIcon = new()
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Delete,
        Color = (Color)Application.Current!.Resources["ToolbarIconColor"]
    };
    internal static FontImageSource ToolbarCloudIcon = new()
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Cloud,
        Color = (Color)Application.Current!.Resources["ToolbarIconColor"]
    };
    internal static FontImageSource ToolbarCloudOffIcon = new()
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Cloud_off,
        Color = (Color)Application.Current!.Resources["ToolbarIconOffColor"]
    };
    internal static FontImageSource ToolbarExpandAllIcon = new()
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Expand_all,
        Color = (Color)Application.Current!.Resources["ToolbarIconColor"]
    };
    internal static FontImageSource ToolbarMoreItemsIcon = new()
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.More_vert,
        Color = (Color)Application.Current!.Resources["ToolbarIconColor"]
    };
    internal static FontImageSource ToolbarPlayIcon = new()
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Play_arrow,
        Color = (Color)Application.Current!.Resources["ToolbarIconColor"]
    };
    internal static FontImageSource ToolbarRateReviewIcon = new()
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Rate_review,
        Color = (Color)Application.Current!.Resources["ToolbarIconColor"]
    };
    internal static FontImageSource ToolbarSaveIcon = new()
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Save,
        Color = (Color)Application.Current!.Resources["ToolbarIconColor"]
    };
    internal static FontImageSource ToolbarSettingsIcon = new()
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Settings,
        Color = (Color)Application.Current!.Resources["ToolbarIconColor"]
    };
    internal static FontImageSource ToolbarSortIcon = new()
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Sort,
        Color = (Color)Application.Current!.Resources["ToolbarIconColor"]
    };
    internal static FontImageSource ToolbarVideoIcon = new()
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Videocam,
        Color = (Color)Application.Current!.Resources["ToolbarIconColor"]
    };
    internal static FontImageSource ToolbarVideoOffIcon = new()
    {
        FontFamily = "MaterialRounded",
        Glyph = MaterialRounded.Videocam_off,
        Color = (Color)Application.Current!.Resources["ToolbarIconOffColor"]
    };
#pragma warning restore S2223 // Non-constant static fields should not be visible

    /// <summary>
    /// When theme updates, update icon colors. 
    /// </summary>
    internal static void RefreshIcons()
    {
        AutoplayIcon.Color = (Color)Application.Current!.Resources["IconColor"];
        CrossIcon.Color = (Color)Application.Current!.Resources["IconColor"];
        BackIcon.Color = (Color)Application.Current!.Resources["IconColor"];
        OffIcon.Color = (Color)Application.Current!.Resources["IconOffColor"];
        PauseIcon.Color = (Color)Application.Current!.Resources["IconColor"];
        PlayIcon.Color = (Color)Application.Current!.Resources["IconColor"];
        RepeatOneIcon.Color = (Color)Application.Current!.Resources["IconColor"];
        ShuffleIcon.Color = (Color)Application.Current!.Resources["IconColor"];
        TimerOffIcon.Color = (Color)Application.Current!.Resources["IconOffColor"];
        Timer3Icon.Color = (Color)Application.Current!.Resources["IconColor"];
        Timer5Icon.Color = (Color)Application.Current!.Resources["IconColor"];
        VoiceIcon.Color = (Color)Application.Current!.Resources["IconColor"];
        VoiceOffIcon.Color = (Color)Application.Current!.Resources["IconOffColor"];
        FavoriteIcon.Color = (Color)Application.Current!.Resources["IconColor"];
        FavoritedIcon.Color = (Color)Application.Current!.Resources["FavoritedIconColor"];

        // Toolbar icons
        ToolbarAddIcon.Color = (Color)Application.Current!.Resources["ToolbarIconColor"];
        ToolbarBackIcon.Color = (Color)Application.Current!.Resources["ToolbarIconColor"];
        ToolbarCasinoIcon.Color = (Color)Application.Current!.Resources["ToolbarIconColor"];
        ToolbarCollapseAllIcon.Color = (Color)Application.Current!.Resources["ToolbarIconColor"];
        ToolbarClearIcon.Color = (Color)Application.Current!.Resources["ToolbarIconColor"];
        ToolbarCloudIcon.Color = (Color)Application.Current!.Resources["ToolbarIconColor"];
        ToolbarCloudOffIcon.Color = (Color)Application.Current!.Resources["ToolbarIconOffColor"];
        ToolbarExpandAllIcon.Color = (Color)Application.Current!.Resources["ToolbarIconColor"];
        ToolbarMoreItemsIcon.Color = (Color)Application.Current!.Resources["ToolbarIconColor"];
        ToolbarPlayIcon.Color = (Color)Application.Current!.Resources["ToolbarIconColor"];
        ToolbarRateReviewIcon.Color = (Color)Application.Current!.Resources["ToolbarIconColor"];
        ToolbarSaveIcon.Color = (Color)Application.Current!.Resources["ToolbarIconColor"];
        ToolbarSettingsIcon.Color = (Color)Application.Current!.Resources["ToolbarIconColor"];
        ToolbarSortIcon.Color = (Color)Application.Current!.Resources["ToolbarIconColor"];
        ToolbarVideoIcon.Color = (Color)Application.Current!.Resources["ToolbarIconColor"];
        ToolbarVideoOffIcon.Color = (Color)Application.Current!.Resources["ToolbarIconOffColor"];
    }
}
