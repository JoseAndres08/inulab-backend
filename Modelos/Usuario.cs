using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace BackendLimpio.Models
{
    [Table("Usuarios")] // 🔥 SOLUCIÓN CLAVE
    public class Usuario
    {
        public Guid Id { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public string Type { get; set; } = "dueño";

        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string? Dni { get; set; }

        public string? Clinic { get; set; }
        public string? Ruc { get; set; }
        public string? ProfileAddress { get; set; }
        public string? ProfileDistrict { get; set; }

        public int NameChanges { get; set; } = 0;

        public List<Pet> Pets { get; set; } = new();
        public List<Address> Addresses { get; set; } = new();
        public List<Order> Orders { get; set; } = new();
    }
}