using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Gavinhow.SpotifyStatistics.Web.Authorization.Handler;

public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
  public string ApiKey { get; set; }
}

public class ApiKeyAuthenticationHandler(
  IOptionsMonitor<ApiKeyAuthenticationOptions> options,
  ILoggerFactory logger,
  UrlEncoder encoder)
  : AuthenticationHandler<ApiKeyAuthenticationOptions>(options, logger, encoder)
{
  public const string DEMO_USER = "DEMO_USER";

  protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
  {
    if (!Request.Headers.TryGetValue("ApiKey", out var apiKeyValues))
    {
      return AuthenticateResult.Fail("Missing API Key");
    }

    var providedApiKey = apiKeyValues.FirstOrDefault();
    var expectedApiKey = Options.ApiKey;

    if (string.IsNullOrEmpty(providedApiKey) || providedApiKey != expectedApiKey)
    {
      return AuthenticateResult.Fail("Invalid API Key");
    }

    var claims = new[] { new Claim(ClaimTypes.Name, DEMO_USER) };

    var identity = new ClaimsIdentity(claims, Scheme.Name);
    var principal = new ClaimsPrincipal(identity);
    var ticket = new AuthenticationTicket(principal, Scheme.Name);

    return AuthenticateResult.Success(ticket);
  }
}