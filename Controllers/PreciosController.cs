using BackendLimpio.Datos;
using BackendLimpio.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BackendLimpio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PreciosController : ControllerBase
    {
        private readonly InulaDbContext _context;

        public PreciosController(InulaDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetPrecios()
        {
            var precios = await _context.ExamenesPrecio.ToListAsync();
            return Ok(precios);
        }

        [HttpPut("{examenId}")]
        public async Task<IActionResult> UpdatePrecio(string examenId, [FromBody] UpdatePrecioDto dto)
        {
            var existente = await _context.ExamenesPrecio
                .FirstOrDefaultAsync(e => e.ExamenId == examenId && e.TipoUsuario == dto.TipoUsuario);

            if (existente == null)
            {
                var nuevo = new ExamenPrecio
                {
                    ExamenId = examenId,
                    Nombre = dto.Nombre,
                    TipoUsuario = dto.TipoUsuario,
                    Precio = dto.Precio,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.ExamenesPrecio.Add(nuevo);
            }
            else
            {
                existente.Precio = dto.Precio;
                existente.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return Ok(new { examenId, precio = dto.Precio });
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> BulkUpdate([FromBody] List<UpdatePrecioDto> dtos)
        {
            foreach (var dto in dtos)
            {
                var existente = await _context.ExamenesPrecio
                    .FirstOrDefaultAsync(e => e.ExamenId == dto.ExamenId && e.TipoUsuario == dto.TipoUsuario);

                if (existente == null)
                {
                    _context.ExamenesPrecio.Add(new ExamenPrecio
                    {
                        ExamenId = dto.ExamenId,
                        Nombre = dto.Nombre,
                        TipoUsuario = dto.TipoUsuario,
                        Precio = dto.Precio,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
                else
                {
                    existente.Precio = dto.Precio;
                    existente.UpdatedAt = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { updated = dtos.Count });
        }
    }

    public class UpdatePrecioDto
    {
        public string ExamenId { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string TipoUsuario { get; set; } = "medico";
        public decimal Precio { get; set; }
    }
}
