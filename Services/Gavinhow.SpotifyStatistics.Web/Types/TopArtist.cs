using Gavinhow.SpotifyStatistics.Web.Types.DataLoaders;

namespace Gavinhow.SpotifyStatistics.Web.Types;

public record TopArtist
{
  [IsProjected(true)]
  public string ArtistId  { get; set; }
  public int    PlayCount { get; set; }
  public async Task<Artist> Artist(ArtistBatchDataLoader dataLoader) 
    => await dataLoader.LoadAsync(ArtistId);
}