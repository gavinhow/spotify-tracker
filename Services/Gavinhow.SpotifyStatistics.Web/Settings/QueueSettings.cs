namespace Gavinhow.SpotifyStatistics.Web.Settings;

public class QueueSettings
{
    public string ConnectionString { get; set; } = "";
    public string QueueName { get; set; } = "spotify-plays";
    public int PollingIntervalSeconds { get; set; } = 10;
    public int VisibilityTimeoutMinutes { get; set; } = 5;
    public int MaxMessagesPerPoll { get; set; } = 32;
    public bool Enabled { get; set; } = true;
}
