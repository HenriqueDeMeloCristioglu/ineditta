using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace Ineditta.API.Controllers.V1
{
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    public class VersionController : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet]
        [OutputCache()]
        public IActionResult Index()
        {
            return Ok("API Version 1.0");
        }
    }
}
