using BackendLimpio.Datos;
using BackendLimpio.DTOs.Common;
using BackendLimpio.DTOs.Responses;
using BackendLimpio.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BackendLimpio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly InulaDbContext _context;

        public OrdersController(InulaDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "cliente,medico,dueño")]
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var user = await _context.Usuarios.FindAsync(userId);

                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    ServiceName = user != null
                        ? (!string.IsNullOrEmpty(user.Name)
                            ? $"{user.Name} {user.LastName}".Trim()
                            : user.Username)
                        : "Cliente",
                    Price = request.Total > 0 ? request.Total : 10,
                    Status = OrderStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    AddressId = request.AddressId == Guid.Empty ? null : request.AddressId
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    ExamName = request.ExamName ?? "Examen general",
                    AddressId = request.AddressId == Guid.Empty ? null : request.AddressId,
                    PetId = request.PetId == Guid.Empty ? null : request.PetId
                };

                _context.OrderItems.Add(orderItem);
                await _context.SaveChangesAsync();

                return Ok(order);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest($"Error: {ex.Message} | {ex.InnerException?.Message}");
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetOrders(
            [FromQuery] OrderStatus? status,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var role = User.FindFirstValue(ClaimTypes.Role);

                Guid? userId = null;
                if (!string.IsNullOrEmpty(userIdClaim))
                    userId = Guid.Parse(userIdClaim);

                IQueryable<Order> query = _context.Orders
                    .Include(o => o.Address)
                    .Include(o => o.Motorizado)
                    .Include(o => o.User)
                    .Include(o => o.Items)
                        .ThenInclude(i => i.Address)
                    .Include(o => o.Items)
                        .ThenInclude(i => i.Pet);

                if (role == "motorizado" && userId.HasValue)
                    query = query.Where(o => o.MotorizadoId == userId.Value);
                else if (role == "cliente" || role == "medico" || role == "dueño")
                {
                    if (!userId.HasValue)
                        return Unauthorized("Usuario sin ID válido");
                    query = query.Where(o => o.UserId == userId.Value);
                }

                if (status.HasValue)
                    query = query.Where(o => o.Status == status.Value);

                var orders = await query
                    .OrderByDescending(o => o.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                string GetUserName(Order o)
                {
                    if (o.User == null)
                        return !string.IsNullOrEmpty(o.ServiceName) ? o.ServiceName : "Cliente";
                    if (!string.IsNullOrEmpty(o.User.Name))
                        return $"{o.User.Name} {o.User.LastName}".Trim();
                    return o.User.Username;
                }

                var result = orders.Select(o => new OrderResponseDto
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    ServiceName = GetUserName(o),
                    UserName = GetUserName(o),
                    Price = o.Price,
                    Status = (int)o.Status,
                    CreatedAt = o.CreatedAt,
                    CompletedAt = o.CompletedAt,
                    InvoicePdfUrl = o.InvoicePdfUrl,
                    ResultPdfUrl = o.ResultPdfUrl,
                    AddressId = o.AddressId,
                    MotoLat = o.MotoLat,
                    MotoLng = o.MotoLng,

                    Address = o.Address != null ? new AddressDto
                    {
                        Id = o.Address.Id,
                        Street = o.Address.Street ?? "N/A",
                        District = o.Address.District ?? "N/A"
                    } : null,

                    Motorizado = o.Motorizado == null ? null : new UserSummaryDto
                    {
                        Id = o.Motorizado.Id,
                        Username = o.Motorizado.Username,
                        Type = o.Motorizado.Type
                    },

                    Items = o.Items.Select(i => new OrderItemDto
                    {
                        ExamName = i.ExamName,
                        AddressStreet = i.Address?.Street ?? "Sin dirección",
                        AddressDistrict = i.Address?.District ?? "Distrito no especificado",
                        PetName = i.Pet?.Name ?? "",
                        PetPhoto = i.Pet?.Species == "perro" ? "🐶" :
                                   i.Pet?.Species == "gato" ? "🐱" :
                                   i.Pet?.Species == "ave" ? "🦜" :
                                   i.Pet?.Species == "conejo" ? "🐰" : "🐾",
                        PetOwner = ""
                    }).ToList()
                }).ToList();

                return Ok(new { orders = result });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetOrders: {ex.Message}");
                return StatusCode(500, $"{ex.Message} | Inner: {ex.InnerException?.Message}");
            }
        }

        [Authorize]
        [HttpGet("mine")]
        public async Task<IActionResult> MyOrders()
        {
            return await GetOrders(null);
        }

        [Authorize(Roles = "motorizado")]
        [HttpPut("{id}/location")]
        public async Task<IActionResult> UpdateLocation(Guid id, [FromBody] LocationRequest request)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound("Orden no encontrada");

            order.MotoLat = request.Lat;
            order.MotoLng = request.Lng;
            await _context.SaveChangesAsync();

            return Ok(new { lat = request.Lat, lng = request.Lng });
        }

        [Authorize]
        [HttpGet("{id}/location")]
        public async Task<IActionResult> GetLocation(Guid id)
        {
            var order = await _context.Orders
                .Where(o => o.Id == id)
                .Select(o => new { o.MotoLat, o.MotoLng })
                .FirstOrDefaultAsync();

            if (order == null) return NotFound("Orden no encontrada");
            if (order.MotoLat == null) return Ok(new { lat = (double?)null, lng = (double?)null });

            return Ok(new { lat = order.MotoLat, lng = order.MotoLng });
        }

        [Authorize(Roles = "admin")]
        [HttpPost("{id}/upload-result")]
        public async Task<IActionResult> UploadResult(Guid id, IFormFile file)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound("Orden no encontrada");

            var folderPath = Path.Combine("/var/data", "results");
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            var fileName = $"{id}.pdf";
            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            order.ResultPdfUrl = $"/api/Orders/{id}/result-pdf";
            if (order.Status != OrderStatus.Completed)
                order.Status = OrderStatus.ResultsUploaded;
            await _context.SaveChangesAsync();

            return Ok(new { message = "PDF subido correctamente", url = order.ResultPdfUrl });
        }

        [AllowAnonymous]
        [HttpGet("{id}/result-pdf")]
        public IActionResult GetResultPdf(Guid id)
        {
            var filePath = Path.Combine("/var/data", "results", $"{id}.pdf");

            if (!System.IO.File.Exists(filePath))
                return NotFound("PDF no encontrado");

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            Response.Headers["Content-Disposition"] = "inline; filename=resultado.pdf";
            return File(fileBytes, "application/pdf");
        }

        [Authorize(Roles = "admin,motorizado")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusRequest request)
        {
            try
            {
                var order = await _context.Orders.FindAsync(id);
                if (order == null) return NotFound("Orden no encontrada");

                var statusLower = request.Status.ToLower();

                OrderStatus newStatus = statusLower switch
                {
                    "pending" => OrderStatus.Pending,
                    "assigned" => OrderStatus.Assigned,
                    "moto_en_camino" => OrderStatus.MotoEnCamino,
                    "moto_arrived" => OrderStatus.MotoArrived,
                    "pickup_in_progress" => OrderStatus.PickupInProgress,
                    "sample_received" => OrderStatus.SampleReceived,
                    "arrived_at_lab" => OrderStatus.ArrivedAtLab,
                    "processing" => OrderStatus.Processing,
                    "results_uploaded" => OrderStatus.ResultsUploaded,
                    "completed" => OrderStatus.Completed,
                    _ => throw new ArgumentException($"Estado inválido: {request.Status}")
                };

                order.Status = newStatus;
                if (newStatus == OrderStatus.Completed)
                    order.CompletedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return Ok(new { message = "Estado actualizado", order });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR UpdateStatus] {ex.Message}");
                return BadRequest($"Error al actualizar estado: {ex.Message}");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id}/assign-motorizado")]
        public async Task<IActionResult> AssignMotorizado(Guid id, [FromBody] AssignMotorizadoRequest request)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound("Orden no encontrada");

            order.MotorizadoId = request.MotorizadoId;
            order.Status = OrderStatus.Assigned;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Motorizado asignado", order });
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound("Orden no encontrada");

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Orden eliminada" });
        }
    }

    public class CreateOrderRequest
    {
        public decimal Total { get; set; }
        public Guid AddressId { get; set; }
        public string? ExamName { get; set; }
        public Guid? PetId { get; set; }
    }

    public class UpdateStatusRequest
    {
        public required string Status { get; set; }
    }

    public class AssignMotorizadoRequest
    {
        public Guid MotorizadoId { get; set; }
    }

    public class LocationRequest
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }
}