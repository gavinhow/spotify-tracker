using Prometheus;

namespace Gavinhow.SpotifyStatistics.Web.Observability;

public class QueueProcessingMetrics
{
    private static readonly Counter RecordsProcessed = Metrics.CreateCounter(
        "spotify_records_processed_total",
        "Total records processed from the queue by outcome.",
        new CounterConfiguration
        {
            LabelNames = new[] { "service", "user_id", "status" }
        });

    private static readonly Counter MessagesProcessed = Metrics.CreateCounter(
        "spotify_queue_messages_processed_total",
        "Total queue messages processed by outcome.",
        new CounterConfiguration
        {
            LabelNames = new[] { "service", "status" }
        });

    private static readonly Gauge ActiveProcessing = Metrics.CreateGauge(
        "spotify_queue_processing_active",
        "Current number of active queue processing operations.",
        new GaugeConfiguration
        {
            LabelNames = new[] { "service" }
        });

    private static readonly Gauge QueueSize = Metrics.CreateGauge(
        "spotify_queue_size",
        "Approximate current queue size.",
        new GaugeConfiguration
        {
            LabelNames = new[] { "service" }
        });

    private static readonly Histogram ProcessingDuration = Metrics.CreateHistogram(
        "spotify_queue_processing_duration_seconds",
        "Time taken to process each queue message.",
        new HistogramConfiguration
        {
            LabelNames = new[] { "service", "operation" },
            Buckets = new[] { 0.01, 0.05, 0.1, 0.5, 1, 2, 5, 10 }
        });

    private static readonly Gauge LastImportTimestamp = Metrics.CreateGauge(
        "spotify_queue_import_last_timestamp_seconds",
        "Unix timestamp of the last queue import run completion.",
        new GaugeConfiguration
        {
            LabelNames = new[] { "service", "status" }
        });

    private static readonly Counter ImportCompletedTotal = Metrics.CreateCounter(
        "spotify_queue_import_completed_total",
        "Total completed queue import runs by outcome.",
        new CounterConfiguration
        {
            LabelNames = new[] { "service", "status" }
        });

    private static readonly Histogram ImportDuration = Metrics.CreateHistogram(
        "spotify_queue_import_duration_seconds",
        "Duration of full queue import runs.",
        new HistogramConfiguration
        {
            LabelNames = new[] { "service", "status" },
            Buckets = new[] { 0.01, 0.05, 0.1, 0.5, 1, 2, 5, 10 }
        });

    private static readonly Counter TracksMetadataImported = Metrics.CreateCounter(
        "spotify_track_metadata_imported_total",
        "Total track metadata records imported.",
        new CounterConfiguration
        {
            LabelNames = new[] { "service", "status" }
        });

    private readonly string _serviceName;

    public QueueProcessingMetrics(string serviceName)
    {
        _serviceName = serviceName;
    }

    public void IncrementRecordsProcessed(string userId, string status, double count)
    {
        if (count <= 0)
        {
            return;
        }

        RecordsProcessed.WithLabels(_serviceName, userId, status).Inc(count);
    }

    public void IncrementMessagesProcessed(string status)
    {
        MessagesProcessed.WithLabels(_serviceName, status).Inc();
    }

    public void IncrementActiveProcessing()
    {
        ActiveProcessing.WithLabels(_serviceName).Inc();
    }

    public void DecrementActiveProcessing()
    {
        ActiveProcessing.WithLabels(_serviceName).Dec();
    }

    public void SetQueueSize(int size)
    {
        QueueSize.WithLabels(_serviceName).Set(size);
    }

    public IDisposable StartProcessingTimer(string operation)
    {
        return ProcessingDuration.WithLabels(_serviceName, operation).NewTimer();
    }

    public void RecordImportRunCompletion(string status, double durationSeconds)
    {
        ImportCompletedTotal.WithLabels(_serviceName, status).Inc();
        ImportDuration.WithLabels(_serviceName, status).Observe(durationSeconds);
        LastImportTimestamp.WithLabels(_serviceName, status).Set(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
    }

    public void IncrementTrackMetadataImported(double count, string status = "success")
    {
        if (count <= 0)
        {
            return;
        }

        TracksMetadataImported.WithLabels(_serviceName, status).Inc(count);
    }
}
