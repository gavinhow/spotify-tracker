using Gavinhow.SpotifyStatistics.Api;
using Gavinhow.SpotifyStatistics.Database;
using Gavinhow.SpotifyStatistics.Database.Entity;
using SpotifyAPI.Web.Models;

namespace Gavinhow.SpotifyStatistics.Importer;

public class SpotifyImporter(SpotifyStatisticsContext dbContext, SpotifyApiFacade spotifyApiFacade, ILogger<SpotifyImporter> logger)
{
  public async Task ImportUserHistory()
  {
    var userIds = dbContext.Users.ToArray();
    await Task.WhenAll(userIds.Select(ImportUserHistory));
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
    foreach (PlayHistory item in await spotifyApiFacade.GetRecentlyPlayed(user.Id, newToken))
    {
      Play play = new Play
      {
        TrackId = item.Track.Id,
        UserId = userId,
        TimeOfPlay = item.PlayedAt
      };

      dbContext.Plays.AddIfNotExists(play,
        x => x.TrackId == item.Track.Id
             && x.UserId == userId
             && x.TimeOfPlay == item.PlayedAt);
    }

    int recordsUpdated = await dbContext.SaveChangesAsync();
    logger.LogInformation("{RecordsUpdated} records updated. ({UserId})", recordsUpdated, userId);
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
}