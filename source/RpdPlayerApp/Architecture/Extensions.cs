﻿namespace RpdPlayerApp.Architecture;

internal static class Extensions
{
    private static Random rng = new Random();

    public static bool IsNullOrBlank(this String text)
    {
        return text == null || text.Trim().Length == 0;
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
