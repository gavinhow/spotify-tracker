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

        private Token CredentialsToken
        {
            get
            {
                if (_token == null || _token.IsExpired())
                {
                    _token = _credentialsAuth.GetToken().Result;
                }

                return _token;
            }
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
            SpotifyWebAPI api = new SpotifyWebAPI
                {TokenType = CredentialsToken.TokenType, AccessToken = CredentialsToken.AccessToken};

            return await api.GetTrackAsync(trackId);
        }

        public FullTrack GetTrack(string trackId)
        {
            SpotifyWebAPI api = new SpotifyWebAPI
                {TokenType = CredentialsToken.TokenType, AccessToken = CredentialsToken.AccessToken};
            return api.GetTrack(trackId);
        }

        public List<FullTrack> GetTracks(List<string> trackIds)
        {
            List<FullTrack> result = new List<FullTrack>();

            for (int i = trackIds.Count - 1; i >= 0; i--)
            {
                FullTrack output;
                if (_cache.TryGetValue(trackIds[i], out output))
                {
                    trackIds.RemoveAt(i);
                    result.Add(output);
                }
            }

            if (trackIds.Count > 0)
            {
                SpotifyWebAPI api = new SpotifyWebAPI
                    {TokenType = CredentialsToken.TokenType, AccessToken = CredentialsToken.AccessToken};
                var apiResult = api.GetSeveralTracks(trackIds).Tracks;
                foreach (var track in apiResult)
                {
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        // Set cache entry size by extension method.
                        .SetSize(1)
                        // Keep in cache for this time, reset time if accessed.
                        .SetSlidingExpiration(TimeSpan.FromDays(1));
                    _cache.Set(track.Id, track, cacheEntryOptions);
                }

                result.AddRange(apiResult);
            }

            return result;
        }

        public async Task<List<FullTrack>> GetSeveralTracksAsync(List<string> trackIds)
        {
            SpotifyWebAPI api = new SpotifyWebAPI
                {TokenType = CredentialsToken.TokenType, AccessToken = CredentialsToken.AccessToken};
            return (await api.GetSeveralTracksAsync(trackIds)).Tracks;
        }

        public List<FullArtist> GetArtists(List<string> artistIds)
        {
            SpotifyWebAPI api = new SpotifyWebAPI
                {TokenType = CredentialsToken.TokenType, AccessToken = CredentialsToken.AccessToken};

            return api.GetSeveralArtists(artistIds).Artists;
        }
        
        public async Task<List<FullArtist>> GetArtistsAsync(List<string> artistIds)
        {
            SpotifyWebAPI api = new SpotifyWebAPI
                {TokenType = CredentialsToken.TokenType, AccessToken = CredentialsToken.AccessToken};

            return (await api.GetSeveralArtistsAsync(artistIds)).Artists;
        }
        
        public List<FullAlbum> GetAlbums(List<string> albumIds)
        {
            SpotifyWebAPI api = new SpotifyWebAPI
                {TokenType = CredentialsToken.TokenType, AccessToken = CredentialsToken.AccessToken};
            return api.GetSeveralAlbums(albumIds).Albums;
        }
        
        public async Task<List<FullAlbum>> GetAlbumsAsync(List<string> albumIds)
        {
            SpotifyWebAPI api = new SpotifyWebAPI
                {TokenType = CredentialsToken.TokenType, AccessToken = CredentialsToken.AccessToken};
            return (await api.GetSeveralAlbumsAsync(albumIds)).Albums;
        }

        public List<SimpleAlbum> GetArtistsAlbums(string artistId)
        {
            SpotifyWebAPI api = new SpotifyWebAPI
                {TokenType = CredentialsToken.TokenType, AccessToken = CredentialsToken.AccessToken};
            return api.GetArtistsAlbums(artistId, AlbumType.Album,limit:50).Items;
        }
    }
}