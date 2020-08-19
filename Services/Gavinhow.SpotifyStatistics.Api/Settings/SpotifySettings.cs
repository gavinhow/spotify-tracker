using System;
namespace Gavinhow.SpotifyStatistics.Api.Settings
{
    public class SpotifySettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string CallbackUri { get; set; }
        public string ServerUri { get; set; }
        public int SyncInterval { get; set; }
    }
}
