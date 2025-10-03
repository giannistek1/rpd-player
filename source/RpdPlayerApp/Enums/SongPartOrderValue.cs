namespace RpdPlayerApp.Enums;

/// <summary>
/// Usually ordered by what it starts with.
/// Examples: Prechorus, Chorus, Dancebreak, Tiktok
/// </summary>
internal enum SongPartOrderValue
{
    Unspecified = 0,
    Prechorus = 1,
    Chorus = 2,
    Dancebreak = 3, // Includes bridges
    Tiktok = 4,
    Verse = 5,
    Intro = 6,
    Ending = 7 // Includes outro
}
