using BackendLimpio.Datos;
using BackendLimpio.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BackendLimpio.Controllers
{
    public class PostFacturaRequest
    {
        public IFormFile? file { get; set; }
        public string? orderId { get; set; }
        public string? documentType { get; set; }
    }

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
                        ? $"https://inulab-backend-production.up.railway.app/api/Facturas/{f.Id}/pdf"
                        : null
                })
                .ToListAsync();

            return Ok(facturas);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> PostFactura([FromForm] PostFacturaRequest request)
        {
            try
            {
                if (request.file == null || request.file.Length == 0)
                    return BadRequest("No se envió archivo");

                if (string.IsNullOrEmpty(request.orderId))
                    return BadRequest("orderId es requerido");

                if (!Guid.TryParse(request.orderId, out var orderGuid))
                    return BadRequest("orderId inválido");

                var folderPath = Path.Combine("/var/data", "facturas");
                Directory.CreateDirectory(folderPath);

                var fileName = $"factura_{Guid.NewGuid()}.pdf";
                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.file.CopyToAsync(stream);
                }

                var factura = new Factura
                {
                    OrderId = orderGuid,
                    TipoComprobante = request.documentType,
                    Total = 0,
                    Fecha = DateTime.UtcNow,
                    PdfPath = filePath
                };

                _context.Facturas.Add(factura);
                await _context.SaveChangesAsync();

                var order = await _context.Orders.FindAsync(factura.OrderId);
                if (order != null)
                {
                    order.InvoicePdfUrl = $"/api/Facturas/{factura.Id}/pdf";
                    await _context.SaveChangesAsync();
                }

                return Ok(new { factura.Id, pdfUrl = $"/api/Facturas/{factura.Id}/pdf" });
            }
            catch (Exception ex)
            {
                return BadRequest("ERROR: " + ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("{id}/pdf")]
        public async Task<IActionResult> GetFacturaPdf(int id)
        {
            var factura = await _context.Facturas.FindAsync(id);
            if (factura == null) return NotFound();
            if (factura.PdfPath == null) return NotFound("Sin PDF");

            if (!System.IO.File.Exists(factura.PdfPath))
                return NotFound("Archivo no encontrado");

            var bytes = await System.IO.File.ReadAllBytesAsync(factura.PdfPath);
            return File(bytes, "application/pdf", $"factura_{factura.Id}.pdf");
        }
    }
}