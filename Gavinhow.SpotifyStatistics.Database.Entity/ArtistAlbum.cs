using System;
using System.ComponentModel.DataAnnotations;

namespace Gavinhow.SpotifyStatistics.Database.Entity
{
    public class ArtistAlbum
    {
        public string ArtistId { get; set; }

        public string AlbumId { get; set; }
    }
}
