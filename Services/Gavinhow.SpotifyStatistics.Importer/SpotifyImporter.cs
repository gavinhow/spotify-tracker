using Gavinhow.SpotifyStatistics.Api;
using Gavinhow.SpotifyStatistics.Api.Settings;
using Gavinhow.SpotifyStatistics.Database;
using Gavinhow.SpotifyStatistics.Database.Entity;
using Microsoft.Extensions.Options;
using SpotifyAPI.Web.Models;

namespace Gavinhow.SpotifyStatistics.Importer;

public class SpotifyImporter(SpotifyStatisticsContext dbContext, SpotifyApiFacade spotifyApiFacade, ILogger<SpotifyImporter> logger, IOptions<ImportSettings> importSettings)
{
  public async Task ImportUserHistory()
  {
    User[] users = dbContext.Users.Where(u => !u.IsDisabled).ToArray();
    foreach (User user in users)
    {
      try
      {
        await ImportUserHistory(user);
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "Failed to import history for user {UserId}", user.Id);
        await LogImportAttempt(user.Id, 0, false, ex.Message);
        await CheckAndDisableUserIfNeeded(user.Id);
      }
    }
  }

  public async Task ImportTrackInformation()
  {
    var results = (from Plays in dbContext.Plays
      join Tracks in dbContext.Tracks
        on Plays.TrackId equals Tracks.Id into joined
      from j in joined.DefaultIfEmpty()
      where j.Id == null
      select Plays.TrackId).Distinct();
            
    List<string> trackIds = results.ToList();
    logger.LogInformation("Tracks found with no track data {TrackIdsCount}", trackIds.Count);
    for (int i = 0; i < trackIds.Count; i += 50)
    {
      foreach (var item in await spotifyApiFacade.GetSeveralTracksAsync(trackIds.Skip(i).Take(50).ToList()))
      {
        SaveTrackInformation(item);
      }
    }
    var recordsUpdated = dbContext.SaveChangesAsync();

    logger.LogInformation($"Records updated {recordsUpdated}");
  }
  
  private async Task ImportUserHistory(User user)
  {
    var userId = user.Id;
    logger.LogTrace("Refreshing users token. ({UserId})", userId);
    Token newToken = await spotifyApiFacade.RefreshToken(user.RefreshToken);
    logger.LogTrace("Getting latest track information. ({UserId})", userId);
    
    var recentlyPlayed = await spotifyApiFacade.GetRecentlyPlayed(user.Id, newToken);
    if (!recentlyPlayed.Any()) 
    {
      await LogImportAttempt(userId, 0, true, null);
      return;
    }
    
    // Get the date range of the new plays to minimize the duplicate check query
    var minPlayTime = recentlyPlayed.Min(p => p.PlayedAt);
    var maxPlayTime = recentlyPlayed.Max(p => p.PlayedAt);
    
    // Only get existing plays within the timeframe we're importing
    var existingPlaysInRange = dbContext.Plays
      .Where(p => p.UserId == userId && p.TimeOfPlay >= minPlayTime && p.TimeOfPlay <= maxPlayTime)
      .Select(p => new { p.TrackId, p.TimeOfPlay })
      .ToHashSet();
    
    var newPlays = new List<Play>();
    
    foreach (PlayHistory item in recentlyPlayed)
    {
      var playKey = new { TrackId = item.Track.Id, TimeOfPlay = item.PlayedAt };
      
      if (!existingPlaysInRange.Contains(playKey))
      {
        newPlays.Add(new Play
        {
          TrackId = item.Track.Id,
          UserId = userId,
          TimeOfPlay = item.PlayedAt
        });
      }
    }

    if (newPlays.Count > 0)
    {
      dbContext.Plays.AddRange(newPlays);
    }

    int recordsUpdated = await dbContext.SaveChangesAsync();
    logger.LogInformation("{RecordsUpdated} records updated. ({UserId})", recordsUpdated, userId);
    
    await LogImportAttempt(userId, newPlays.Count, true, null);
  }
  
  private void SaveTrackInformation(FullTrack item)
  {
    Track track = new Track
    {
      Id = item.Id,
      AlbumId = item.Album.Id
    };

    dbContext.Tracks.AddIfNotExists(track,
      x => x.Id == item.Id);
    foreach (var artist in item.Artists)
    {
      ArtistTrack artistTrack = new ArtistTrack
      {
        TrackId = item.Id,
        ArtistId = artist.Id
      };
      dbContext.ArtistTracks.AddIfNotExists(artistTrack,
        x => x.ArtistId == artistTrack.ArtistId
             && x.TrackId == artistTrack.TrackId);
    }

    foreach (var artist in item.Album.Artists)
    {
      ArtistAlbum artistAlbum = new ArtistAlbum
      {
        ArtistId = artist.Id,
        AlbumId = item.Album.Id
      };
      dbContext.ArtistAlbums.AddIfNotExists(artistAlbum,
        x => x.AlbumId == artistAlbum.AlbumId
             && x.ArtistId == artistAlbum.ArtistId);
    }
  }

  private async Task LogImportAttempt(string userId, int tracksImported, bool isSuccessful, string errorMessage)
  {
    var importLog = new ImportLog
    {
      UserId = userId,
      ImportDateTime = DateTime.UtcNow,
      TracksImported = tracksImported,
      IsSuccessful = isSuccessful,
      ErrorMessage = errorMessage
    };

    dbContext.ImportLogs.Add(importLog);
    await dbContext.SaveChangesAsync();
  }

  private async Task CheckAndDisableUserIfNeeded(string userId)
  {
    var maxFailures = importSettings.Value.MaxConsecutiveFailures;
    
    var recentImports = dbContext.ImportLogs
      .Where(il => il.UserId == userId)
      .OrderByDescending(il => il.ImportDateTime)
      .Take(maxFailures)
      .ToList();

    if (recentImports.Count == maxFailures && recentImports.All(i => !i.IsSuccessful))
    {
      var user = await dbContext.Users.FindAsync(userId);
      if (user != null)
      {
        user.IsDisabled = true;
        await dbContext.SaveChangesAsync();
        logger.LogWarning("User {UserId} has been disabled due to {MaxFailures} consecutive import failures", userId, maxFailures);
      }
    }
  }

}