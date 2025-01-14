namespace Gavinhow.SpotifyStatistics.Web.Types;

public record SimplifiedAlbum
{
  public required string Id       { get; init; }
  public required string Name     { get; init; }
  public required string ImageUrl { get; set; }
};