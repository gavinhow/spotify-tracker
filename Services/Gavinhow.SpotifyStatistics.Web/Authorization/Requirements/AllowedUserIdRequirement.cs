using Microsoft.AspNetCore.Authorization;

namespace Gavinhow.SpotifyStatistics.Web.Authorization.Requirements;

public class AllowedUserIdRequirement : IAuthorizationRequirement
{
  public const string AllowedUserIdPolicy = "AllowedUserId";
}