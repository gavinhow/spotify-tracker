namespace Gavinhow.SpotifyStatistics.Web.Types;

public record Album
{
  public required string                          Id          { get; set; }
  public required string                          Name        { get; set; }
  public required string                          ImageUrl    { get; set; }
  public required IReadOnlyList<SimplifiedArtist> Artists     { get; set; }
  public required IReadOnlyList<Track>            Tracks      { get; set; }
  public required string                          ReleaseDate { get; set; }
};