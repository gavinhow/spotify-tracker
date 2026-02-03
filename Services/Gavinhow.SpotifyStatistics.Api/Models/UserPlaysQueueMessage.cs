using System;
using System.Collections.Generic;

namespace Gavinhow.SpotifyStatistics.Api.Models
{
    public class UserPlaysQueueMessage
    {
        public string MessageId { get; set; } = "";
        public string UserId { get; set; } = "";
        public DateTime EnqueuedAt { get; set; }
        public DateTime ImportWindowStart { get; set; }
        public DateTime ImportWindowEnd { get; set; }
        public List<PlayData> Plays { get; set; } = new List<PlayData>();
    }

    public class PlayData
    {
        public string TrackId { get; set; } = "";
        public DateTime TimeOfPlay { get; set; }
    }
}
