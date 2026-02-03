namespace Gavinhow.SpotifyStatistics.Web.Settings
{
    public class CorsSettings
    {
        public string[] AllowedOrigins { get; set; } = Array.Empty<string>();
        public string[] AllowedMethods { get; set; } = ["GET", "POST", "OPTIONS"];
        public string[] AllowedHeaders { get; set; } = ["Content-Type", "Authorization"];
        public string[] ExposedHeaders { get; set; } = Array.Empty<string>();
        public int MaxAgeSeconds { get; set; } = 3600;
        public bool AllowCredentials { get; set; } = false;
    }
}
