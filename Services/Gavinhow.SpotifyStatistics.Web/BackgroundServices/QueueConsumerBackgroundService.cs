using System.Text;
using System.Text.Json;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Gavinhow.SpotifyStatistics.Api.Models;
using Gavinhow.SpotifyStatistics.Web.Services;
using Gavinhow.SpotifyStatistics.Web.Settings;
using Microsoft.Extensions.Options;

namespace Gavinhow.SpotifyStatistics.Web.BackgroundServices;

public class QueueConsumerBackgroundService : BackgroundService
{
    private readonly QueueClient _queueClient;
    private readonly IServiceProvider _serviceProvider;
    private readonly QueueSettings _settings;
    private readonly ILogger<QueueConsumerBackgroundService> _logger;

    public QueueConsumerBackgroundService(
        QueueClient queueClient,
        IServiceProvider serviceProvider,
        IOptions<QueueSettings> settings,
        ILogger<QueueConsumerBackgroundService> logger)
    {
        _queueClient = queueClient;
        _serviceProvider = serviceProvider;
        _settings = settings.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Queue consumer background service starting");

        // Ensure queue exists
        await _queueClient.CreateIfNotExistsAsync(cancellationToken: stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessMessagesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing queue messages");
            }

            await Task.Delay(TimeSpan.FromSeconds(_settings.PollingIntervalSeconds), stoppingToken);
        }

        _logger.LogInformation("Queue consumer background service stopping");
    }

    private async Task ProcessMessagesAsync(CancellationToken ct)
    {
        var visibilityTimeout = TimeSpan.FromMinutes(_settings.VisibilityTimeoutMinutes);

        QueueMessage[] messages = await _queueClient.ReceiveMessagesAsync(
            maxMessages: _settings.MaxMessagesPerPoll,
            visibilityTimeout: visibilityTimeout,
            cancellationToken: ct);

        if (messages.Length == 0)
        {
            return;
        }

        _logger.LogInformation("Received {MessageCount} messages from queue", messages.Length);

        foreach (var message in messages)
        {
            if (ct.IsCancellationRequested) break;

            try
            {
                var queueMessage = DeserializeMessage(message);
                if (queueMessage == null)
                {
                    _logger.LogWarning(
                        "Failed to deserialize message {MessageId}, dequeue count: {DequeueCount}",
                        message.MessageId, message.DequeueCount);

                    // After 5 failed attempts, Azure will move to poison queue automatically
                    // For now, just delete malformed messages
                    if (message.DequeueCount >= 5)
                    {
                        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, ct);
                    }
                    continue;
                }

                // Process the message using a scoped service
                using var scope = _serviceProvider.CreateScope();
                var processor = scope.ServiceProvider.GetRequiredService<IPlaysQueueProcessor>();

                await processor.ProcessPlaysAsync(queueMessage, ct);

                // Success - delete message from queue
                await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, ct);

                _logger.LogDebug(
                    "Successfully processed message {MessageId} for user {UserId}",
                    queueMessage.MessageId, queueMessage.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error processing message {MessageId}, dequeue count: {DequeueCount}",
                    message.MessageId, message.DequeueCount);

                // Message will become visible again after visibility timeout
                // and will be retried. After max dequeue count, moves to poison queue.
            }
        }
    }

    private UserPlaysQueueMessage? DeserializeMessage(QueueMessage message)
    {
        try
        {
            // Messages are Base64 encoded
            var json = Encoding.UTF8.GetString(Convert.FromBase64String(message.MessageText));
            return JsonSerializer.Deserialize<UserPlaysQueueMessage>(json);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to deserialize queue message");
            return null;
        }
    }
}
