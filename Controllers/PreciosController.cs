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
                _context.ExamenesPrecio.Add(new ExamenPrecio
                {
                    ExamenId = examenId,
                    Nombre = dto.Nombre,
                    TipoUsuario = dto.TipoUsuario,
                    Precio = dto.Precio,
                    Especie = dto.Especie,
                    RequiereTomaMuestra = dto.RequiereTomaMuestra,
                    TiempoEntrega = dto.TiempoEntrega,
                    Descripcion = dto.Descripcion,
                    Categoria = dto.Categoria,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            else
            {
                existente.Precio = dto.Precio;
                if (!string.IsNullOrWhiteSpace(dto.Especie))
                    existente.Especie = dto.Especie;
                existente.RequiereTomaMuestra = dto.RequiereTomaMuestra;
                if (!string.IsNullOrWhiteSpace(dto.TiempoEntrega))
                    existente.TiempoEntrega = dto.TiempoEntrega;
                if (!string.IsNullOrWhiteSpace(dto.Descripcion))
                    existente.Descripcion = dto.Descripcion;
                if (!string.IsNullOrWhiteSpace(dto.Categoria))
                    existente.Categoria = dto.Categoria;
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
                        Especie = dto.Especie,
                        RequiereTomaMuestra = dto.RequiereTomaMuestra,
                        TiempoEntrega = dto.TiempoEntrega,
                        Descripcion = dto.Descripcion,
                        Categoria = dto.Categoria,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
                else
                {
                    existente.Precio = dto.Precio;
                    if (!string.IsNullOrWhiteSpace(dto.Especie))
                        existente.Especie = dto.Especie;
                    existente.RequiereTomaMuestra = dto.RequiereTomaMuestra;
                    if (!string.IsNullOrWhiteSpace(dto.TiempoEntrega))
                        existente.TiempoEntrega = dto.TiempoEntrega;
                    if (!string.IsNullOrWhiteSpace(dto.Descripcion))
                        existente.Descripcion = dto.Descripcion;
                    if (!string.IsNullOrWhiteSpace(dto.Categoria))
                        existente.Categoria = dto.Categoria;
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
        public string Especie { get; set; } = "ambos";
        public bool RequiereTomaMuestra { get; set; } = true;
        public string TiempoEntrega { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
    }
}