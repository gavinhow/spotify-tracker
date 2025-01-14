namespace Gavinhow.SpotifyStatistics.Web.Types;

public record Artist
{
  public required string Id   { get; set; }
  public required string Name { get; set; }
  public required string ImageUrl { get; set; }
}