using System.Threading.Tasks;
using Gavinhow.SpotifyStatistics.Database;
using Gavinhow.SpotifyStatistics.Database.Entity;
using Microsoft.Extensions.Logging;

namespace Gavinhow.SpotifyStatistics.Api.Services
{
    public interface IImportStatusService
    {
        Task EnableUserAsync(string userId);
        Task<bool> IsUserDisabledAsync(string userId);
    }

    public class ImportStatusService : IImportStatusService
    {
        private readonly SpotifyStatisticsContext _dbContext;
        private readonly ILogger<ImportStatusService> _logger;

        public ImportStatusService(SpotifyStatisticsContext dbContext, ILogger<ImportStatusService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task EnableUserAsync(string userId)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user != null && user.IsDisabled)
            {
                user.IsDisabled = false;
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("User {UserId} has been re-enabled", userId);
            }
        }

        public async Task<bool> IsUserDisabledAsync(string userId)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            return user?.IsDisabled ?? false;
        }
    }
}