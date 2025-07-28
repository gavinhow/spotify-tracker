namespace Gavinhow.SpotifyStatistics.Importer;

public class Worker(ILogger<Worker> logger, IServiceProvider services) : BackgroundService
{
  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    while (!stoppingToken.IsCancellationRequested)
    {
      logger.LogInformation("Running Spotify import at: {time}", DateTimeOffset.Now);
      using var scope = services.CreateScope();
      var importer = scope.ServiceProvider.GetRequiredService<SpotifyImporter>();

      
      // First import all the user history
      try
      {
        await importer.ImportUserHistory();
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "Failed to import user history");
      }
      
      // Then import the track information which could be missing
      try
      {
        await importer.ImportTrackInformation();
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "Failed to import track information");
      }

      await Task.Delay(TimeSpan.FromMinutes(25), stoppingToken); // run every 25 min
    }
  }
}