using RpdPlayerApp.Models;

namespace RpdPlayerApp.Architecture;

internal class MyEventArgs : EventArgs
{
    internal SongPart SongPart { get; set; }

    internal MyEventArgs(SongPart songPart)
    {
        SongPart = songPart;
    }
}
