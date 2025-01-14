using Gavinhow.SpotifyStatistics.Web.Types.DataLoaders;

namespace Gavinhow.SpotifyStatistics.Web.Types;

public record TopSong
{    
  [IsProjected(true)]
  public required string TrackId { get; set; }
  public int PlayCount { get; set; }
  public async Task<Track> Track(TrackBatchDataLoader dataLoader)
    => await dataLoader.LoadAsync(this.TrackId);
}