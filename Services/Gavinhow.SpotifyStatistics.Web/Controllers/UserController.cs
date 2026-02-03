using System;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.Extensions.Options;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Threading.Tasks;
using Gavinhow.SpotifyStatistics.Api.Settings;
using Gavinhow.SpotifyStatistics.Web.Services;
using Gavinhow.SpotifyStatistics.Web.Settings;
using Microsoft.AspNetCore.Authorization;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;

namespace Gavinhow.SpotifyStatistics.Web.Controllers
{
  [ApiController]
  [Route("[controller]")]
  [Authorize]
  public class UserController : ControllerBase
  {
    private readonly IUserService _userService;
    private readonly SpotifySettings _spotifySettings;
    private readonly AppSettings _appSettings;

    public string UserId
    {
      get { return User.Claims.First(x => x.Type == ClaimTypes.Name).Value; }
    }

    public UserController(IOptions<SpotifySettings> spotifySettings, IUserService userService, IOptions<AppSettings>
        appSettings)
    {
      _userService = userService;
      _appSettings = appSettings.Value;
      _spotifySettings = spotifySettings.Value;
    }

    [AllowAnonymous]
    [HttpGet("Login")]
    public IActionResult Login()
    {
      AuthorizationCodeAuth auth =
          new AuthorizationCodeAuth(_spotifySettings.ClientId, _spotifySettings.ClientSecret,
              _spotifySettings.CallbackUri, _spotifySettings.ServerUri,
              Scope.UserReadRecentlyPlayed);

      return Redirect(auth.GetUri());
    }

    [AllowAnonymous]
    [HttpGet("authenticate")]
    public async Task<IActionResult> Authenticate(string code)
    {
      var user = await _userService.Authenticate(code);

      if (user == null)
        return Unauthorized();

      var tokenHandler = new JwtSecurityTokenHandler();
      var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(new Claim[]
          {
                    new Claim(ClaimTypes.Name, user.Id)
          }),
        Expires = DateTime.UtcNow.AddDays(7),
        Issuer = _appSettings.Issuer,
        Audience = _appSettings.Audience,
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
              SecurityAlgorithms.HmacSha256Signature)
      };
      var token = tokenHandler.CreateToken(tokenDescriptor);
      var tokenString = tokenHandler.WriteToken(token);

      // return basic user info (without password) and token to store client side
      return Ok(new
      {
        user.Id,
        user.DisplayName,
        Token = tokenString
      });
    }

    [HttpPost("Friends")]
    public IActionResult AddFriend([FromBody] FriendRequestBodyModel requestBodyModel)
    {
      return Ok(_userService.AddFriend(UserId, requestBodyModel.friendId));
    }

    [HttpDelete("Friends")]
    public IActionResult RemoveFriend([FromBody] FriendRequestBodyModel requestBodyModel)
    {
      _userService.RemoveFriend(UserId, requestBodyModel.friendId);
      return Ok();
    }


    [HttpGet("Friends")]
    public IActionResult Friends()
    {
      return Ok(_userService.GetFriends(UserId));
    }
    
    [HttpGet("DisplayName/{id}")]
    public IActionResult DisplayName(string id)
    {
      return Ok(_userService.GetDisplayName(id));
    }

    public class FriendRequestBodyModel
    {
      public string friendId { get; set; }
    }
  }
}
