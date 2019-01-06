using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gavinhow.SpotifyStatistics.Api.Settings;
using Gavinhow.SpotifyStatistics.Database;
using Gavinhow.SpotifyStatistics.Database.Entity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;

namespace Gavinhow.SpotifyStatistics.Api
{
    public class SpotifyApi
    {
        private readonly SpotifySettings _spotifySettings;
        private readonly ILogger _logger;
        private SpotifyStatisticsContext dbContext;

        public SpotifyApi(IOptions<SpotifySettings> spotifySettings, ILogger<SpotifyApi> logger, SpotifyStatisticsContext dbContext)
        {
            _logger = logger;
            _spotifySettings = spotifySettings.Value;
            this.dbContext = dbContext;
        }

        public async Task<Token> RefreshToken(string refreshToken)
        {
            AuthorizationCodeAuth auth =
                    new AuthorizationCodeAuth(_spotifySettings.ClientId, _spotifySettings.ClientSecret, "https://localhost:5001/Login/Authorise", "https://localhost:5001",
                        Scope.UserReadRecentlyPlayed);
            return await auth.RefreshToken(refreshToken);
        }

        public async Task UpdateAllUsers()
        {
            foreach (var user in dbContext.Users.ToList())
            {
                _logger.LogDebug($"Updating recently played for {user.Id}");
                await SaveRecentlyPlayed(await RefreshToken(user.RefreshToken));
            }
            await dbContext.SaveChangesAsync();
        }

        private async Task SaveRecentlyPlayed(Token token)
        {
            var spotifyApi =  new SpotifyWebAPI()
            {
                AccessToken = token.AccessToken,
                TokenType = token.TokenType
            };
            PrivateProfile profile = await spotifyApi.GetPrivateProfileAsync();
            CursorPaging<PlayHistory> histories = spotifyApi.GetUsersRecentlyPlayedTracks(50);

            foreach (var item in histories.Items)
            {
                Play play = new Play
                {
                    TrackId = item.Track.Id,
                    UserId = profile.Id,
                    TimeOfPlay = item.PlayedAt
                };
                dbContext.Plays.AddIfNotExists(play,
                    x => x.TrackId == item.Track.Id
                    && x.UserId == profile.Id
                    && x.TimeOfPlay == item.PlayedAt);
            }

            dbContext.SaveChanges();
        }

        public async Task<FullTrack> GetTrackAsync(string trackId)
        {
            CredentialsAuth auth = new CredentialsAuth(_spotifySettings.ClientId, _spotifySettings.ClientSecret);
            Token token = await auth.GetToken();
            SpotifyWebAPI api = new SpotifyWebAPI() { TokenType = token.TokenType, AccessToken = token.AccessToken };

            return await api.GetTrackAsync(trackId);
        }

        public FullTrack GetTrack(string trackId)
        {
            CredentialsAuth auth = new CredentialsAuth(_spotifySettings.ClientId, _spotifySettings.ClientSecret);
            Token token = auth.GetToken().Result;
            SpotifyWebAPI api = new SpotifyWebAPI() { TokenType = token.TokenType, AccessToken = token.AccessToken };

            return api.GetTrack(trackId);
        }

        public List<FullTrack> GetTracks(List<string> trackIds)
        {
            CredentialsAuth auth = new CredentialsAuth(_spotifySettings.ClientId, _spotifySettings.ClientSecret);
            Token token = auth.GetToken().Result;
            SpotifyWebAPI api = new SpotifyWebAPI() { TokenType = token.TokenType, AccessToken = token.AccessToken };

            return api.GetSeveralTracks(trackIds).Tracks;
        }
    }
}
