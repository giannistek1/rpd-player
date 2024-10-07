﻿using CommunityToolkit.Maui.Alerts;
using RpdPlayerApp.Models;

namespace RpdPlayerApp.Architecture;

internal static class HelperClass
{
    private static Random rng = new Random();

    /// <summary>
    /// Checks whether Network has internet access. If false, shows toast saying "No internet connection!"
    /// </summary>
    /// <returns></returns>
    public static bool HasInternetConnection()
    {
        NetworkAccess accessType = Connectivity.Current.NetworkAccess;

        if (accessType != NetworkAccess.Internet)
        {
            Toast.Make($"No internet connection!", CommunityToolkit.Maui.Core.ToastDuration.Short, 14).Show();
            return false;
        }
        return true;
    }

    public static string ReadTextFile(string filePath)
    {
        try
        {
            // Ensure the file exists
            if (File.Exists(filePath))
            {
                // Read the contents of the file
                string fileContent = File.ReadAllText(filePath);
                return fileContent;
            }
            else
            {
                return "File not found.";
            }
        }
        catch (Exception ex)
        {
            // Handle any exceptions that occur during file reading
            return $"An error occurred: {ex.Message}";
        }
    }

    // Playlist functionality
    public static List<SongPart> RandomizePlaylist(List<SongPart> playlist)
    {
        for (int i = 0; i < playlist.Count; i++)
        {
            SongPart temp = playlist[i];
            int randomIndex = rng.Next(i, playlist.Count);
            playlist[i] = playlist[randomIndex];
            playlist[randomIndex] = temp;
        }

        SentrySdk.CaptureMessage($"Randomized playlist with {playlist.Count} songs");

        return playlist;
    }

    // Extra functionality
    public static List<SongPart> RandomizeAndAlternatePlaylist(List<SongPart> playlist)
    {
        SongPart previousSong = null;

        var firstGroupType = GroupType.GG;

        int bgCount = playlist.AsEnumerable().Count(s => s.Artist?.GroupType == GroupType.BG);
        int ggCount = playlist.AsEnumerable().Count(s => s.Artist?.GroupType == GroupType.GG);
        int mixCount = playlist.AsEnumerable().Count(s => s.Artist?.GroupType == GroupType.MIX);
        int nsCount = playlist.AsEnumerable().Count(s => s.Artist?.GroupType == GroupType.NOT_SET);

        if (bgCount > ggCount)
        {
            firstGroupType = GroupType.BG;
        }
        if (mixCount > bgCount)
        {
            firstGroupType = GroupType.MIX;
        }

        List<int> groupTypesCount =
        [
            bgCount,
            ggCount,
            mixCount,
            nsCount,
        ];

        // You can do the same with artists
        /*List<int> artistsCount = new List<int>();
		// GroupBy basically groups everything that's the same into a list
		var g = playlistSongs.GroupBy( i => i.artist );
		foreach (var grp in g)
		{
			artistsCount.Add(grp.Count());
		}*/

        for (int i = 0; i < playlist.Count; i++)
        {
            SongPart temp = playlist[i];  

            // Select random song
            int randomIndex = rng.Next(i, playlist.Count);

            int depletedGrouptypes = 0;
            // Check for depleted groupTypes
            foreach (int groupTypeCount in groupTypesCount)
            {
                if (groupTypeCount == 0)
                {
                    depletedGrouptypes++;
                }
            }

            // If random song groupType is same as previous, get new randomIndex.	
            if (previousSong is not null)
            {
                while (playlist[randomIndex].Artist?.GroupType == previousSong.Artist?.GroupType && depletedGrouptypes < 3)
                {
                    randomIndex = rng.Next(i, playlist.Count);
                }
            }
            else
            {
                // Choose random index while grouptype is different than first grouptype.
                while (playlist[randomIndex].Artist?.GroupType != firstGroupType && depletedGrouptypes < 3)
                {
                    randomIndex = rng.Next(i, playlist.Count);
                }
            }

            // Substract from count based on groupType.
            groupTypesCount[(int)playlist[randomIndex].Artist.GroupType]--;

            // Set previous song and put song in playlist.
            previousSong = playlist[randomIndex];
            playlist[i] = playlist[randomIndex];

            // Swap the two songs.
            playlist[randomIndex] = temp;
        }

        //SentrySdk.CaptureMessage($"Randomized alternating playlist with {playlist.Count} songs");

        return playlist;
    }
}
