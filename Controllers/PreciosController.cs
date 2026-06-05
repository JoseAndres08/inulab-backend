using BackendLimpio.Datos;
using BackendLimpio.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace BackendLimpio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PreciosController : ControllerBase
    {
        private readonly InulaDbContext _context;
        private readonly IMemoryCache _cache;
        private const string CACHE_KEY = "precios_catalogo";

        public PreciosController(InulaDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPrecios()
        {
            if (_cache.TryGetValue(CACHE_KEY, out List<ExamenPrecio>? cached) && cached != null)
                return Ok(cached);

            var precios = await _context.ExamenesPrecio.ToListAsync();

            _cache.Set(CACHE_KEY, precios, TimeSpan.FromMinutes(30));

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
            _cache.Remove(CACHE_KEY); // invalidar caché al actualizar
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
            _cache.Remove(CACHE_KEY); // invalidar caché al actualizar bulk
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