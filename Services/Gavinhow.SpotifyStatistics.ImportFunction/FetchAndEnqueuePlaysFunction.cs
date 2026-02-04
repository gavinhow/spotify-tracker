using System.Text;
using System.Text.Json;
using Azure.Data.Tables;
using Azure.Storage.Queues;
using Gavinhow.SpotifyStatistics.Api;
using Gavinhow.SpotifyStatistics.Api.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Gavinhow.SpotifyStatistics.ImportFunction;

public class FetchAndEnqueuePlaysFunction
{
    private readonly TableClient _tableClient;
    private readonly SpotifyApiFacade _spotifyApiFacade;
    private readonly QueueClient _queueClient;
    private readonly ILogger<FetchAndEnqueuePlaysFunction> _logger;

    public FetchAndEnqueuePlaysFunction(
        TableClient tableClient,
        SpotifyApiFacade spotifyApiFacade,
        QueueClient queueClient,
        ILogger<FetchAndEnqueuePlaysFunction> logger)
    {
        _tableClient = tableClient;
        _spotifyApiFacade = spotifyApiFacade;
        _queueClient = queueClient;
        _logger = logger;
    }

    [Function("FetchAndEnqueuePlays")]
    public async Task RunAsync([TimerTrigger("0 */20 * * * *")] TimerInfo timer)
    {
        _logger.LogInformation("Starting play fetch at {Time}", DateTime.UtcNow);

        await _queueClient.CreateIfNotExistsAsync();
        await _tableClient.CreateIfNotExistsAsync();

        // Read all user tokens from Table Storage; filter disabled users in code
        // (Table Storage OData boolean filters are unreliable with Azurite)
        var allUsers = _tableClient.Query<UserTokenTableEntity>().ToList();
        var users = allUsers.Where(u => !u.IsDisabled).ToList();

        if (users.Count == 0)
        {
            _logger.LogInformation("No active users found in Table Storage");
            return;
        }

        _logger.LogInformation("Found {UserCount} active users in Table Storage", users.Count);

        var messagesEnqueued = 0;
        var totalPlays = 0;

        foreach (var user in users)
        {
            try
            {
                var token = await _spotifyApiFacade.RefreshToken(user.SpotifyRefreshToken);

                // Persist the refreshed access token back to Table Storage immediately.
                // This keeps the row authoritative even when home Postgres is down.
                user.SpotifyAccessToken = token.AccessToken;
                user.TokenExpiry = DateTimeOffset.UtcNow.AddSeconds(token.ExpiresIn);
                _tableClient.UpdateEntity(user, user.ETag);

                var recentlyPlayed = await _spotifyApiFacade.GetRecentlyPlayed(user.PartitionKey, token);

                if (!recentlyPlayed.Any()) continue;

                var message = new UserPlaysQueueMessage
                {
                    MessageId = Guid.NewGuid().ToString(),
                    UserId = user.PartitionKey,
                    EnqueuedAt = DateTime.UtcNow,
                    ImportWindowStart = recentlyPlayed.Min(p => p.PlayedAt),
                    ImportWindowEnd = recentlyPlayed.Max(p => p.PlayedAt),
                    Plays = recentlyPlayed.Select(p => new PlayData
                    {
                        TrackId = p.Track.Id,
                        TimeOfPlay = p.PlayedAt
                    }).ToList()
                };

                var json = JsonSerializer.Serialize(message);
                var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
                await _queueClient.SendMessageAsync(base64);

                messagesEnqueued++;
                totalPlays += message.Plays.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch plays for user {UserId}", user.PartitionKey);
            }
        }

        _logger.LogInformation(
            "Completed: {MessagesEnqueued} messages enqueued, {TotalPlays} plays total",
            messagesEnqueued, totalPlays);
    }
}
