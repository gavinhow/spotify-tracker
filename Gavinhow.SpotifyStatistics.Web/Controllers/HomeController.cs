using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Gavinhow.SpotifyStatistics.Web.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using Gavinhow.SpotifyStatistics.Database;

namespace Gavinhow.SpotifyStatistics.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly SpotifyStatisticsContext _dbContext;

        public HomeController(SpotifyStatisticsContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.Keys.Contains("username"))
            {
                return View(new HomePageModel { CurrentUser = _dbContext.Users.Find(HttpContext.Session.GetString("username")) });
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
