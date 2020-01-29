using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Gavinhow.SpotifyStatistics.Api;
using Gavinhow.SpotifyStatistics.Database;
using Gavinhow.SpotifyStatistics.Database.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SpotifyAPI.Web.Models;

namespace Gavinhow.SpotifyStatistics.ImportFunction
{
    public class ImportTrackInformation
    {
        
        private readonly SpotifyStatisticsContext _dbContext;
        private readonly SpotifyApiFacade _spotifyApiFacade;

        public ImportTrackInformation(SpotifyStatisticsContext dbContext, SpotifyApiFacade spotifyApiFacade)
        {
            _dbContext = dbContext;
            _spotifyApiFacade = spotifyApiFacade;
        }
        [FunctionName("ImportTrackInformation")]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequest req, ILogger log)
        {
            var results = (from Plays in _dbContext.Plays
                join Tracks in _dbContext.Tracks
                    on Plays.TrackId equals Tracks.Id into joined
                from j in joined.DefaultIfEmpty()
                where j.Id == null
                select Plays.TrackId).Distinct();
            
            List<string> trackIds = results.ToList();
            log.LogInformation($"Tracks found with no track data {trackIds.Count}");

            for (int i = 0; i < trackIds.Count; i += 50)
            {
                foreach (var item in await _spotifyApiFacade.GetSeveralTracksAsync(trackIds.Skip(i).Take(50).ToList()))
                {
                    SaveTrackInformation(item);
                }
            }

            int recordsUpdated = _dbContext.SaveChanges();
            log.LogInformation($"Records updated {recordsUpdated}");
            return new OkObjectResult($"Tracks imported: {trackIds.Count}");
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


    }
}