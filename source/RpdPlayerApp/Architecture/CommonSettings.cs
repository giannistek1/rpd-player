using System.Diagnostics;

namespace RpdPlayerApp.Architecture;

internal static class CommonSettings
{
    internal const string MAIN_VOLUME = "MAIN_VOLUME";
    internal const double DEFAULT_MAIN_VOLUME = 0.8;
    internal const string USE_ANALYTICS = "USE_ANALYTICS"; // Not used
    internal const string START_RPD_AUTOMATIC = "START_RPD_AUTOMATIC";
    internal const string USE_NONCHOREO_SONGS = "USE_NONCHOREO_SONGS";
    internal const string TOTAL_ACTIVITY_TIME = "TOTAL_ACTIVITY_TIME";

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

    internal static void RecalculateTotalActivityTime() => TotalActivityTime = TotalActivityTime.Add(ActivityTimeStopWatch.Elapsed);
}