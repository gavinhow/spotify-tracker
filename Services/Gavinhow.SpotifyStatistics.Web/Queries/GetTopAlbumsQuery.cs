using Gavinhow.SpotifyStatistics.Database;
using Gavinhow.SpotifyStatistics.Web.CQRS;
using Gavinhow.SpotifyStatistics.Web.Types;
using GreenDonut.Selectors;
using Play = Gavinhow.SpotifyStatistics.Database.Entity.Play;

namespace Gavinhow.SpotifyStatistics.Web.Queries;

public class GetTopAlbumsQuery(SpotifyStatisticsContext dbContext): IQuery<GetTopAlbumsQuery.Request, IQueryable<TopAlbum>>
{
  public record Request(string UserId,string? ArtistId, DateTime? StartDate, DateTime? EndDate);

  public async Task<IQueryable<TopAlbum>> QueryAsync(Request request, CancellationToken cancellationToken)
  {
    IQueryable<Play> query = dbContext.Plays.Where(row => row.UserId == request.UserId);
    
    if(request.StartDate.HasValue)
      query = query.Where(row => row.TimeOfPlay >= request.StartDate.Value);
    
    if(request.EndDate.HasValue)
      query = query.Where(row => row.TimeOfPlay <= request.EndDate.Value);
    
    if (request.ArtistId is not null)
      query= query.Where(row => row.Track.Artists.Any(artist => artist.ArtistId == request.ArtistId));
    
    return query
      .GroupBy(play => play.Track.AlbumId)
      .Select(g => new TopAlbum()
      {
        AlbumId = g.Key,
        PlayCount = g.Count()
      }).OrderByDescending(x=> x.PlayCount).ThenByDescending(x=> x.AlbumId);
  }
}