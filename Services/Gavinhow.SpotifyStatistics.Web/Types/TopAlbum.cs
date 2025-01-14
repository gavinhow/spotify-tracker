using Gavinhow.SpotifyStatistics.Web.Types.DataLoaders;

namespace Gavinhow.SpotifyStatistics.Web.Types;

public class TopAlbum
{
  [IsProjected(true)]
  public string AlbumId { get; set; }
  public int    PlayCount { get; set; }
  
  public async Task<Album> Album(AlbumBatchDataLoader dataLoader) => await dataLoader.LoadAsync(AlbumId);
  
}