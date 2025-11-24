namespace RpdPlayerApp.Enums;


public enum AutoplayModeValue
{
    /// <summary> Only play one song. </summary>
    Single = 0,
    /// <summary> Loops through whatever songlist. </summary>
    AutoplayLoop = 1,
    Shuffle = 2,
    RepeatOne = 3,
    // Not used: Regular play, song by song.
    //Autoplay = 4,
}