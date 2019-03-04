using System;
using System.Threading;
using System.Threading.Tasks;
using Gavinhow.SpotifyStatistics.Api;
using Gavinhow.SpotifyStatistics.Api.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gavinhow.SpotifyStatistics.Backend
{
    public class ImportHostedService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private Timer _timer;
        readonly SpotifyApiFacade _spotifyApiFacade;
        readonly SpotifySettings _spotifySettings;

        public ImportHostedService(ILogger<ImportHostedService> logger, SpotifyApiFacade spotifyApiFacade, IOptions<SpotifySettings> spotifySettings)
        {
            _logger = logger;
            _spotifyApiFacade = spotifyApiFacade;
            _spotifySettings = spotifySettings.Value;

        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(_spotifySettings.SyncInterval));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            _logger.LogInformation("Timed Background Service is working.");
            _spotifyApiFacade.UpdateAllUsers().Wait();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }

}
