using System;
using System.ComponentModel.DataAnnotations;

namespace Gavinhow.SpotifyStatistics.Database.Entity
{
    public class ArtistTrack : BaseEntity
    {
            public string ArtistId { get; set; }

            public string TrackId { get; set; }
    }
}
