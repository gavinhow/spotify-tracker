using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gavinhow.SpotifyStatistics.Database.Entity
{
    public class BaseEntity
    {
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
}
