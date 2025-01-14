using Gavinhow.SpotifyStatistics.Api;
using Gavinhow.SpotifyStatistics.Api.Settings;
using Microsoft.Extensions.Options;

namespace Gavinhow.SpotifyStatistics.Web.Types.DataLoaders;

public class TrackBatchDataLoader(
  SpotifyApiFacade spotifyApi,
  IOptions<SpotifySettings> spotifySettings,
  IBatchScheduler batchScheduler,
  DataLoaderOptions? options = null)
  : BatchDataLoader<string, Track>(batchScheduler, options)
{
  private readonly SpotifySettings _spotifySettings = spotifySettings.Value;

  protected override async Task<IReadOnlyDictionary<string, Track>> LoadBatchAsync(
    IReadOnlyList<string> keys,
    CancellationToken cancellationToken)
  {
    if (_spotifySettings.DisableMetadataFetching)
    {
      return keys.Select((id, index) => new Track()
      {
        Id = id,
        Name = "Test Track",
        TrackNumber = index + 1,
        AlbumArtUrl = "https://i.scdn.co/image/ab67616d0000b273254c8a09649551438b20f4c0",
        Artists = new[]
        {
          new SimplifiedArtist
          {
            Id = "1234",
            Name = "Gavin",
          }
        },
        Album = new SimplifiedAlbum()
        {
          Id = "1234",
          Name = "Gavin",
          ImageUrl = "https://i.scdn.co/image/ab67616d0000b273254c8a09649551438b20f4c0",
        }
      }).ToDictionary(x => x.Id);
    }

    return (await spotifyApi.GetSeveralTracksAsync(keys.ToList()))
      .Select(item => new Track()
      {
        Id = item.Id,
        Name = item.Name,
        TrackNumber = item.TrackNumber,
        AlbumArtUrl = item.Album.Images[0].Url,
        Artists = item.Artists.Select(artist => new SimplifiedArtist
        {
          Id = artist.Id,
          Name = artist.Name,
        }).ToList(),
        Album = new SimplifiedAlbum()
        {
          Id = item.Album.Id,
          Name = item.Album.Name,
          ImageUrl = item.Album.Images[0].Url
        }
      }).ToDictionary(x => x.Id);
  }
}