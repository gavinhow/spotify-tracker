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
using System.Collections.Generic;

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
            if (HttpContext.Session.Keys.Contains(SessionVariables.PROFILE_ID))
            {
                string userId = HttpContext.Session.GetString(SessionVariables.PROFILE_ID);
                if (_dbContext.Plays.Where(play => play.UserId == userId).Count() == 0)
                {
                    return View(new HomeViewModel(_dbContext.Users.Find(userId)));
                }
                return View(new HomeViewModel(_dbContext.Users.Find(userId), GetOldestSong(userId), GetMostPlayedSongs(userId)));
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

        private SongPlayCount GetMostPlayedSong(string userId)
        {
            var mostplayedsong = _dbContext.Plays
                        .Where(play => play.UserId == userId)
                        .GroupBy(play => play.TrackId)
                        .OrderByDescending(gp => gp.Count()).First();

            return new SongPlayCount { track = _spotifyApi.GetTrack(mostplayedsong.Key), plays = mostplayedsong.Count() };
        }

        private Play GetOldestSong(string userId)
        {
            return _dbContext.Plays
                        .Where(play => play.UserId == userId)
                        .OrderBy(play => play.TimeOfPlay).First();
        }

        private List<SongPlayCount> GetMostPlayedSongs(string userId, int numOfSongs = 10)
        {
            return _dbContext.Plays
                        .Where(play => play.UserId == userId)
                        .GroupBy(play => play.TrackId)
                        .OrderByDescending(gp => gp.Count())
                        .Take(numOfSongs)
                        .Select(song => new SongPlayCount { track = _spotifyApi.GetTrack(song.Key), plays = song.Count() }).ToList();
        }
    }
}
