using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gavinhow.SpotifyStatistics.Api.Models;
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
        private SpotifyStatisticsContext _dbContext;

        public SpotifyApi(IOptions<SpotifySettings> spotifySettings, ILogger<SpotifyApi> logger, SpotifyStatisticsContext dbContext)
        {
            _logger = logger;
            _spotifySettings = spotifySettings.Value;
            this._dbContext = dbContext;
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
            foreach (var user in _dbContext.Users.ToList())
            {
                try
                {
                    _logger.LogDebug($"Updating recently played for {user.Id}");
                    await SaveRecentlyPlayed(await RefreshToken(user.RefreshToken), user.Id);
                }
                catch (System.Exception ex)
                {
                    _logger.LogError($"Error when trying to update {user.Id}");
                    _logger.LogError($"{ex.Message}\n{ex.StackTrace}");
                }
               
            }
            await _dbContext.SaveChangesAsync();
        }

        private async Task SaveRecentlyPlayed(Token token, string userId)
        {
            var spotifyApi = new SpotifyWebAPI()
            {
                AccessToken = token.AccessToken,
                TokenType = token.TokenType
            };
            CursorPaging<PlayHistory> histories = spotifyApi.GetUsersRecentlyPlayedTracks(50);

            foreach (var item in histories.Items)
            {
                Play play = new Play
                {
                    TrackId = item.Track.Id,
                    UserId = userId,
                    TimeOfPlay = item.PlayedAt
                };
                _dbContext.Plays.AddIfNotExists(play,
                    x => x.TrackId == item.Track.Id
                    && x.UserId == userId
                    && x.TimeOfPlay == item.PlayedAt);
            }

            _dbContext.SaveChanges();
        }

        public async Task<FullTrack> GetTrackAsync(string trackId)
        {
            CredentialsAuth auth = new CredentialsAuth(_spotifySettings.ClientId, _spotifySettings.ClientSecret);
            Token token = await auth.GetToken();
            SpotifyWebAPI api = new SpotifyWebAPI { TokenType = token.TokenType, AccessToken = token.AccessToken };

            return await api.GetTrackAsync(trackId);
        }

        public FullTrack GetTrack(string trackId)
        {
            CredentialsAuth auth = new CredentialsAuth(_spotifySettings.ClientId, _spotifySettings.ClientSecret);
            Token token = auth.GetToken().Result;
            SpotifyWebAPI api = new SpotifyWebAPI { TokenType = token.TokenType, AccessToken = token.AccessToken };

            return api.GetTrack(trackId);
        }

        public List<FullTrack> GetTracks(List<string> trackIds)
        {
            CredentialsAuth auth = new CredentialsAuth(_spotifySettings.ClientId, _spotifySettings.ClientSecret);
            Token token = auth.GetToken().Result;
            SpotifyWebAPI api = new SpotifyWebAPI { TokenType = token.TokenType, AccessToken = token.AccessToken };

            return api.GetSeveralTracks(trackIds).Tracks;
        }

        public Play GetOldestSong(string userId)
        {
            return _dbContext.Plays
                        .Where(play => play.UserId == userId)
                        .OrderBy(play => play.TimeOfPlay).First();
        }

        public SongPlayCount GetMostPlayedSong(string userId)
        {
            var mostplayedsong = _dbContext.Plays
                        .Where(play => play.UserId == userId)
                        .GroupBy(play => play.TrackId)
                        .OrderByDescending(gp => gp.Count()).First();

            return new SongPlayCount { track = this.GetTrack(mostplayedsong.Key), plays = mostplayedsong.Count() };
        }

        public List<SongPlayCount> GetMostPlayedSongs(string userId, int numOfSongs = 10)
        {
            var topPlays = _dbContext.Plays
                        .Where(play => play.UserId == userId)
                        .GroupBy(play => play.TrackId)
                        .OrderByDescending(gp => gp.Count())
                        .Take(numOfSongs).ToList();

            List<string> trackIds = topPlays.Select(topPlay => topPlay.Key).ToList();
            List<FullTrack> tracks = this.GetTracks(trackIds);

            return topPlays.Select(topPlay => new SongPlayCount { track = tracks.First(track => track.Id == topPlay.Key), plays = topPlay.Count() }).ToList();
        }
    }
}
