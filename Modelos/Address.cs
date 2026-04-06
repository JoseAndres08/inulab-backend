using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendLimpio.Models
{
    public class Address
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public Usuario? User { get; set; }

        public string Name { get; set; } = string.Empty;

        [Required]
        public string Street { get; set; } = string.Empty;

        public string District { get; set; } = string.Empty;

        public string Reference { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}