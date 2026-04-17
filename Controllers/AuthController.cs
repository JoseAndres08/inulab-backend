using BackendLimpio.Datos;
using BackendLimpio.DTOs;
using BackendLimpio.Models;
using BackendLimpio.Servicios;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Security.Claims;

namespace BackendLimpio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly InulaDbContext _context;
        private readonly JwtServicio _jwtServicio;

        public AuthController(InulaDbContext context, JwtServicio jwtServicio)
        {
            _context = context;
            _jwtServicio = jwtServicio;
        }

        // ==========================
        // REGISTER
        // ==========================
        [HttpPost("registrarse")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            // Validar usuario o email duplicado
            var existe = await _context.Usuarios
                .AnyAsync(u => u.Username == request.Username || u.Email == request.Email);

            if (existe)
                return BadRequest("El usuario o email ya existe");

            // Validar roles permitidos
            var rolesValidos = new[] { "dueño", "medico", "admin", "motorizado", "clinica" };

            if (!rolesValidos.Contains(request.Type))
                return BadRequest("Rol inválido");

            // Validar DNI (Perú: 8 dígitos)
            if (!string.IsNullOrEmpty(request.Dni) && request.Dni.Length != 8)
                return BadRequest("DNI debe tener 8 dígitos");

            // Encriptar contraseña
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new Usuario
            {
                Id = Guid.NewGuid(),
                Username = request.Username ?? string.Empty,
                PasswordHash = hashedPassword,
                Type = request.Type ?? "dueño",
                Email = request.Email ?? string.Empty,
                Phone = request.Phone ?? string.Empty,
                Dni = request.Dni,
                Name = request.ClinicName ?? string.Empty,
                Clinic = request.ClinicName ?? string.Empty,
                ProfileDistrict = request.District ?? string.Empty,
                Ruc = request.Ruc ?? string.Empty
            };

            _context.Usuarios.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Usuario registrado correctamente");
        }

        // ==========================
        // LOGIN
        // ==========================
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var usuario = _context.Usuarios
                .FirstOrDefault(u => u.Username == request.Username);

            if (usuario == null)
                return Unauthorized("Usuario no encontrado");

            // Validar contraseña con BCrypt
            if (!BCrypt.Net.BCrypt.Verify(request.Password, usuario.PasswordHash))
                return Unauthorized("Contraseña incorrecta");

            var token = _jwtServicio.GenerarToken(usuario);

            return Ok(new
            {
                id = usuario.Id,
                userId = usuario.Id,
                username = usuario.Username,
                type = usuario.Type,
                email = usuario.Email,
                name = usuario.Name,
                clinic = usuario.Clinic,
                district = usuario.ProfileDistrict,
                ruc = usuario.Ruc,
                phone = usuario.Phone,
                role = User.FindFirst(ClaimTypes.Role)?.Value
            });
        }

        // ==========================
        // PERFIL USUARIO
        // ==========================
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (Guid.TryParse(userId, out var userIdGuid))
            {
                var usuario = await _context.Usuarios.FindAsync(userIdGuid);
                if (usuario != null)
                {
                    return Ok(new
                    {
                        token = token,   // ← ESTO FALTA
                        user = new
                        {
                            id = usuario.Id,
                            userId = usuario.Id,
                            username = usuario.Username,
                            type = usuario.Type,
                            email = usuario.Email,
                            name = usuario.Name,
                            clinic = usuario.Clinic,
                            district = usuario.ProfileDistrict,
                            ruc = usuario.Ruc,
                            phone = usuario.Phone
                        }
                    });
                }
            }

            return Unauthorized("Usuario no encontrado");
        }

        // ==========================
        // SOLO ADMIN
        // ==========================
        [Authorize(Roles = "admin")]
        [HttpGet("solo-admin")]
        public IActionResult SoloAdmin()
        {
            return Ok("Acceso exclusivo para administradores");
        }
    }
}