using System.Threading.Tasks;
using Gavinhow.SpotifyStatistics.Api;
using Gavinhow.SpotifyStatistics.Database;
using Gavinhow.SpotifyStatistics.Database.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SpotifyAPI.Web.Models;

namespace Gavinhow.SpotifyStatistics.ImportFunction
{
    public class ImportUserHistory
    {
        private readonly SpotifyStatisticsContext _dbContext;
        private readonly SpotifyApiFacade _spotifyApiFacade;

        public ImportUserHistory(SpotifyStatisticsContext dbContext, SpotifyApiFacade spotifyApiFacade)
        {
            _dbContext = dbContext;
            _spotifyApiFacade = spotifyApiFacade;
        }

        [FunctionName("ImportUserHistory")]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequest req, ILogger log)
        {
            string userId = req.Query["userId"];

            User user = _dbContext.Users.Find(userId);
            log.LogTrace($"Refreshing users token. ({userId})");
            Token newToken = await _spotifyApiFacade.RefreshToken(user.RefreshToken);
            log.LogTrace($"Getting latest track information. ({userId})");
            foreach (PlayHistory item in await _spotifyApiFacade.GetRecentlyPlayed(user.Id, newToken))
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

            int recordsUpdated = _dbContext.SaveChanges();
            log.LogInformation($"{recordsUpdated} records updated. ({userId})");
            
            return userId != null
                ? (ActionResult) new OkObjectResult($"User history updated successfully: {userId}")
                : new BadRequestObjectResult("Please pass a existing userId on the query string");
        }
    }
}