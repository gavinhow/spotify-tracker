using System;
namespace Gavinhow.SpotifyStatistics.Database.Entity
{
    public class Play : BaseEntity
    {
        public string TrackId { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public DateTime TimeOfPlay { get; set; }
    }
}
