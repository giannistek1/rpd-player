namespace RpdPlayerApp.Models;

internal class Album
{
    public string Name { get; set; }
    public DateTime Date { get; set; }
    public string ImageURL { get; set; }

    public Album(string name, DateTime date, string imageURL = "")
    {
        Name = name;
        Date = date;
        ImageURL = imageURL;
    }
}
