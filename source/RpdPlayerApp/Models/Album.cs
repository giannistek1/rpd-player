﻿using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using RpdPlayerApp.Architecture;

namespace RpdPlayerApp.Models;

internal partial class Album : ObservableObject
{
    public int Id { get; set; }

    [ObservableProperty]
    private string _artistName;

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private DateTime _releaseDate;

    // TODO: Change KR, CH, TH, EN etc to Genre
    /// <summary> KR, CH, TH, EN, krnb etc... No enum because there is short name and long name. </summary>
    [ObservableProperty]
    private string _genreShort;

    /// <summary> K-pop, J-pop, C-pop, T-pop, Pop, K-RnB etc... </summary>
    [ObservableProperty]
    private string _genreFull;

    [ObservableProperty]
    private string _imageURL;

    [JsonIgnore]
    [ObservableProperty]
    private bool _showAlbumTitle = false;

    [JsonIgnore]
    [ObservableProperty]
    private bool _showAlbumReleaseDate = false;

    [JsonIgnore]
    [ObservableProperty]
    private bool _showGenreShort = false;

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
            Constants.GenreKpop => "K-pop",
            Constants.GenreKrnB => "K-RnB",
            Constants.GenreJpop => "J-pop",
            Constants.GenreCpop => "C-pop",
            Constants.GenrePop => "Pop",
            Constants.GenreTpop => "T-pop",
            Constants.GenreNlPop => "NL-pop",

            _ => "Unknown genre"
        };
    }
}