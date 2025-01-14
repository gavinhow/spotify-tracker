using Gavinhow.SpotifyStatistics.Api;
using Gavinhow.SpotifyStatistics.Api.Settings;
using Microsoft.Extensions.Options;

namespace Gavinhow.SpotifyStatistics.Web.Types.DataLoaders;

public class AlbumBatchDataLoader(
  SpotifyApiFacade spotifyApi,
  IOptions<SpotifySettings> spotifySettings,
  IBatchScheduler batchScheduler,
  DataLoaderOptions? options = null)
  : BatchDataLoader<string, Album>(batchScheduler, options)
{
  private readonly SpotifySettings _spotifySettings = spotifySettings.Value;

  protected override async Task<IReadOnlyDictionary<string, Album>> LoadBatchAsync(IReadOnlyList<string> keys,
    CancellationToken cancellationToken)
  {
    if (_spotifySettings.DisableMetadataFetching)
    {
      return keys.Select(id => new Album()
      {
        Id = id,
        Name = "Test album",
        ImageUrl = "https://i.scdn.co/image/ab67616d0000b273254c8a09649551438b20f4c0",
        Artists = new[]
        {
          new SimplifiedArtist
          {
            Id = "1234",
            Name = "Gavin",
          }
        },
        Tracks = new[]
        {
          new Track()
          {
            Id = "1234",
            Name = "Gavin",
            TrackNumber = 1,
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
              Id = id,
              Name = "Test album",
              ImageUrl = "https://i.scdn.co/image/ab67616d0000b273254c8a09649551438b20f4c0",
            }
          }
        },
        ReleaseDate = DateTime.Now.ToString()
      }).ToDictionary(x => x.Id);
    }

    return (await spotifyApi.GetAlbumsAsync(keys.ToList()))
      .Select(item => new Album
      {
        Id = item.Id,
        Name = item.Name,
        ImageUrl = item.Images[0].Url,
        Artists = item.Artists.Select(artist => new SimplifiedArtist
        {
          Id = artist.Id,
          Name = artist.Name,
        }).ToList(),
        Tracks = item.Tracks.Items.Select(track =>
          new Track
          {
            Id = track.Id,
            Name = track.Name,
            TrackNumber = track.TrackNumber,
            AlbumArtUrl = item.Images[0].Url,
            Artists = item.Artists.Select(artist =>
              new SimplifiedArtist()
              {
                Id = artist.Id,
                Name = artist.Name,
              }).ToList(),
            Album = new SimplifiedAlbum()
            {
              Id = item.Id,
              Name = item.Name,
              ImageUrl = item.Images[0].Url,
            }
          }
        ).ToList(),
        ReleaseDate = item.ReleaseDate
      }).ToDictionary(x => x.Id);
  }
}