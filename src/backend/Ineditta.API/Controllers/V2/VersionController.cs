using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace Ineditta.API.Controllers.V2
{
    [ApiController]
    [Route("v{version:apiVersion}/version")]
    [ApiVersion("2.0")]
    public class VersionController : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet]
        [OutputCache()]
        public IActionResult Index()
        {
            return Ok("API Version 2.0");
        }
    }
}
