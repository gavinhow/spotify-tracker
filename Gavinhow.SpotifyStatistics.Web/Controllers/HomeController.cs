using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Gavinhow.SpotifyStatistics.Web.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using Gavinhow.SpotifyStatistics.Database;
using SpotifyAPI.Web.Models;
using Gavinhow.SpotifyStatistics.Api;
using Gavinhow.SpotifyStatistics.Database.Entity;

namespace Gavinhow.SpotifyStatistics.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly SpotifyStatisticsContext _dbContext;
        private readonly SpotifyApi _spotifyApi;

        public HomeController(SpotifyStatisticsContext dbContext, SpotifyApi spotifyApi)
        {
            _dbContext = dbContext;
            _spotifyApi = spotifyApi;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.Keys.Contains("username"))
            {
                string userId = HttpContext.Session.GetString("username");
                return View(new HomeViewModel(_dbContext.Users.Find(userId) ,GetOldestSong(userId), GetMostPlayedSong(userId)));
            }
            return RedirectToAction("Index", "Login");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private MostPlayedSong GetMostPlayedSong(string userId)
        {
            var mostplayedsong = _dbContext.Plays
                        .Where(play => play.UserId == userId)
                        .GroupBy(play => play.TrackId)
                        .OrderByDescending(gp => gp.Count()).First();

            return new MostPlayedSong { track = _spotifyApi.GetTrack(mostplayedsong.Key), plays = mostplayedsong.Count() };
        }

        private Play GetOldestSong(string userId)
        {
            return _dbContext.Plays
                        .Where(play => play.UserId == userId)
                        .OrderBy(play => play.TimeOfPlay).First();
        }

        public class MostPlayedSong
        {
            public FullTrack track;
            public int plays;
        }
    }
}
