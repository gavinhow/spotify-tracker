using Gavinhow.SpotifyStatistics.Database;
using Gavinhow.SpotifyStatistics.Web.Authorization.Requirements;
using Gavinhow.SpotifyStatistics.Web.Extensions;
using HotChocolate.Resolvers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Gavinhow.SpotifyStatistics.Web.Authorization.Handler;

public class AllowedUserIdHandler(SpotifyStatisticsContext dbContext)
  : AuthorizationHandler<AllowedUserIdRequirement, IResolverContext>
{
  protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
    AllowedUserIdRequirement requirement, IResolverContext resolverContext)
  {
    string spotifyUsername = context.User.GetSpotifyUsername();
    string? resolverUserId = resolverContext.ArgumentValue<string?>("userId");
    if (resolverUserId is null || spotifyUsername == resolverUserId ||
        ValidDemoAccount(spotifyUsername, resolverUserId))
    {
      context.Succeed(requirement);
      return;
    }

    int friends = await dbContext.Friends.Where(x => x.FriendId == spotifyUsername && x.UserId == resolverUserId)
      .CountAsync();
    if (friends > 0)
      context.Succeed(requirement);
    else
    {
      context.Fail();
    }
  }

  private bool ValidDemoAccount(string spotifyUsername, string resolverUserId)
  {
    return spotifyUsername == ApiKeyAuthenticationHandler.DEMO_USER && resolverUserId == "gavinhow";
  }
}