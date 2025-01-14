using Gavinhow.SpotifyStatistics.Database;
using Gavinhow.SpotifyStatistics.Web.CQRS;
using Gavinhow.SpotifyStatistics.Web.Types;

namespace Gavinhow.SpotifyStatistics.Web.Queries;

public class GetArtistsByAlbumQuery(SpotifyStatisticsContext dbContext)
  : IQuery<GetArtistsByAlbumQuery.Request, IQueryable<ArtistByAlbum>>
{
  public record Request(string AlbumId);

  public async Task<IQueryable<ArtistByAlbum>> QueryAsync(Request request, CancellationToken cancellationToken)
  {
    return dbContext.ArtistAlbums
      .Where(row => row.AlbumId == request.AlbumId)
      .Select(row => new ArtistByAlbum
      {
        ArtistId = row.ArtistId
      });
  }
}