using BackendLimpio.Datos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendLimpio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly InulaDbContext _context;

        public UsersController(InulaDbContext context)
        {
            _context = context;
        }

        [HttpGet("doctors")]
        public async Task<IActionResult> GetDoctors()
        {
            var doctors = await _context.Usuarios
                .Where(u => u.Type == "doctor")
                .Select(u => new { u.Id, u.Username })
                .ToListAsync();
            return Ok(doctors);
        }

        [HttpGet("motorizados")]
        public async Task<IActionResult> GetMotorizados()
        {
            var motorizados = await _context.Usuarios
                .Where(u => u.Type == "motorizado")
                .Select(u => new { u.Id, u.Username, u.Email })
                .ToListAsync();
            return Ok(motorizados);
        }
    }
}