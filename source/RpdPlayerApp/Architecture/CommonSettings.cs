using System.Diagnostics;

namespace RpdPlayerApp.Architecture;

internal static class CommonSettings
{
    internal const string MAIN_VOLUME = "MAIN_VOLUME";
    internal const string USE_ANALYTICS = "USE_ANALYTICS";
    internal const string START_RPD_AUTOMATIC = "START_RPD_AUTOMATIC";
    internal const string USE_NONCHOREO_SONGS = "USE_NONCHOREO_SONGS";
    internal const string TOTAL_ACTIVITY_TIME = "TOTAL_ACTIVITY_TIME";

    /// <summary> 0.0 - 1.0 as Double </summary>
#pragma warning disable S2223 // Non-constant static fields should not be visible
    internal static double MainVolume = 1.0;
#pragma warning restore S2223
    internal static bool IsVolumeMuted = false;
    internal static bool ShouldLoopVideo { get; set; } = true;

    // TODO: StatsClass
    internal static Stopwatch ActivityTimeStopWatch { get; set; } = new Stopwatch();
    internal static TimeSpan TotalActivityTime { get; set; }

    internal static void RecalculateTotalActivityTime()
    {
        TotalActivityTime = TotalActivityTime.Add(ActivityTimeStopWatch.Elapsed);
    }
}