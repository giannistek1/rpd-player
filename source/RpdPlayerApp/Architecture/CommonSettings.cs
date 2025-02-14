namespace RpdPlayerApp.Architecture;

internal static class CommonSettings
{
    internal const string MAIN_VOLUME = "MAIN_VOLUME";
    internal const string USE_ANALYTICS = "USE_ANALYTICS";
    internal const string START_RPD_AUTOMATIC = "START_RPD_AUTOMATIC";
    internal const string USE_NONCHOREO_SONGS = "USE_NONCHOREO_SONGS";

    /// <summary> 0.0 - 1.0 as Double </summary>
#pragma warning disable S2223 // Non-constant static fields should not be visible
    internal static double MainVolume = 1.0;
#pragma warning restore S2223
    internal static bool ShouldLoopVideo { get; set; } = true;
}