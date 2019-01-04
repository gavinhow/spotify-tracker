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
        public readonly Play _oldestSong;
        public readonly MostPlayedSong _mostPlayedSong;

        public HomeViewModel(User user,Play oldestSong, MostPlayedSong mostPlayedSong)
        {
            CurrentUser = user;
            _oldestSong = oldestSong;
            _mostPlayedSong = mostPlayedSong;
        }
    }
}
