using Gavinhow.SpotifyStatistics.Web.Types.DataLoaders;

namespace Gavinhow.SpotifyStatistics.Web.Types;

public record ArtistByTrack
{
  public required string ArtistId { get; init; }
  public async Task<Artist> Artist(ArtistBatchDataLoader dataLoader) => await dataLoader.LoadAsync(ArtistId);  
};