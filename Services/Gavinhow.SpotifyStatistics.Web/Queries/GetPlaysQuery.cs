using Gavinhow.SpotifyStatistics.Database;
using Gavinhow.SpotifyStatistics.Web.CQRS;
using Gavinhow.SpotifyStatistics.Web.Types;

namespace Gavinhow.SpotifyStatistics.Web.Queries;

public class GetPlaysQuery(SpotifyStatisticsContext context): IQuery<GetPlaysQuery.Request, IQueryable<Play>>
{
  public record Request(string UserId, DateTime? StartDate, DateTime? EndDate);
  
  public async Task<IQueryable<Play>> QueryAsync(Request request, CancellationToken cancellationToken)
  {
    IQueryable<Play> query = context.Plays
      .OrderByDescending(x => x.TimeOfPlay)
      .Where(row => row.UserId == request.UserId)
      .Select(row => new Play
      {
        TrackId = row.TrackId,
        TimeOfPlay = row.TimeOfPlay
      });  
    
    if(request.StartDate.HasValue)
      query = query.Where(row => row.TimeOfPlay >= request.StartDate.Value);
    
    if(request.EndDate.HasValue)
      query = query.Where(row => row.TimeOfPlay <= request.EndDate.Value);
    
    return query;
  }
}