﻿using Newtonsoft.Json;
using RpdPlayerApp.ViewModels;

namespace RpdPlayerApp.Models;

internal class Album
{
    public int Id { get; set; }
    public string ArtistName { get; set; }
    public string Title { get; set; }
    public DateTime ReleaseDate { get; set; }

    // TODO: Change KR, CH, TH, EN etc to Genre
    /// <summary> KR, CH, TH, EN, krnb etc... No enum because there is short name and long name. </summary>
    public string GenreShort { get; set; }

    /// <summary> K-pop, J-pop, C-pop, T-pop, Pop, K-RnB etc... </summary>
    public string GenreFull { get; set; }

    public string ImageURL { get; set; }

    [JsonIgnore]
    public bool ShowAlbumTitle { get; set; } = false;

    [JsonIgnore]
    public bool ShowAlbumReleaseDate { get; set; } = false;

    [JsonIgnore]
    public bool ShowGenreShort { get; set; } = false;

    public Album(int id = -1, string artistName = "", DateTime releaseDate = new(), string title = "", string genreShort = "", string imageURL = "")
    {
        Id = id;
        ArtistName = artistName;
        ReleaseDate = releaseDate;
        Title = title;
        GenreShort = genreShort;
        GenreFull = GetGenre();
        ImageURL = imageURL;
    }

    private string GetGenre()
    {
        return GenreShort switch
        {
            MainViewModel.GenreKpop => "K-pop",
            "krnb" => "K-RnB",
            "JP" => "J-pop",
            "CH" => "C-pop",
            "EN" => "Pop",
            "TH" => "T-pop",
            "NL" => "NL-pop",

            _ => "Unknown genre"
        };
    }
}