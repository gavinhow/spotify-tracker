using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;

namespace Gavinhow.SpotifyStatistics.Web.Controllers
{
    [Route("Home")]
    public class HomeController : Controller
    {
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpGet("Error")]
        public IActionResult Error()
        {
            var exceptionHandlerPathFeature =
                HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            return StatusCode(500, new
            {
                errorMessage = exceptionHandlerPathFeature.Error.Message
            });
        }


    }
}
