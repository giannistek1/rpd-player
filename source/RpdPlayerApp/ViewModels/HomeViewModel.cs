using CommunityToolkit.Mvvm.ComponentModel;
using RpdPlayerApp.Models;
using RpdPlayerApp.Repositories;
using System.Collections.ObjectModel;

namespace RpdPlayerApp.ViewModels;

internal partial class HomeViewModel : ObservableObject
{
    public ObservableCollection<Artist> Artists => ArtistRepository.Artists;
    public ObservableCollection<Album> Albums => AlbumRepository.Albums;
    public ObservableCollection<SongPart> SongParts => SongPartRepository.SongParts;
}
