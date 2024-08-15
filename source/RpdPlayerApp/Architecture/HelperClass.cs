﻿using CommunityToolkit.Maui.Alerts;
using RpdPlayerApp.Models;

namespace RpdPlayerApp.Architecture;

internal static class HelperClass
{
    internal static readonly DateTime secondGenStartDate = new DateTime(2002, 12, 31);
    internal static readonly DateTime thirdGenStartDate = new DateTime(2012, 1, 1);
    internal static readonly DateTime fourthGenStartDate = new DateTime(2018, 1, 1);
    internal static readonly DateTime fifthGenStartDate = new DateTime(2023, 1, 1);


    private static Random rng = new Random();

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
        List<int> groupTypesCount = new List<int>();

        for (int i = 0; i < playlist.Count; i++)
        {
            SongPart temp = playlist[i];

            // Select random song
            int randomIndex = rng.Next(i, playlist.Count);

            int emptyGroups = 0;
            // Check for empty groupTypes
            foreach (int groupTypeCount in groupTypesCount)
            {
                if (groupTypeCount == 0)
                {
                    emptyGroups++;
                }
            }

            // If random song groupType is same as previous, get new randomIndex	
            if (previousSong != null)
            {
                while (playlist[randomIndex].Artist.GroupType == previousSong.Artist.GroupType && emptyGroups < 3)
                {
                    randomIndex = rng.Next(i, playlist.Count);
                }
            }

            // Substract from count based on groupType
            groupTypesCount[(int)playlist[randomIndex].Artist.GroupType]--;

            // Set previous song and put song in playlist
            previousSong = playlist[randomIndex];
            playlist[i] = playlist[randomIndex];

            // Swap the two songs
            playlist[randomIndex] = temp;
        }

        SentrySdk.CaptureMessage($"Randomized alternating playlist with {playlist.Count} songs");

        return playlist;
    }
}
