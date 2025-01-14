using Gavinhow.SpotifyStatistics.Database;
using Gavinhow.SpotifyStatistics.Web.CQRS;
using Gavinhow.SpotifyStatistics.Web.Types;

namespace Gavinhow.SpotifyStatistics.Web.Queries;

public class GetArtistsByTrackQuery(SpotifyStatisticsContext dbContext)
  : IQuery<GetArtistsByTrackQuery.Request, IQueryable<ArtistByTrack>>
{
  public record Request(string TrackId);

  public async Task<IQueryable<ArtistByTrack>> QueryAsync(Request request, CancellationToken cancellationToken)
  {
    return dbContext.ArtistTracks
      .Where(row => row.TrackId == request.TrackId)
      .Select(row => new ArtistByTrack()
      {
        ArtistId = row.ArtistId
      });
  }
}