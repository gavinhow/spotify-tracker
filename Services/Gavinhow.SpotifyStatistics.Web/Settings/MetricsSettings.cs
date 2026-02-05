namespace Gavinhow.SpotifyStatistics.Web.Settings;

public class MetricsSettings
{
    public string ServiceName { get; set; } = "spotify-api";
    public string Path { get; set; } = "/metrics";
    public string AllowedHost { get; set; } = "*:9090";
}
