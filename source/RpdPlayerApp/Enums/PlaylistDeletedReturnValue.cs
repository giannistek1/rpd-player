
namespace RpdPlayerApp.Enums;

internal enum PlaylistDeletedReturnValue
{
    CantDeletePublicPlaylist = -3,
    FailedToDelete = -1,
    DeletedLocally = 1,
    DeletedFromCloud = 2,
}
