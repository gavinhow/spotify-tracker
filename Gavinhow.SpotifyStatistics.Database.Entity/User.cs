using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gavinhow.SpotifyStatistics.Database.Entity
{
    public class User : BaseEntity
    {
        [Key]
        public string Id { get; set; }

        public string DisplayName { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public double ExpiresIn { get; set; }

        public DateTime TokenCreateDate { get; set; }

        public List<Play> Plays { get; set; }

    }
}
