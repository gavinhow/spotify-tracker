using Gavinhow.SpotifyStatistics.Web.Types.DataLoaders;

namespace Gavinhow.SpotifyStatistics.Web.Types;

public record SimplifiedArtist
{
  public required string Id   { get; set; }
  public required string Name { get; set; }

}