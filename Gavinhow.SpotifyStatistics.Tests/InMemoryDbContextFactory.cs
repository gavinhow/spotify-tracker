using Gavinhow.SpotifyStatistics.Database;
using Microsoft.EntityFrameworkCore;

namespace Gavinhow.SpotifyStatistics.Tests
{
    public class InMemoryDbContextFactory
    {
        public static SpotifyStatisticsContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<SpotifyStatisticsContext>()
                            .UseInMemoryDatabase(databaseName: "InMemoryOdsDatabase")
                            .Options;
            var dbContext = new SpotifyStatisticsContext(options);

            return dbContext;
        }
    }
}
