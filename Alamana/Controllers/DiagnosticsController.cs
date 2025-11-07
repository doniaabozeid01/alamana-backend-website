using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Alamana.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiagnosticsController : ControllerBase
    {
        [HttpGet("ping")]
        public IActionResult Ping() => Ok("OK");
    }
}
