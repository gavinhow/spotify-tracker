using System.Collections.Generic;
using Gavinhow.SpotifyStatistics.Database;
using Gavinhow.SpotifyStatistics.Database.Entity;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

namespace Gavinhow.SpotifyStatistics.Web.Tests;

public static class Utilities
{
    public static void InitializeDbForTests(SpotifyStatisticsContext db)
    {
        db.Users.AddRange(GetSeedingMessages());
        db.SaveChanges();
    }

    public static List<User> GetSeedingMessages()
    {
        return new List<User>()
        {
            new User(){ Id = "test_user", DisplayName = "Gavin How"},
        };
    }
}