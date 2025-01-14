using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gavinhow.SpotifyStatistics.Api.Settings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;

namespace Gavinhow.SpotifyStatistics.Api
{
    public class SpotifyApiFacade
    {
        private readonly IMemoryCache _cache;
        private readonly SpotifySettings _spotifySettings;
        private Token _token;
        private readonly CredentialsAuth _credentialsAuth;
        
        private async Task<Token> GetCredentialsToken()
        {
            if (_token == null || _token.IsExpired())
            {
                _token = await _credentialsAuth.GetToken();
            }

            return _token;
        }

        public SpotifyApiFacade(IOptions<SpotifySettings> spotifySettings, IMemoryCache memoryCache)
        {
            _cache = memoryCache;
            _spotifySettings = spotifySettings.Value;
            _credentialsAuth = new CredentialsAuth(_spotifySettings.ClientId, _spotifySettings.ClientSecret);
        }

        public async Task<Token> RefreshToken(string refreshToken)
        {
            AuthorizationCodeAuth auth =
                new AuthorizationCodeAuth(_spotifySettings.ClientId, _spotifySettings.ClientSecret,
                    "https://localhost:5001/Login/Authorise", "https://localhost:5001",
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
            Token token = await GetCredentialsToken();
            SpotifyWebAPI api = new SpotifyWebAPI
                {TokenType =  token.TokenType, AccessToken = token.AccessToken};

            return await api.GetTrackAsync(trackId);
        }

        public async Task<List<FullTrack>> GetSeveralTracksAsync(List<string> trackIds)
        {
            if (trackIds.Count > 50)
                throw new ArgumentException("Can only get 50 tracks at a time");
            Token token = await GetCredentialsToken();
            SpotifyWebAPI api = new SpotifyWebAPI
                {TokenType = token.TokenType, AccessToken = token.AccessToken};
            var response = await api.GetSeveralTracksAsync(trackIds);
            if (response.HasError())
                throw new Exception(response.Error.Message);
            else
                return response.Tracks;
        }

        
        public async Task<List<FullArtist>> GetArtistsAsync(List<string> artistIds)
        {
            if (artistIds.Count > 50)
                throw new ArgumentException("Can only get 50 artists at a time");
            Token token = await GetCredentialsToken();
            SpotifyWebAPI api = new SpotifyWebAPI
                {TokenType = token.TokenType, AccessToken = token.AccessToken};

            return (await api.GetSeveralArtistsAsync(artistIds)).Artists;
        }
        
        
        public async Task<List<FullAlbum>> GetAlbumsAsync(List<string> albumIds)
        {
            if (albumIds.Count > 20)
                throw new ArgumentException("Can only get 20 albums at a time");
            Token token = await GetCredentialsToken();
            SpotifyWebAPI api = new SpotifyWebAPI
                {TokenType = token.TokenType, AccessToken = token.AccessToken};
            return (await api.GetSeveralAlbumsAsync(albumIds)).Albums;
        }

        public async Task<PublicProfile> GetPublicProfileAsync(string userId)
        {
            Token token = await GetCredentialsToken();
            SpotifyWebAPI api = new SpotifyWebAPI
                {TokenType = token.TokenType, AccessToken = token.AccessToken};
            return await api.GetPublicProfileAsync(userId);
        }
    }
}