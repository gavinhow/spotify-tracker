using Gavinhow.SpotifyStatistics.Web.Types.DataLoaders;
using HotChocolate.Authorization;

namespace Gavinhow.SpotifyStatistics.Web.Types;

[Authorize]
public record Me
{
  [IsProjected(true)]
  public required string Id { get; set; }
  public async Task<string?> DisplayName(SpotifyUserProfileBatchDataLoader dataLoader) {
    return (await dataLoader.LoadAsync(Id))?.DisplayName;
  }

  public async Task<string?> ImageUrl(SpotifyUserProfileBatchDataLoader dataLoader) {
    return (await dataLoader.LoadAsync(Id))?.Images.FirstOrDefault()?.Url;
  }
  public required IReadOnlyList<Friend> Friends { get; set; }
};

public record Friend 
{
  [IsProjected(true)]
  public required string Id { get; set; }
  
  public async Task<string?> DisplayName(SpotifyUserProfileBatchDataLoader dataLoader) {
    return (await dataLoader.LoadAsync(Id))?.DisplayName;
  }
  
  public async Task<string?> ImageUrl(SpotifyUserProfileBatchDataLoader dataLoader) {
    return (await dataLoader.LoadAsync(Id))?.Images.FirstOrDefault()?.Url;
  }
};