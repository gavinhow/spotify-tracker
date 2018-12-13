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
        private readonly string _clientId;
        private readonly string _secretId;

        private readonly SpotifyStatisticsContext _dbContext;

        public LoginController(SpotifyStatisticsContext dbContext, IOptions<SpotifySettings> spotifySettings)
        {
            _dbContext = dbContext;
            _clientId = spotifySettings.Value.ClientId;
            _secretId = spotifySettings.Value.ClientSecret;
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
                    new AuthorizationCodeAuth(_clientId, _secretId, "https://localhost:5001/Login/Authorise", "https://localhost:5001",
                        Scope.UserReadCurrentlyPlaying | Scope.UserReadRecentlyPlayed);

            return Redirect(auth.GetUri());
        }

        [HttpGet]
        public async Task<string> Authorise(string code)
        {
            AuthorizationCodeAuth auth = new AuthorizationCodeAuth(_clientId, _secretId, "https://localhost:5001/Login/Authorise", "https://localhost:5001",
                        Scope.UserReadCurrentlyPlaying | Scope.UserReadRecentlyPlayed);

            Token token = await auth.ExchangeCode(code);

            SpotifyWebAPI api = new SpotifyWebAPI
            {
                AccessToken = token.AccessToken,
                TokenType = token.TokenType,
            };

            PrivateProfile profile = await api.GetPrivateProfileAsync();

            User user = new User
            {
                UserName = profile.Id,
                AccessToken = token.AccessToken,
                RefreshToken = token.RefreshToken,
                ExpiresIn = token.ExpiresIn,
                TokenCreateDate = token.CreateDate
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return user.UserName;
        }
    }
}
