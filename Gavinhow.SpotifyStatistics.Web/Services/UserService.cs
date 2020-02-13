using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gavinhow.SpotifyStatistics.Api.Settings;
using Gavinhow.SpotifyStatistics.Database;
using Gavinhow.SpotifyStatistics.Database.Entity;
using Microsoft.Extensions.Options;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;

namespace Gavinhow.SpotifyStatistics.Web.Services
{
    public interface IUserService
    {
        Task<User> Authenticate(string code);
        User GetById(int id);
    }
    
    public class UserService : IUserService
    {
        private readonly SpotifyStatisticsContext _dbContext;
        private readonly SpotifySettings _spotifySettings;

        public UserService(IOptions<SpotifySettings> spotifySettings, SpotifyStatisticsContext dbContext)
        {
            _dbContext = dbContext;
            _spotifySettings = spotifySettings.Value;
        }
        public async Task<User> Authenticate(string code)
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

            User user = _dbContext.Users.FirstOrDefault(u => u.Id.Equals(profile.Id));

            if ( user!=null )
            {
                user.AccessToken = token.AccessToken;
                user.RefreshToken = token.RefreshToken;
                user.TokenCreateDate = token.CreateDate;
                user.ExpiresIn = token.ExpiresIn;
                user.DisplayName = profile.DisplayName;

                _dbContext.Users.Update(user);
            }
            else
            {
                user = new User
                {
                    Id = profile.Id,
                    AccessToken = token.AccessToken,
                    RefreshToken = token.RefreshToken,
                    ExpiresIn = token.ExpiresIn,
                    TokenCreateDate = token.CreateDate,
                    DisplayName = profile.DisplayName
                };
                _dbContext.Users.Add(user);
            }
            await _dbContext.SaveChangesAsync();

            return user;
        }

        public User GetById(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}