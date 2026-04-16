using BackendLimpio.Datos;
using BackendLimpio.DTOs.Common;
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
    public class PetsController : ControllerBase
    {
        private readonly InulaDbContext _context;

        public PetsController(InulaDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetPets()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var pets = _context.Pets
                .Where(p => p.UserId == Guid.Parse(userId))
                .Select(p => new {
                    p.Id,
                    p.Name,
                    p.Species,
                    p.Breed,
                    p.Age,
                    p.Sex,
                    p.UserId,
                    exams = _context.OrderItems
                        .Include(i => i.Order)
                        .Where(i => i.PetId == p.Id)
                        .Select(i => new {
                            id = i.Id,
                            type = i.ExamName,
                            date = i.Order != null ? i.Order.CreatedAt.ToLocalTime() : DateTime.Now,
                            pdfUrl = i.PdfUrl
                        })
                        .ToList()
                })
                .ToList();

            return Ok(new { pets });
        }

        [HttpPost]
        public IActionResult CreatePet([FromBody] CreatePetDto dto)
        {
            if (dto == null)
                return BadRequest("Mascota inválida");

            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("El nombre es obligatorio");

            if (string.IsNullOrWhiteSpace(dto.Species))
                return BadRequest("La especie es obligatoria");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var pet = new Pet
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Species = dto.Species,
                Breed = dto.Breed,
                Age = dto.Age,
                Sex = dto.Sex,
                UserId = Guid.Parse(userId)
            };

            _context.Pets.Add(pet);
            _context.SaveChanges();

            return Ok(new
            {
                message = "Mascota creada correctamente",
                pet
            });
        }
    }
}