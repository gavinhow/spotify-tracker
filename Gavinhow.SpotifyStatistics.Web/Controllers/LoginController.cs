using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gavinhow.SpotifyStatistics.Database;
using Gavinhow.SpotifyStatistics.Database.Entity;
using Gavinhow.SpotifyStatistics.Web.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Gavinhow.SpotifyStatistics.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly SpotifySettings _spotifySettings;

        private readonly SpotifyStatisticsContext _dbContext;

        public LoginController(SpotifyStatisticsContext dbContext, IOptions<SpotifySettings> spotifySettings)
        {
            _dbContext = dbContext;
            _spotifySettings = spotifySettings.Value;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login()
        {
            AuthorizationCodeAuth auth =
                    new AuthorizationCodeAuth(_spotifySettings.ClientId, _spotifySettings.ClientSecret, 
                _spotifySettings.CallbackUri, _spotifySettings.ServerUri,
                        Scope.UserReadRecentlyPlayed);

            return Redirect(auth.GetUri());
        }

        [HttpGet]
        public async Task<IActionResult> Authorise(string code)
        {
            AuthorizationCodeAuth auth = new AuthorizationCodeAuth(_spotifySettings.ClientId, _spotifySettings.ClientSecret,
                _spotifySettings.CallbackUri, _spotifySettings.ServerUri,
                        Scope.UserReadPrivate);

            Token token = await auth.ExchangeCode(code);

            SpotifyWebAPI api = new SpotifyWebAPI
            {
                AccessToken = token.AccessToken,
                TokenType = token.TokenType,
            };

            PrivateProfile profile = await api.GetPrivateProfileAsync();

            User existingUser = _dbContext.Users.Where(u => u.Id.Equals(profile.Id)).FirstOrDefault();

            if ( existingUser!=null )
            {
                existingUser.AccessToken = token.AccessToken;
                existingUser.RefreshToken = token.RefreshToken;
                existingUser.TokenCreateDate = token.CreateDate;
                existingUser.ExpiresIn = token.ExpiresIn;

                _dbContext.Users.Update(existingUser);
            }
            else
            {
                User user = new User
                {
                    Id = profile.Id,
                    AccessToken = token.AccessToken,
                    RefreshToken = token.RefreshToken,
                    ExpiresIn = token.ExpiresIn,
                    TokenCreateDate = token.CreateDate
                };
                _dbContext.Users.Add(user);
            }
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}
