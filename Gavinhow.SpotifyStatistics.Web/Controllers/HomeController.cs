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

        public IActionResult Index(string id = null)
        {
            if (HttpContext.Session.Keys.Contains(SessionVariables.PROFILE_ID))
            {
                string userId = HttpContext.Session.GetString(SessionVariables.PROFILE_ID);

                if (userId != "gavinhow" || id == null)
                {
                    id = userId;
                }
                if (_dbContext.Plays.Where(play => play.UserId == id).Count() == 0)
                {
                    return View(new HomeViewModel(_dbContext.Users.Find(id)));
                }

                return View(new HomeViewModel(_dbContext.Users.Find(id), _spotifyApi.GetOldestSong(id), _spotifyApi.GetMostPlayedSongs(id), _spotifyApi.GetTopPlayedArtist(id)));
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


    }
}
