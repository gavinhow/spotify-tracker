using Gavinhow.SpotifyStatistics.Api;
using Gavinhow.SpotifyStatistics.Api.Settings;
using Microsoft.Extensions.Options;

namespace Gavinhow.SpotifyStatistics.Web.Types.DataLoaders;

public class ArtistBatchDataLoader(
  SpotifyApiFacade spotifyApi,
  IOptions<SpotifySettings> spotifySettings,
  IBatchScheduler batchScheduler,
  DataLoaderOptions? options = null)
  : BatchDataLoader<string, Artist>(batchScheduler, options)
{
  private readonly SpotifySettings _spotifySettings = spotifySettings.Value;

  protected override async Task<IReadOnlyDictionary<string, Artist>> LoadBatchAsync(IReadOnlyList<string> keys,
    CancellationToken cancellationToken)
  {
    if (_spotifySettings.DisableMetadataFetching)
    {
      return keys.Select(id => new Artist()
      {
        Id = id,
        Name = "Test artist",
        ImageUrl = "https://i.scdn.co/image/ab67616d0000b273254c8a09649551438b20f4c0",
      }).ToDictionary(x => x.Id);
    }

    return (await spotifyApi.GetArtistsAsync(keys.ToList()))
      .Select(item => new Artist
      {
        Id = item.Id,
        Name = item.Name,
        ImageUrl = item.Images[0].Url,
      }).ToDictionary(x => x.Id);
  }
}