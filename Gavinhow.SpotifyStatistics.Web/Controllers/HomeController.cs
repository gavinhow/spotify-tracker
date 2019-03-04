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
        private readonly SpotifyApiFacade _spotifyApiFacade;

        public HomeController(SpotifyStatisticsContext dbContext, SpotifyApiFacade spotifyApiFacade)
        {
            _dbContext = dbContext;
            _spotifyApiFacade = spotifyApiFacade;
        }

        public IActionResult Index(string id = null)
        {
            if (!HttpContext.Session.Keys.Contains(SessionVariables.PROFILE_ID))
                return RedirectToAction("Index", "Login");
            
            string userId = HttpContext.Session.GetString(SessionVariables.PROFILE_ID);

            if (userId != "gavinhow" || id == null)
            {
                id = userId;
            }
            if (!_dbContext.Plays.Any(play => play.UserId == id))
            {
                return View(new HomeViewModel(_dbContext.Users.Find(id)));
            }

            var today = DateTime.Today;
            var month = new DateTime(today.Year, today.Month, 1);
            var first = month.AddMonths(-1);
            var last = month.AddDays(-1);
            return View(new HomeViewModel(_dbContext.Users.Find(id),
                _spotifyApiFacade.GetOldestSong(id),
                _spotifyApiFacade.GetMostPlayedSongs(id),
                _spotifyApiFacade.GetTopPlayedArtist(id),
                _spotifyApiFacade.GetMostPlayedSongs(id,
                    first,
                    last),
                _spotifyApiFacade.GetSongCount(id, first, last),
                _spotifyApiFacade.GetSongCount(id)));
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


    }
}
