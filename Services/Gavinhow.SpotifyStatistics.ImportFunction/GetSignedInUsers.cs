using System.Linq;
using System.Threading.Tasks;
using Gavinhow.SpotifyStatistics.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Gavinhow.SpotifyStatistics.ImportFunction
{
    public class GetSignedInUsers
    {
        private readonly SpotifyStatisticsContext _dbContext;

        public GetSignedInUsers(SpotifyStatisticsContext dbContext)
        {
            _dbContext = dbContext;
        }
        [FunctionName("GetSignInUsers")]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequest req, ILogger log)
        {
            return new OkObjectResult(_dbContext.Users.Select(user => user.Id).ToList());
        }
    }
}