using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gavinhow.SpotifyStatistics.Api.Models;
using Gavinhow.SpotifyStatistics.Api.Settings;
using Gavinhow.SpotifyStatistics.Database;
using Gavinhow.SpotifyStatistics.Database.Entity;
using Microsoft.Extensions.Options;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;

namespace Gavinhow.SpotifyStatistics.Api
{
    public class SpotifyApiFacade
    {
        private readonly SpotifySettings _spotifySettings;
        private Token _token;
        private readonly CredentialsAuth _credentialsAuth;

        private Token CredentialsToken
        {
            get {
                if (_token == null || _token.IsExpired())
                {
                    _token = _credentialsAuth.GetToken().Result;
                }
                return _token;
            }
        }

        public SpotifyApiFacade(IOptions<SpotifySettings> spotifySettings)
        {
            _spotifySettings = spotifySettings.Value;
            _credentialsAuth = new CredentialsAuth(_spotifySettings.ClientId, _spotifySettings.ClientSecret);
        }

        public async Task<Token> RefreshToken(string refreshToken)
        {
            AuthorizationCodeAuth auth =
                    new AuthorizationCodeAuth(_spotifySettings.ClientId, _spotifySettings.ClientSecret, "https://localhost:5001/Login/Authorise", "https://localhost:5001",
                        Scope.UserReadRecentlyPlayed);
            return await auth.RefreshToken(refreshToken);
        }

        public async Task<List<PlayHistory>> GetRecentlyPlayed(string userId, Token token)
        {
            var spotifyApi = new SpotifyWebAPI()
            {
                AccessToken = token.AccessToken,
                TokenType = token.TokenType
            };
            CursorPaging<PlayHistory> histories = await spotifyApi.GetUsersRecentlyPlayedTracksAsync(50);
            return histories.Items;
        }

        public async Task<FullTrack> GetTrackAsync(string trackId)
        {
            SpotifyWebAPI api = new SpotifyWebAPI { TokenType = CredentialsToken.TokenType, AccessToken = CredentialsToken.AccessToken };

            return await api.GetTrackAsync(trackId);
        }

        public FullTrack GetTrack(string trackId)
        {
            SpotifyWebAPI api = new SpotifyWebAPI { TokenType = CredentialsToken.TokenType, AccessToken = CredentialsToken.AccessToken };
            return api.GetTrack(trackId);
        }

        public List<FullTrack> GetTracks(List<string> trackIds)
        {
            SpotifyWebAPI api = new SpotifyWebAPI { TokenType = CredentialsToken.TokenType, AccessToken = CredentialsToken.AccessToken };
            return api.GetSeveralTracks(trackIds).Tracks;
        }
        
        public async Task<List<FullTrack>> GetSeveralTracksAsync(List<string> trackIds)
        {
            SpotifyWebAPI api = new SpotifyWebAPI { TokenType = CredentialsToken.TokenType, AccessToken = CredentialsToken.AccessToken };
            return (await api.GetSeveralTracksAsync(trackIds)).Tracks;
        }

        public List<FullArtist> GetArtists(List<string> artistIds)
        {
            SpotifyWebAPI api = new SpotifyWebAPI {TokenType = CredentialsToken.TokenType, AccessToken = CredentialsToken.AccessToken};

            return api.GetSeveralArtists(artistIds).Artists;
        }
    }
}
