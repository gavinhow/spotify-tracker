using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Gavinhow.SpotifyStatistics.Api;
using Gavinhow.SpotifyStatistics.Database;
using Gavinhow.SpotifyStatistics.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gavinhow.SpotifyStatistics.Web.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class AlbumController : ControllerBase
    {
        private readonly SpotifyStatisticsContext _dbContext;
        private readonly IUserService _userService;
        private readonly SpotifyApiFacade _spotifyApi;

        public AlbumController(SpotifyStatisticsContext dbContext, IUserService userService,SpotifyApiFacade _spotifyApi)
        {
            _userService = userService;
            this._spotifyApi = _spotifyApi;
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
        public async Task<IActionResult> GetMultiple([FromQuery] string[] ids)
        {
            return Ok((await _spotifyApi.GetAlbumsAsync(ids.ToList())).Select(item => 
                new { item.Id, 
                    item.Name, 
                    artists = item.Artists
                            .Select(artist => new { artist.Id, artist.Name}), 
                    imageUrl = item.Images[0].Url, 
                    Tracks = item.Tracks
                            .Items.Select(track => 
                                new {id = track.Id, 
                                    track.Name, 
                                    artists = item.Artists.Select(artist => 
                                        new { artist.Id,
                                            artist.Name,})
                                }
                            ),
                    item.ReleaseDate
                }
                )
            );
        }
        
        
        
        [HttpGet("top")]
        public async Task<IActionResult> Top([FromQuery]DateTime? start, [FromQuery]DateTime? end, [FromQuery]int top=10)
        {
            start ??= DateTime.MinValue;
            end ??= DateTime.Now;

            return Ok(await _dbContext.Plays
                .Where(play => play.UserId == UserId && play.TimeOfPlay < end && play.TimeOfPlay > start)
                .GroupBy(item => item.Track.AlbumId)
                .Select(g => new {Id = g.Key, Count = g.Count()})
                .OrderByDescending(g => g.Count)
                .Take(top)
                .ToListAsync());
        }
    }
}