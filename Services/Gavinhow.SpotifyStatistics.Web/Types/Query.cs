using System.Security.Claims;
using Gavinhow.SpotifyStatistics.Web.Authorization.Requirements;
using Gavinhow.SpotifyStatistics.Web.CQRS;
using Gavinhow.SpotifyStatistics.Web.Extensions;
using Gavinhow.SpotifyStatistics.Web.Queries;
using Gavinhow.SpotifyStatistics.Web.Types.DataLoaders;
using HotChocolate.Authorization;

namespace Gavinhow.SpotifyStatistics.Web.Types;

[QueryType]
public static class Query
{
  [UseSingleOrDefault]
  [UseProjection]
  public static async Task<IQueryable<Me>> GetMe(ClaimsPrincipal claimsPrincipal, IQuery<GetMeQuery.Request, IQueryable<Me>> query,
    CancellationToken cancellationToken)
  {
    return await query.QueryAsync(new GetMeQuery.Request(claimsPrincipal.GetSpotifyUsername()), cancellationToken);
  }
  
  [UsePaging(MaxPageSize = 50)]
  [UseProjection]
  [UseFiltering]
  [Authorize(Policy = AllowedUserIdRequirement.AllowedUserIdPolicy)]
  public static Task<IQueryable<Play>> GetPlays(string userId, DateTime? from, DateTime? to, IQuery<GetPlaysQuery.Request, IQueryable<Play>> query,
    CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(userId);
    return query.QueryAsync(new GetPlaysQuery.Request(userId, from, to), cancellationToken);
  }
  
  [UsePaging(MaxPageSize = 50)]
  [UseProjection]
  [UseFiltering]
  [Authorize(Policy = AllowedUserIdRequirement.AllowedUserIdPolicy)]
  public static Task<IQueryable<TopSong>> GetTopSongs(string userId, DateTime? from, DateTime? to, string? artistId, string? albumId,
    IQuery<GetTopSongsQuery.Request, IQueryable<TopSong>> query, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(userId);
    return query.QueryAsync(new GetTopSongsQuery.Request
    {
      UserId = userId,
      StartDate = from,
      EndDate = to,
      ArtistId = artistId,
      AlbumId = albumId,
    }, cancellationToken);
  }
  
  [UsePaging(MaxPageSize = 50)]
  [UseProjection]
  [UseFiltering]
  [Authorize(Policy = AllowedUserIdRequirement.AllowedUserIdPolicy)]
  public static Task<IQueryable<TopAlbum>> GetTopAlbums(string userId, string? artistId, DateTime? from, DateTime? to,
    IQuery<GetTopAlbumsQuery.Request, IQueryable<TopAlbum>> query, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(userId);
    return query.QueryAsync(new GetTopAlbumsQuery.Request(userId, artistId, from, to), cancellationToken);
  }

  [UsePaging(MaxPageSize = 50)]
  [UseProjection]
  [UseFiltering]
  [Authorize(Policy = AllowedUserIdRequirement.AllowedUserIdPolicy)]
  public static Task<IQueryable<TopArtist>> GetTopArtists(string userId, DateTime? from, DateTime? to,
    IQuery<GetTopArtistsQuery.Request, IQueryable<TopArtist>> query, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(userId);
    return query.QueryAsync(new GetTopArtistsQuery.Request(userId, from, to), cancellationToken);
  }
  
  public static Task<Artist> GetArtist(string artistId, ArtistBatchDataLoader dataLoader,
    CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(artistId);
    return dataLoader.LoadAsync(artistId, cancellationToken);
  }
  
  [UseProjection]
  [UseFiltering]
  public static Task<IQueryable<ArtistByAlbum>> GetArtistsByAlbum(string albumId, IQuery<GetArtistsByAlbumQuery.Request, IQueryable<ArtistByAlbum>> query,
    CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(albumId);
    return query.QueryAsync(new (albumId), cancellationToken);
  }
  
  [UseProjection]
  [UseFiltering]
  public static Task<IQueryable<ArtistByTrack>> GetArtistsByTrack(string trackId, IQuery<GetArtistsByTrackQuery.Request, IQueryable<ArtistByTrack>> query,
    CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(trackId);
    return query.QueryAsync(new (trackId), cancellationToken);
  }
  
  public static Task<Album> GetAlbum(string albumId, AlbumBatchDataLoader dataLoader,
    CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(albumId);
    return dataLoader.LoadAsync(albumId, cancellationToken);
  }
  
  public static Task<Track> GetTrack(string trackId, TrackBatchDataLoader dataLoader,
    CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(trackId);
    return dataLoader.LoadAsync(trackId, cancellationToken);
  }
  
  
}