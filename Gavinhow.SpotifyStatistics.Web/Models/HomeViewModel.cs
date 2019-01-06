using System.Collections.Generic;
using System.Linq;
using Gavinhow.SpotifyStatistics.Database;
using Gavinhow.SpotifyStatistics.Database.Entity;
using Microsoft.AspNetCore.Http;
using static Gavinhow.SpotifyStatistics.Web.Controllers.HomeController;

namespace Gavinhow.SpotifyStatistics.Web.Models
{
    public class HomeViewModel
    {
        public User CurrentUser { get; set; }
        public readonly bool hasPlays;
        public readonly Play _oldestSong;
        public readonly List<SongPlayCount> _mostPlayedSongs;

        public HomeViewModel(User user)
        {
            CurrentUser = user;
            hasPlays = false;
        }

        public HomeViewModel(User user, Play oldestSong, List<SongPlayCount> mostPlayedSongs)
        {
            CurrentUser = user;
            _oldestSong = oldestSong;
            _mostPlayedSongs = mostPlayedSongs;

            hasPlays = (oldestSong != null);
        }
    }
}
