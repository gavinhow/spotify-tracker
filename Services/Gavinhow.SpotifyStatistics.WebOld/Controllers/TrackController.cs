using System.Linq;
using System.Threading.Tasks;
using Gavinhow.SpotifyStatistics.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> GetMultiple([FromQuery] string[] ids)
        {
            return Ok((await _spotifyApi.GetSeveralTracksAsync(ids.ToList())).Select(item => new { item.Id, item.Name, 
            ImageUrl = item.Album
            .Images[0].Url , 
            artists = item
            .Artists.Select(artist => new { artist.Id, artist.Name})}));
        }
    }
}