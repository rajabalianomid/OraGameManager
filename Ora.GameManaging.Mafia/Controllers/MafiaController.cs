using Microsoft.AspNetCore.Mvc;

namespace Ora.GameManaging.Mafia.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MafiaController : ControllerBase
    {
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("Mafia WebAPI is running.");
        }
    }
}