using System.Collections.Generic;
using System.Linq;
using Gavinhow.SpotifyStatistics.Api.Models;
using Gavinhow.SpotifyStatistics.Database;
using Gavinhow.SpotifyStatistics.Database.Entity;
using Microsoft.AspNetCore.Http;
using SpotifyAPI.Web.Models;
using static Gavinhow.SpotifyStatistics.Web.Controllers.HomeController;

namespace Gavinhow.SpotifyStatistics.Web.Models
{
    public class HomeViewModel
    {
        public User CurrentUser { get; set; }
        public readonly bool hasPlays;
        public readonly Play _oldestSong;
        public readonly List<SongPlayCount> _mostPlayedSongs;
        public readonly List<SongPlayCount> _lastMonthMostPlayedSongs;
        public readonly List<ArtistPlayCount> _mostPlayedArtists;

        public HomeViewModel(User user)
        {
            CurrentUser = user;
            hasPlays = false;
        }

        public HomeViewModel(User user, Play oldestSong, List<SongPlayCount> mostPlayedSongs, List<ArtistPlayCount> mostPlayedArtists, List<SongPlayCount> lastMonthPlayed)
        {
            CurrentUser = user;
            _oldestSong = oldestSong;
            _mostPlayedSongs = mostPlayedSongs;
            _mostPlayedArtists = mostPlayedArtists;
            _lastMonthMostPlayedSongs = lastMonthPlayed;

            hasPlays = (oldestSong != null);
        }
    }
}
