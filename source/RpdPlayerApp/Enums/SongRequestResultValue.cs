namespace RpdPlayerApp.Enums;

internal enum SongRequestResultValue
{
    ApiKeyMissing = -3,
    Cooldown = -2,
    Error = -1,
    Success = 0,
    Failure = 1,
    Duplicate = 2
}
