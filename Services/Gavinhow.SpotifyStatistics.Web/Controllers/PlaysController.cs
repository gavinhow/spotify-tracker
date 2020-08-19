using System;
using System.Linq;
using System.Security.Claims;
using Gavinhow.SpotifyStatistics.Database;
using Gavinhow.SpotifyStatistics.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gavinhow.SpotifyStatistics.Web.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class PlaysController : ControllerBase
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
        public IActionResult GetMultiple([FromQuery] string[] ids)
        {
            return Ok(_dbContext.Plays.Where(play => play.UserId == UserId && ids.Contains(play.TrackId)).GroupBy(item => item.TrackId).Select(play
             => new {Id = play.Key, Count = play.Count()})
            .ToList());
        }
        
        [HttpGet("top")]
        public IActionResult Top([FromQuery]DateTime? start, [FromQuery]DateTime? end, [FromQuery]int top=10)
        {
            start ??= DateTime.MinValue;
            end ??= DateTime.Now;
            return Ok(_dbContext.Plays.Where(play => play.UserId == UserId && play.TimeOfPlay < end && play.TimeOfPlay > start)
                .GroupBy(item => item.TrackId)
                .Select(g => new {TrackId = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .Take(top)
                .ToList());
        }
        
        [HttpGet("count")]
        public IActionResult Count([FromQuery]DateTime? start, [FromQuery]DateTime? end)
        {
            start ??= DateTime.MinValue;
            end ??= DateTime.Now;
            return Ok(_dbContext.Plays.Count(play => play.UserId == UserId && play.TimeOfPlay < end && play.TimeOfPlay > start));
        }

        [HttpGet("artists")]
        public IActionResult GetAllArtists([FromQuery]DateTime? start, [FromQuery]DateTime? end)
        {
            start ??= DateTime.MinValue;
            end ??= DateTime.Now;
            var results = from a in _dbContext.ArtistTracks
                join p in _dbContext.Plays on a.TrackId equals p.TrackId
                where UserId == p.UserId && p.TimeOfPlay < end && p.TimeOfPlay > start
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