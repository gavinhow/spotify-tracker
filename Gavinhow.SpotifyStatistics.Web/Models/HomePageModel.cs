using System;
using Gavinhow.SpotifyStatistics.Database.Entity;

namespace Gavinhow.SpotifyStatistics.Web.Models
{
    public class HomePageModel
    {
        public User CurrentUser { get; set; }

        public bool IsCurrentUserLoggedIn => (CurrentUser != null);
    }
}
