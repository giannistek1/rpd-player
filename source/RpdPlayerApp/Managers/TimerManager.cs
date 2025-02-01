using RpdPlayerApp.Models;

namespace RpdPlayerApp.Managers;

internal static class TimerManager
{
    private static System.Timers.Timer? timer;
#pragma warning disable S2223 // Non-constant static fields should not be visible
    internal static SongPart? songPart;
#pragma warning restore S2223
    internal static void StartInfiniteScaleYAnimationWithTimer()
    {
        if (timer is not null) { timer.Close(); timer.Dispose(); }

        double startScaleY = 1;
        double endScaleY = 2;
        int duration = 1000; // 1 second for a full cycle (up and down)
        int interval = 16; // ~60 FPS
        int steps = duration / interval / 2; // Half the steps for up, half for down
        double stepChange = (endScaleY - startScaleY) / steps;

        bool scalingUp1 = true; // Track the direction of scaling
        bool scalingUp2 = true; // Track the direction of scaling
        bool scalingUp3 = true; // Track the direction of scaling

        // Create and start the timer
        timer = new System.Timers.Timer(interval);
        timer.Elapsed += (s, e) =>
        {
            if (!songPart!.IsPlaying) { timer.Close(); }

            if (scalingUp1)
            {
                songPart.PlayingIconScaleY1 += stepChange;
                // Adjust TranslationY to keep the bottom edge anchored
                songPart.PlayingIconTranslationY1 = (1 - songPart.PlayingIconScaleY1) * 12 / 2;
                if (songPart.PlayingIconScaleY1 >= endScaleY)
                {
                    scalingUp1 = false;
                }
            }
            else
            {
                songPart.PlayingIconScaleY1 -= stepChange;
                // Adjust TranslationY to keep the bottom edge anchored
                songPart.PlayingIconTranslationY1 = (1 - songPart.PlayingIconScaleY1) * 12 / 2;
                if (songPart.PlayingIconScaleY1 <= startScaleY)
                {
                    scalingUp1 = true;
                }
            }

            if (scalingUp2)
            {
                songPart.PlayingIconScaleY2 += stepChange;
                // Adjust TranslationY to keep the bottom edge anchored
                songPart.PlayingIconTranslationY2 = (1 - songPart.PlayingIconScaleY2) * 12 / 2;
                if (songPart.PlayingIconScaleY2 >= endScaleY)
                {
                    scalingUp2 = false;
                }
            }
            else
            {
                songPart.PlayingIconScaleY2 -= stepChange;
                // Adjust TranslationY to keep the bottom edge anchored
                songPart.PlayingIconTranslationY2 = (1 - songPart.PlayingIconScaleY2) * 12 / 2;
                if (songPart.PlayingIconScaleY2 <= startScaleY)
                {
                    scalingUp2 = true;
                }
            }

            if (scalingUp3)
            {
                songPart.PlayingIconScaleY3 += stepChange;
                // Adjust TranslationY to keep the bottom edge anchored
                songPart.PlayingIconTranslationY3 = (1 - songPart.PlayingIconScaleY3) * 12 / 2;
                if (songPart.PlayingIconScaleY3 >= endScaleY)
                {
                    scalingUp3 = false;
                }
            }
            else
            {
                songPart.PlayingIconScaleY3 -= stepChange;
                // Adjust TranslationY to keep the bottom edge anchored
                songPart.PlayingIconTranslationY3 = (1 - songPart.PlayingIconScaleY3) * 12 / 2;
                if (songPart.PlayingIconScaleY3 <= startScaleY)
                {
                    scalingUp3 = true;
                }
            }
        };

        timer.Start();
    }
}