using Azure.Data.Tables;
using Gavinhow.SpotifyStatistics.Api.Models;

namespace Gavinhow.SpotifyStatistics.Web.Services;

public class UserTokenStore : IUserTokenStore
{
    private readonly TableClient _tableClient;

    public UserTokenStore(TableClient tableClient)
    {
        _tableClient = tableClient;
    }

    public async Task UpsertUserTokenAsync(string userId, string accessToken, string refreshToken, DateTimeOffset tokenExpiry, bool isDisabled)
    {
        var entity = new UserTokenTableEntity(userId)
        {
            SpotifyAccessToken = accessToken,
            SpotifyRefreshToken = refreshToken,
            TokenExpiry = tokenExpiry,
            LastSynced = DateTimeOffset.UtcNow,
            IsDisabled = isDisabled
        };

        await _tableClient.UpsertEntityAsync(entity);
    }
}

/// <summary>
/// Registered when Table Storage is not configured.  Lets the rest of the
/// application start normally without a real storage back-end.
/// </summary>
public class NoOpUserTokenStore : IUserTokenStore
{
    public Task UpsertUserTokenAsync(string userId, string accessToken, string refreshToken, DateTimeOffset tokenExpiry, bool isDisabled)
        => Task.CompletedTask;
}
