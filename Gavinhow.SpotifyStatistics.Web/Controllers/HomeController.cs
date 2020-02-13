using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Gavinhow.SpotifyStatistics.Web.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using Gavinhow.SpotifyStatistics.Database;
using SpotifyAPI.Web.Models;
using Gavinhow.SpotifyStatistics.Api;
using Gavinhow.SpotifyStatistics.Database.Entity;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
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
