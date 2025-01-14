using Gavinhow.SpotifyStatistics.Database;
using Gavinhow.SpotifyStatistics.Database.Entity;
using Gavinhow.SpotifyStatistics.Web.Authorization.Handler;
using Gavinhow.SpotifyStatistics.Web.CQRS;
using Gavinhow.SpotifyStatistics.Web.Types;
using Microsoft.EntityFrameworkCore;
using Friend = Gavinhow.SpotifyStatistics.Web.Types.Friend;

namespace Gavinhow.SpotifyStatistics.Web.Queries;

public class GetMeQuery(SpotifyStatisticsContext dbContext) : IQuery<GetMeQuery.Request, IQueryable<Me>>
{
  public record Request(string userId);

  public async Task<IQueryable<Me>> QueryAsync(Request request, CancellationToken cancellationToken)
  {
    if (request.userId == ApiKeyAuthenticationHandler.DEMO_USER)
    {
      return new List<Me>()
      {
        new Me()
        {
          Id = "gavinhow",
          Friends = new List<Friend>(),
        }
      }.AsQueryable();
    }
    return dbContext.Users.Where(x => x.Id == request.userId).Select(x => new Me
    {
      Id = request.userId,
      Friends = dbContext.Friends.Where(friend => friend.FriendId == request.userId).Select(friend => new Friend
      {
        Id = friend.UserId,
      }).ToList()
    });
  }
}