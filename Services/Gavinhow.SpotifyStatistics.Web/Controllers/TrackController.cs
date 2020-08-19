using System.Collections.Generic;
using System.Linq;
using Gavinhow.SpotifyStatistics.Api;
using Gavinhow.SpotifyStatistics.Database.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.Web.Models;

namespace Gavinhow.SpotifyStatistics.Web.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class TrackController : ControllerBase
    {
        private readonly SpotifyApiFacade _spotifyApi;

        public TrackController(SpotifyApiFacade _spotifyApi)
        {
            this._spotifyApi = _spotifyApi;
        }

        [HttpGet]
        public IActionResult GetMultiple([FromQuery] string[] ids)
        {
            return Ok(_spotifyApi.GetTracks(ids.ToList()).Select(item => new { item.Id, item.Name, artists = item.Artists.Select(artist => new { artist.Id, artist.Name})}));
        }
    }
}