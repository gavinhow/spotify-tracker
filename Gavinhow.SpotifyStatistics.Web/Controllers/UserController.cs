using System;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
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
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly UserService _userService;
        private readonly SpotifySettings _spotifySettings;
        private readonly AppSettings _appSettings;

        public UserController(IOptions<SpotifySettings> spotifySettings, UserService userService, IOptions<AppSettings>
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
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // return basic user info (without password) and token to store client side
            return Ok(new
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                Token = tokenString
            });
        }

        // [HttpGet("{id}")]
        // public IActionResult GetById(int id)
        // {
        //     var user =  _userService.GetById(id);
        //     var userDto = _mapper.Map<UserDto>(user);
        //     return Ok(userDto);
        // }
        //
        // [HttpPut("{id}")]
        // public IActionResult Update(int id, [FromBody]UserDto userDto)
        // {
        //     // map dto to entity and set id
        //     var user = _mapper.Map<User>(userDto);
        //     user.Id = id;
        //
        //     try
        //     {
        //         // save 
        //         _userService.Update(user, userDto.Password);
        //         return Ok();
        //     } 
        //     catch(AppException ex)
        //     {
        //         // return error message if there was an exception
        //         return BadRequest(ex.Message);
        //     }
        // }
        //
        // [HttpDelete("{id}")]
        // public IActionResult Delete(int id)
        // {
        //     _userService.Delete(id);
        //     return Ok();
        // }
    }
}