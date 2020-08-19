using System;
using System.Linq;
using System.Security.Claims;
using Gavinhow.SpotifyStatistics.Api;
using Gavinhow.SpotifyStatistics.Database;
using Gavinhow.SpotifyStatistics.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gavinhow.SpotifyStatistics.Web.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class ArtistController : ControllerBase
    {
        private readonly SpotifyStatisticsContext _dbContext;
        private readonly IUserService _userService;
        private readonly SpotifyApiFacade _spotifyApi;

        public ArtistController(SpotifyStatisticsContext dbContext, IUserService userService,SpotifyApiFacade 
        spotifyApi)
        {
            _userService = userService;
            this._spotifyApi = spotifyApi;
            _dbContext = dbContext;
        }
        
        public string UserId
        {
            get
            {
                string authenticatedUserId = User.Claims.First(x => x.Type == ClaimTypes.Name).Value;
                var friendId =  Request.Headers["userId"].ToString();
                if (!string.IsNullOrEmpty(friendId) && friendId != authenticatedUserId)
                {
                    if (!_userService.CheckIsFriend(authenticatedUserId, friendId))
                    {
                        throw new Exception("Not authorised to see that users details");
                    }

                    return friendId;
                }
                return authenticatedUserId;
            }
        }
        
        [HttpGet]
        public IActionResult GetMultiple([FromQuery] string[] ids)
        {
            return Ok(_spotifyApi.GetArtists(ids.ToList()).Select(item => new { item.Id, item.Name }));
        }
        
        [HttpGet("top")]
        public IActionResult Top([FromQuery]DateTime? start, [FromQuery]DateTime? end, [FromQuery]int top=10)
        {
            start ??= DateTime.MinValue;
            end ??= DateTime.Now;
            
            var results = (from a in _dbContext.ArtistTracks
                join p in _dbContext.Plays on a.TrackId equals p.TrackId
                where UserId == p.UserId && p.TimeOfPlay < end && p.TimeOfPlay > start
                group new {a, p} by new {a.ArtistId}
                into g
                select new
                {
                    id = g.Key.ArtistId,
                    Count = g.Count()
                }).OrderByDescending(item => item.Count).Take(top);

            return Ok(results.ToList());
        }
    }
}