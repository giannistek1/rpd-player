using CommunityToolkit.Mvvm.ComponentModel;

namespace RpdPlayerApp.Models;

internal partial class Video : ObservableObject
{
    public int Id { get; set; }

    [ObservableProperty]
    public string _title;

    [ObservableProperty]
    public string _artistName;

    /// <summary> PartNameShort (P, C, D, DB, O, T, etc) </summary>
    [ObservableProperty]
    public string _partNameShort;

    [ObservableProperty]
    public string _partNameNumber;

    [ObservableProperty]
    public string _albumTitle;

    public Video(int id, string artistName, string albumTitle, string title, string partNameShort, string partNameNumber)
    {
        Id = id;
        Title = title;
        ArtistName = artistName;

        // Somehow make part name a two parter? like if CDB 3 = Chorus 3 & Dance Break
        PartNameShort = partNameShort; // C
        PartNameNumber = partNameNumber; // 1

        AlbumTitle = albumTitle;
    }
}