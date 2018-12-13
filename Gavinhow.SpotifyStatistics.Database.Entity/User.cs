using System;
using System.Collections.Generic;

namespace Gavinhow.SpotifyStatistics.Database.Entity
{
    public class User : BaseEntity
    {
        public string UserName { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public double ExpiresIn { get; set; }

        public DateTime TokenCreateDate { get; set; }

        public List<Play> Plays { get; set; }

    }
}
