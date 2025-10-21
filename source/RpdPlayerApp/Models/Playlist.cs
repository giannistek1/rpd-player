using CommunityToolkit.Mvvm.ComponentModel;
using RpdPlayerApp.Enums;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace RpdPlayerApp.Models;

internal partial class Playlist : ObservableObject
{
    public long Id { get; set; } = -1L;

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

    public Playlist(DateTime creationDate, DateTime lastModifiedDate, string name = "", string path = "", int count = 0)
    {
        CreationDate = creationDate;
        LastModifiedDate = lastModifiedDate;
        Name = name;
        LocalPath = path;

        // Because the computed properties don't know when SongParts has changes, need to subscribe to changes and get notified.
        Segments.CollectionChanged += SongPartsCollectionChanged;
    }

    private void SongPartsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(Count));
        OnPropertyChanged(nameof(Length));
        OnPropertyChanged(nameof(LengthInSeconds));
    }
}
