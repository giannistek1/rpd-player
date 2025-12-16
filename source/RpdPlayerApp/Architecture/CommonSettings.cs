using System.Diagnostics;

namespace RpdPlayerApp.Architecture;

/// <summary> Secretly shared constants static class. </summary>
internal static class CommonSettings
{
    // General string constants.
    internal const string MAIN_VOLUME = "MAIN_VOLUME";
    internal const string USERNAME = "USERNAME";
    internal const string USE_ANALYTICS = "USE_ANALYTICS"; // Not used
    internal const string START_RPD_AUTOMATIC = "START_RPD_AUTOMATIC";
    internal const string USE_NONCHOREO_SONGS = "USE_NONCHOREO_SONGS";
    internal const string USE_TAB_ANIMATION = "USE_TAB_ANIMATION";
    internal const string DEBUG_MODE = "DEBUG_MODE";
    internal const string ONBOARDING_COMPLETED = "ONBOARDING_COMPLETED";

    internal const string TOTAL_ACTIVITY_TIME = "TOTAL_ACTIVITY_TIME";

    // Home view rpd string constants.
    internal const string HOME_DURATION = "HOME_DURATION";
    internal const string HOME_TIMER = "HOME_TIMER";
    internal const string HOME_VOICES = "HOME_VOICES";
    internal const string HOME_GROUPTYPES = "HOME_GROUPTYPES";
    internal const string HOME_GENRES = "HOME_GENRES";
    internal const string HOME_GENS = "HOME_GENS";
    internal const string HOME_COMPANIES = "HOME_COMPANIES";
    internal const string HOME_YEARS = "HOME_YEARS";
    internal const string HOME_ANTI_OPTIONS = "HOME_ANTI_OPTIONS";

    internal const double DEFAULT_MAIN_VOLUME = 0.8;
    // General settings.
    /// <summary> 0.0 - 1.0 as Double. </summary>
    /// <remarks> Default: 0.8 </remarks>
#pragma warning disable S2223 // Non-constant static fields should not be visible
    internal static double MainVolume = DEFAULT_MAIN_VOLUME;
#pragma warning restore S2223
    internal static bool IsVolumeMuted = false;
    internal static bool ShouldLoopVideo { get; set; } = true;

    // TODO: StatsClass
    internal static Stopwatch ActivityTimeStopWatch { get; set; } = new();
    internal static TimeSpan TotalActivityTime { get; set; }

    internal static void RecalculateTotalActivityTime()
    {
        TotalActivityTime = TotalActivityTime.Add(ActivityTimeStopWatch.Elapsed);
        ActivityTimeStopWatch.Reset();
        ActivityTimeStopWatch.Start();
    }
}