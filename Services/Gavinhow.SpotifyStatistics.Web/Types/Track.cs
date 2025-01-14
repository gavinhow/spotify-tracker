using Gavinhow.SpotifyStatistics.Web.CQRS;
using Gavinhow.SpotifyStatistics.Web.Queries;
using Microsoft.EntityFrameworkCore;
using SpotifyAPI.Web.Models;

namespace Gavinhow.SpotifyStatistics.Web.Types;

public record Track
{
  public required string                          Id          { get; set; }
  public required string                          Name        { get; set; }
  public required int                             TrackNumber { get; set; }
  public required IReadOnlyList<SimplifiedArtist> Artists     { get; set; }
  public required string                          AlbumArtUrl { get; set; }
  public required SimplifiedAlbum                 Album       { get; set; }

  public async Task<int> PlayCount(string userId, DateTime? from, DateTime? to,
    IQuery<GetTopSongsQuery.Request, IQueryable<TopSong>> query,
    CancellationToken ct)
  {
    var track = await (await query.QueryAsync(new GetTopSongsQuery.Request
    {
      UserId = userId,
      StartDate = from,
      EndDate = to,
      TrackId = Id
    }, ct)).FirstOrDefaultAsync(cancellationToken: ct);

    return track?.PlayCount ?? 0;
  }
}