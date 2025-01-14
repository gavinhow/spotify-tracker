using Gavinhow.SpotifyStatistics.Api;
using Gavinhow.SpotifyStatistics.Api.Settings;
using Microsoft.Extensions.Options;
using SpotifyAPI.Web.Models;

namespace Gavinhow.SpotifyStatistics.Web.Types.DataLoaders;

public class SpotifyUserProfileBatchDataLoader(
  SpotifyApiFacade spotifyApi,
  IOptions<SpotifySettings> spotifySettings,
  IBatchScheduler batchScheduler,
  DataLoaderOptions? options = null)
  : BatchDataLoader<string, PublicProfile>(batchScheduler, options)
{
  private readonly SpotifySettings _spotifySettings = spotifySettings.Value;

  protected override async Task<IReadOnlyDictionary<string, PublicProfile>> LoadBatchAsync(
    IReadOnlyList<string> keys,
    CancellationToken cancellationToken)
  {
    if (_spotifySettings.DisableMetadataFetching)
    {
      return keys.Select(id => new PublicProfile()
      {
        Id = id,
        DisplayName = id,
        Images = new List<Image>()
        {
          new Image()
          {
            Height = 1,
            Width = 1,
            Url = "https://i.scdn.co/image/ab67616d0000b273254c8a09649551438b20f4c0"
          }
        }
      }).ToDictionary(x => x.Id);
    }

    List<Task<PublicProfile>> profileTasks = keys.Select(spotifyApi.GetPublicProfileAsync).ToList();

    PublicProfile[] profiles = await Task.WhenAll(profileTasks);
    return profiles.ToDictionary(x => x.Id);
  }
}