namespace RpdPlayerApp.Architecture;

internal static class MyExtensions
{
    /// <summary> Trims whitespace </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static bool IsNullOrWhiteSpace(this string text)
    {
        return text == null || text.Trim().Length == 0;
    }
}
