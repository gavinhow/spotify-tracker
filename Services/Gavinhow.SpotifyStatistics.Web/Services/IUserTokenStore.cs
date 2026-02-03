namespace Gavinhow.SpotifyStatistics.Web.Services;

public interface IUserTokenStore
{
    /// <summary>
    /// Full upsert — called immediately when a user authenticates via OAuth
    /// so that Table Storage has the freshest tokens before the next import cycle.
    /// </summary>
    Task UpsertUserTokenAsync(string userId, string accessToken, string refreshToken, DateTimeOffset tokenExpiry, bool isDisabled);
}
