﻿using System;
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
                    _logger.LogInformation($"Updating recently played for {user.Id}");
                    await SaveRecentlyPlayed(await RefreshToken(user.RefreshToken), user.Id);
                }
                catch (Exception ex)
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
            CursorPaging<PlayHistory> histories = await spotifyApi.GetUsersRecentlyPlayedTracksAsync(50);
            SeveralTracks severalTracks = await spotifyApi.GetSeveralTracksAsync(histories.Items.Select(item => item.Track.Id).ToList());

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
            
            foreach (var item in severalTracks.Tracks)
            {
                if (_dbContext.Tracks.Any(obj => obj.Id == item.Id))
                {
                    continue;
                }
                SaveTrackInformation(item);
                _dbContext.SaveChanges();
            }

            var result = from play in _dbContext.Plays
                         join track in _dbContext.Tracks on play.TrackId equals track.Id into combined
                         from test in combined.DefaultIfEmpty()
                         where test.Id == null
                         select play.TrackId;

            List<string> trackIdsWithoutLocalDetails = result.ToList();
            for (int i = 0; i < trackIdsWithoutLocalDetails.Count; i+=50)
            {
               
                SeveralTracks tracksWithoutLocalDetails = await spotifyApi.GetSeveralTracksAsync(trackIdsWithoutLocalDetails.Skip(i).Take(50).ToList());

                _logger.LogWarning($"HELLO GAVIN2 {tracksWithoutLocalDetails.Tracks.Count}");

                foreach (var item in tracksWithoutLocalDetails.Tracks)
                {
                    if (_dbContext.Tracks.Any(obj => obj.Id == item.Id))
                    {
                        continue;
                    }
                    SaveTrackInformation(item);
                    _dbContext.SaveChanges();
                }
            }
           

        }

        private void SaveTrackInformation(FullTrack item)
        {
            Track track = new Track
            {
                Id = item.Id,
                AlbumId = item.Album.Id
            };

            _dbContext.Tracks.AddIfNotExists(track,
                x => x.Id == item.Id);
            foreach (var artist in item.Artists)
            {
                ArtistTrack artistTrack = new ArtistTrack
                {
                    TrackId = item.Id,
                    ArtistId = artist.Id
                };
                _dbContext.ArtistTracks.AddIfNotExists(artistTrack,
                    x => x.ArtistId == artistTrack.ArtistId
                    && x.TrackId == artistTrack.TrackId);
            }

            foreach (var artist in item.Album.Artists)
            {
                ArtistAlbum artistAlbum = new ArtistAlbum
                {
                    ArtistId = artist.Id,
                    AlbumId = item.Album.Id
                };
                _dbContext.ArtistAlbums.AddIfNotExists(artistAlbum,
                    x => x.AlbumId == artistAlbum.AlbumId
                    && x.ArtistId == artistAlbum.ArtistId);
            }
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
