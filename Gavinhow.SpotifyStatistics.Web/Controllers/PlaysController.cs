using System;
using System.Linq;
using System.Security.Claims;
using Gavinhow.SpotifyStatistics.Database;
using Gavinhow.SpotifyStatistics.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS;
using Microsoft.Extensions.Primitives;

namespace Gavinhow.SpotifyStatistics.Web.Controllers
{
    // [Authorize]
    [Route("[controller]")]
    public class PlaysController : Controller
    {
        private readonly SpotifyStatisticsContext _dbContext;
        private readonly IUserService _userService;

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
        public PlaysController(SpotifyStatisticsContext dbContext, IUserService userService)
        {
            _userService = userService;
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_dbContext.Plays.Where(play => play.UserId == UserId).ToList());
        }
        
        [HttpGet("count")]
        public IActionResult Count()
        {
            return Ok(_dbContext.Plays.Count(play => play.UserId == UserId));
        }

        [HttpGet("artists")]
        public IActionResult GetAllArtists()
        {
            var results = from a in _dbContext.ArtistTracks
                join p in _dbContext.Plays on a.TrackId equals p.TrackId
                where UserId == p.UserId
                group new {a, p} by new {a.ArtistId}
                into g
                select new
                {
                    g.Key.ArtistId,
                    FirstListen = g.Min(x => x.p.TimeOfPlay)
                };

            return Ok(results.ToList());
        }

        [HttpGet("PlaysByDay")]
        public IActionResult GetGroupedPlaybackHistory()
        {
            var results = _dbContext.Plays
                .Where(x => x.UserId == UserId)
                .GroupBy(x => x.TimeOfPlay.Date)
                .OrderBy(x => x.Key)
                .Select(x => new {Day = (DateTime) x.Key, Value = x.Count()});

            return Ok(results);
        }
    }
}