using System;
using System.Linq;
using Gavinhow.SpotifyStatistics.Database;
using Gavinhow.SpotifyStatistics.Database.Entity;

namespace Gavinhow.SpotifyStatistics.Web.Models
{
    public class HomeViewModel
    {
        private readonly SpotifyStatisticsContext _dbContext;

        public User CurrentUser { get; set; }
        public readonly Play oldestSong;
        public readonly MostPlayedSong mostPlayedSong;

        public HomeViewModel(SpotifyStatisticsContext dbContext, User user)
        {
            _dbContext = dbContext;
            CurrentUser = user;
            oldestSong = GetOldestSong();
            mostPlayedSong = GetMostPlayedSong();
        }


        public bool IsCurrentUserLoggedIn => (CurrentUser != null);

        private Play GetOldestSong()
        {
            return _dbContext.Plays
                        .Where(play => play.UserId == CurrentUser.Id)
                        .OrderBy(play => play.TimeOfPlay).First();
        }

        private MostPlayedSong GetMostPlayedSong()
        {
            var mostplayedsong = _dbContext.Plays
                        .Where(play => play.UserId == CurrentUser.Id)
                        .GroupBy(play => play.TrackId)
                        .OrderByDescending(gp => gp.Count()).First();

            return new MostPlayedSong { trackId = mostplayedsong.Key, plays = mostplayedsong.Count() };
        }

        public class MostPlayedSong
        {
            public string trackId;
            public int plays;
        }
    }
}
