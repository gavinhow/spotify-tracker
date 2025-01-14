using System.Security.Claims;

namespace Gavinhow.SpotifyStatistics.Web.Extensions;

public static class ClaimsPrincipleExtensions
{
  public static string GetSpotifyUsername(this ClaimsPrincipal claimsPrincipal)
  {
    return claimsPrincipal.Claims.First(x => x.Type == ClaimTypes.Name).Value;
  }
}