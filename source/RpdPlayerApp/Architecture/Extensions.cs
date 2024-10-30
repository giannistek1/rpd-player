namespace RpdPlayerApp.Architecture;

internal static class Extensions
{
    private static Random rng = new Random();

    public static bool IsNullOrBlank(this String text)
    {
        return text == null || text.Trim().Length == 0;
    }
}
