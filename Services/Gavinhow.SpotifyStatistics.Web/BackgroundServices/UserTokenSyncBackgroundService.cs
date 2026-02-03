using Azure.Data.Tables;
using Gavinhow.SpotifyStatistics.Api.Models;
using Gavinhow.SpotifyStatistics.Database;
using Gavinhow.SpotifyStatistics.Web.Settings;
using Microsoft.Extensions.Options;

namespace Gavinhow.SpotifyStatistics.Web.BackgroundServices;

public class UserTokenSyncBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly TableClient _tableClient;
    private readonly TimeSpan _syncInterval;
    private readonly ILogger<UserTokenSyncBackgroundService> _logger;

    public UserTokenSyncBackgroundService(
        IServiceProvider serviceProvider,
        TableClient tableClient,
        IOptions<TableStorageSettings> settings,
        ILogger<UserTokenSyncBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _tableClient = tableClient;
        _syncInterval = TimeSpan.FromSeconds(settings.Value.SyncIntervalSeconds);
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("User token sync service starting");

        await _tableClient.CreateIfNotExistsAsync(cancellationToken: stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await SyncUsersAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user token sync cycle");
            }

            await Task.Delay(_syncInterval, stoppingToken);
        }
    }

    private async Task SyncUsersAsync(CancellationToken ct)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SpotifyStatisticsContext>();

        var users = dbContext.Users.ToList();

        // Snapshot current Table Storage state for comparison
        var existingEntities = _tableClient
            .Query<UserTokenTableEntity>(cancellationToken: ct)
            .ToList();
        var existingByUserId = existingEntities.ToDictionary(e => e.PartitionKey);

        var synced = 0;

        foreach (var user in users)
        {
            if (existingByUserId.TryGetValue(user.Id, out var existing))
            {
                // User already in Table Storage.  Only update if RefreshToken or
                // IsDisabled changed.  Crucially we do NOT touch AccessToken or
                // TokenExpiry — those are owned by the Function after it refreshes.
                if (existing.SpotifyRefreshToken == user.RefreshToken
                    && existing.IsDisabled == user.IsDisabled)
                    continue;

                existing.SpotifyRefreshToken = user.RefreshToken;
                existing.IsDisabled = user.IsDisabled;
                existing.LastSynced = DateTimeOffset.UtcNow;
                await _tableClient.UpdateEntityAsync(existing, existing.ETag, cancellationToken: ct);
            }
            else
            {
                // Brand-new user not yet in Table Storage — full upsert so the
                // Function can start importing for them on the next cycle.
                var tokenExpiry = new DateTimeOffset(user.TokenCreateDate, TimeSpan.Zero)
                    .AddSeconds(user.ExpiresIn);

                var entity = new UserTokenTableEntity(user.Id)
                {
                    SpotifyAccessToken = user.AccessToken,
                    SpotifyRefreshToken = user.RefreshToken,
                    TokenExpiry = tokenExpiry,
                    LastSynced = DateTimeOffset.UtcNow,
                    IsDisabled = user.IsDisabled
                };

                await _tableClient.UpsertEntityAsync(entity, cancellationToken: ct);
            }

            synced++;
        }

        if (synced > 0)
        {
            _logger.LogInformation("Synced {SyncedCount} user token(s) to Table Storage", synced);
        }
    }
}
