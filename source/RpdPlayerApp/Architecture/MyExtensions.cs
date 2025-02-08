namespace RpdPlayerApp.Architecture;

internal static class MyExtensions
{
    public static bool IsNullOrEmpty(this String text)
    {
        return text == null || text.Trim().Length == 0;
    }
}
