namespace RpdPlayerApp.Architecture;

internal class ImportResult
{
    public string Artist { get; set; }
    public string Title { get; set; }

    public ImportResult(string artist, string title)
    {
        Artist = artist;
        Title = title;
    }
}
