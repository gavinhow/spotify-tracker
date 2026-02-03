using Gavinhow.SpotifyStatistics.Api.Models;

namespace Gavinhow.SpotifyStatistics.Web.Services;

public interface IPlaysQueueProcessor
{
    Task ProcessPlaysAsync(UserPlaysQueueMessage message, CancellationToken ct);
}
