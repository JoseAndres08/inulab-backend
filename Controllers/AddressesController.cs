using BackendLimpio.Datos;
using BackendLimpio.Models;
using BackendLimpio.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackendLimpio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AddressesController : ControllerBase
    {
        private readonly InulaDbContext _context;

        public AddressesController(InulaDbContext context)
        {
            _context = context;
        }

        // 🔹 OBTENER DIRECCIONES DEL USUARIO
        [HttpGet]
        public IActionResult GetAddresses()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("Usuario no autenticado");

            if (!Guid.TryParse(userIdClaim, out Guid userId))
                return BadRequest("UserId inválido");

            var addresses = _context.Addresses
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.CreatedAt)
                .ToList();

            return Ok(addresses);
        }

        // 🔹 CREAR DIRECCIÓN
        [HttpPost]
        public IActionResult CreateAddress([FromBody] CreateAddressDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("Usuario no autenticado");

            if (!Guid.TryParse(userIdClaim, out Guid userId))
                return BadRequest("UserId inválido");

            var address = new Address
            {
                Id = Guid.NewGuid(),
                UserId = userId, // 🔥 SIEMPRE DEL TOKEN
                Name = dto.Name,
                Street = dto.Street,
                District = dto.District,
                Reference = dto.Reference,
                CreatedAt = DateTime.UtcNow
            };

            _context.Addresses.Add(address);
            _context.SaveChanges();

            return Ok(address);
        }
    }
}