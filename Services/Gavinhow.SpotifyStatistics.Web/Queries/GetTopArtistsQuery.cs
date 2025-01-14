using Gavinhow.SpotifyStatistics.Database;
using Gavinhow.SpotifyStatistics.Web.CQRS;
using Gavinhow.SpotifyStatistics.Web.Types;
using GreenDonut.Selectors;
using Play = Gavinhow.SpotifyStatistics.Database.Entity.Play;

namespace Gavinhow.SpotifyStatistics.Web.Queries;

public class GetTopArtistsQuery(SpotifyStatisticsContext dbContext)
  : IQuery<GetTopArtistsQuery.Request, IQueryable<TopArtist>>
{
  public record Request(string UserId, DateTime? StartDate, DateTime? EndDate);

  public async Task<IQueryable<TopArtist>> QueryAsync(Request request, CancellationToken cancellationToken)
  {
    IQueryable<Play> query = dbContext.Plays.Where(row => row.UserId == request.UserId);

    if (request.StartDate.HasValue)
      query = query.Where(row => row.TimeOfPlay >= request.StartDate.Value);

    if (request.EndDate.HasValue)
      query = query.Where(row => row.TimeOfPlay <= request.EndDate.Value);

    return query.SelectMany(play => play.Track.Artists)
      .GroupBy(artist => artist.ArtistId)
      .Select(g => new TopArtist()
      {
        ArtistId = g.Key,
        PlayCount = g.Count()
      }).OrderByDescending(x => x.PlayCount)
      .ThenByDescending(x => x.ArtistId);
  }
}