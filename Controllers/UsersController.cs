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

        [HttpGet("admins")]
        [Authorize]
        public async Task<IActionResult> GetAdmins()
        {
            var admins = await _context.Usuarios
                .Where(u => u.Type == "admin")
                .Select(u => new { u.Id, u.Username })
                .ToListAsync();
            return Ok(admins);
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

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _context.Usuarios
                .Where(u => u.Id == id)
                .Select(u => new { u.Id, u.Username, u.Phone, u.Email, u.Name, u.LastName, u.Type })
                .FirstOrDefaultAsync();

            if (user == null) return NotFound();

            return Ok(user);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _context.Usuarios.FindAsync(id);
            if (user == null) return NotFound();
            _context.Usuarios.Remove(user);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Usuario eliminado" });
        }
    }
}