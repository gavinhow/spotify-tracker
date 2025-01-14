using Gavinhow.SpotifyStatistics.Web.Types.DataLoaders;

namespace Gavinhow.SpotifyStatistics.Web.Types;

public record Play
{
  public string   TrackId    { get; set; }
  public DateTime TimeOfPlay { get; set; }

  public async Task<Track> Track(TrackBatchDataLoader dataLoader)
    => await dataLoader.LoadAsync(this.TrackId);
};