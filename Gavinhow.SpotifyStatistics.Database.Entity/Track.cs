using System.ComponentModel.DataAnnotations;

namespace Gavinhow.SpotifyStatistics.Database.Entity
{
    public class Track : BaseEntity
    {
        [Key]
        public string Id { get; set; }

        public string AlbumId { get; set; }

    }
}
