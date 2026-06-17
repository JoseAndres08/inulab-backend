using BackendLimpio.Datos;
using BackendLimpio.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
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
        private readonly Cloudinary _cloudinary;

        public FacturasController(InulaDbContext context)
        {
            _context = context;
            var account = new Account("dfuzlltmk", "216388186713139", "XvPdNDkcBJWFKbU2dke3DiYZk7Q");
            _cloudinary = new Cloudinary(account);
            _cloudinary.Api.Secure = true;
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
                    invoicePdf = f.PdfPath
                })
                .ToListAsync();

            return Ok(facturas);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [Consumes("multipart/form-data")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> PostFactura(
            [FromForm] IFormFile file,
            [FromForm] string? orderId,
            [FromForm] string? documentType)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No se envió archivo");

                if (string.IsNullOrEmpty(orderId))
                    return BadRequest("orderId es requerido");

                if (!Guid.TryParse(orderId, out var orderGuid))
                    return BadRequest("orderId inválido");

                using var stream = file.OpenReadStream();
                var uploadParams = new RawUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    PublicId = $"inulab/facturas/factura_{orderGuid}",
                    Overwrite = true
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                    return BadRequest($"Error Cloudinary: {uploadResult.Error.Message}");

                var pdfUrl = uploadResult.SecureUrl.ToString();

                var factura = new Factura
                {
                    OrderId = orderGuid,
                    TipoComprobante = documentType,
                    Total = 0,
                    Fecha = DateTime.UtcNow,
                    PdfPath = pdfUrl
                };

                _context.Facturas.Add(factura);
                await _context.SaveChangesAsync();

                var order = await _context.Orders.FindAsync(factura.OrderId);
                if (order != null)
                {
                    order.InvoicePdfUrl = pdfUrl;
                    await _context.SaveChangesAsync();
                }

                return Ok(new { factura.Id, pdfUrl });
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
            return Redirect(factura.PdfPath);
        }
    }
}