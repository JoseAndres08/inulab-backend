using BackendLimpio.Models;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendLimpio.Models
{
    public class OrderItem
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public Order? Order { get; set; }

        public string ExamName { get; set; } = string.Empty;

        public string? PdfUrl { get; set; }

        // 🔥 NUEVO
        public Guid? AddressId { get; set; }

        [ForeignKey("AddressId")]
        public Address? Address { get; set; }

        public Guid? PetId { get; set; }

        [ForeignKey("PetId")]
        public Pet? Pet { get; set; }
    }
}