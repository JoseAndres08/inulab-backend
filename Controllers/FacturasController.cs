using BackendLimpio.Datos;
using BackendLimpio.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BackendLimpio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacturasController : ControllerBase
    {
        private readonly InulaDbContext _context;

        public FacturasController(InulaDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetFacturas()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);

            var query = _context.Facturas
                .Include(f => f.Order)
                .AsQueryable();

            if (role != "admin")
            {
                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized();

                var userId = Guid.Parse(userIdClaim);
                query = query.Where(f => f.Order != null && f.Order.UserId == userId);
            }

            var facturas = await query
                .OrderByDescending(f => f.Fecha)
                .Select(f => new
                {
                    f.Id,
                    f.OrderId,
                    f.Total,
                    documentType = f.TipoComprobante,
                    f.Fecha,
                    invoicePdf = f.PdfPath != null
                        ? $"http://localhost:7237{f.PdfPath}"
                        : null
                })
                .ToListAsync();

            return Ok(facturas);
        }

        [HttpPost]
        [AllowAnonymous]
        [Consumes("multipart/form-data")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> PostFactura([FromForm] IFormFile file, [FromForm] string orderId, [FromForm] string documentType)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No se envió archivo");

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "facturas");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = $"factura_{Guid.NewGuid()}.pdf";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var factura = new Factura
                {
                    OrderId = Guid.Parse(orderId),
                    TipoComprobante = documentType,
                    Total = 0,
                    Fecha = DateTime.UtcNow,
                    PdfPath = $"/facturas/{fileName}"
                };

                _context.Facturas.Add(factura);
                await _context.SaveChangesAsync();

                var order = await _context.Orders.FindAsync(factura.OrderId);
                if (order != null)
                {
                    order.InvoicePdfUrl = factura.PdfPath;
                    await _context.SaveChangesAsync();
                }

                return Ok(new { factura.Id, pdfUrl = factura.PdfPath });
            }
            catch (Exception ex)
            {
                return BadRequest("ERROR: " + ex.Message);
            }
        }

        [HttpGet("{id}/pdf")]
        public async Task<IActionResult> GetFacturaPdf(int id)
        {
            var factura = await _context.Facturas.FindAsync(id);
            if (factura == null) return NotFound();

            if (factura.PdfPath == null) return NotFound("Sin PDF");

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", factura.PdfPath.TrimStart('/'));
            if (!System.IO.File.Exists(filePath)) return NotFound("Archivo no encontrado");

            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(bytes, "application/pdf", $"factura_{factura.Id}.pdf");
        }
    }
}