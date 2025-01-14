using Gavinhow.SpotifyStatistics.Database;
using Gavinhow.SpotifyStatistics.Web.CQRS;
using Gavinhow.SpotifyStatistics.Web.Types;
using Play = Gavinhow.SpotifyStatistics.Database.Entity.Play;

namespace Gavinhow.SpotifyStatistics.Web.Queries;

public class GetTopSongsQuery(SpotifyStatisticsContext dbContext): IQuery<GetTopSongsQuery.Request, IQueryable<TopSong>>
{
  public record Request
  {
    public required string    UserId    { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate   { get; init; }
    public string?   ArtistId  { get; init; }
    public string?   AlbumId   { get; init; }
    public string?   TrackId   { get; init; }
  }

  public async Task<IQueryable<TopSong>> QueryAsync(Request request, CancellationToken cancellationToken)
  {
    IQueryable<Play> query = dbContext.Plays.Where(row => row.UserId == request.UserId);
    
    if(request.StartDate.HasValue)
      query = query.Where(row => row.TimeOfPlay >= request.StartDate.Value);
    
    if(request.EndDate.HasValue)
      query = query.Where(row => row.TimeOfPlay <= request.EndDate.Value);
    
    if (request.ArtistId is not null)
      query = query.Where(row => row.Track.Artists.Any(artistTrack => artistTrack.ArtistId == request.ArtistId));
    
    if (request.AlbumId is not null)
      query = query.Where(row => row.Track.AlbumId == request.AlbumId);
    
    if (request.TrackId is not null)
      query = query.Where(row => row.TrackId == request.TrackId);

    return query
      .GroupBy(play => play.TrackId)
      .Select(g => new TopSong()
      {
        TrackId = g.Key,
        PlayCount = g.Count()
      }).OrderByDescending(x=> x.PlayCount)
      .ThenByDescending(x=> x.TrackId);
  }
}