using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendLimpio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [Authorize]
        [HttpGet("protegido")]
        public IActionResult RutaProtegida()
        {
            return Ok("Acceso autorizado correctamente");
        }
    }
}
