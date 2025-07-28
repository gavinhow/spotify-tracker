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
      await importer.ImportUserHistory();
      
      // Then import the track information which could be missing
      await importer.ImportTrackInformation();

      await Task.Delay(TimeSpan.FromMinutes(25), stoppingToken); // run every 25 min
    }
  }
}