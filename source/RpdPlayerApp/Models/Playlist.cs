using CommunityToolkit.Mvvm.ComponentModel;
using RpdPlayerApp.Enums;
using RpdPlayerApp.Services;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace RpdPlayerApp.Models;

internal partial class Playlist : ObservableObject
{
    /// <summary> Only for cloud updating/deleting. </summary>
    public long Id { get; set; }

    [ObservableProperty]
    private string _name = string.Empty;

    // Handy local variable
    public string LocalPath { get; set; }

    [ObservableProperty]
    public DateTime _creationDate;

    [ObservableProperty]
    public DateTime _lastModifiedDate = DateTime.MinValue;

    [ObservableProperty]
    public CountdownModeValue _countdownMode = CountdownModeValue.Off; // TODO: Implement

    [ObservableProperty]
    public bool _isCloudPlaylist = false;

    [ObservableProperty]
    public bool _isPublic = false;

    [ObservableProperty]
    public string _owner = string.Empty;

    public ObservableCollection<SongPart> Segments = [];

    // Computed properties
    public int Count => Segments.Count;
    public TimeSpan Length => TimeSpan.FromSeconds(Segments.Sum(t => t.ClipLength));
    public int LengthInSeconds => (int)Segments.Sum(t => t.ClipLength);

    public Playlist(DateTime creationDate, DateTime lastModifiedDate, string name = "", string path = "", string owner = "")
    {
        CreationDate = creationDate;
        LastModifiedDate = lastModifiedDate;
        Name = name;
        LocalPath = path;
        Owner = owner;

        // Because the computed properties don't know when SongParts has changes, need to subscribe to changes and get notified.
        // Optionally we can put this in the Set of the property. But that makes it more complex.
        Segments.CollectionChanged += SongPartsCollectionChanged;
    }

    private void SongPartsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(Count));
        OnPropertyChanged(nameof(Length));
        OnPropertyChanged(nameof(LengthInSeconds));
    }
}
