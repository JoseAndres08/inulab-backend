using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendLimpio.Models
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public Usuario? User { get; set; }

        [Required]
        public string ServiceName { get; set; } = string.Empty;

        [Required]
        public decimal Price { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public string? Comment { get; set; }

        public string? InvoicePdfUrl { get; set; }
        public string? ResultPdfUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? CompletedAt { get; set; }

        public List<OrderItem> Items { get; set; } = new();

        public Guid? MotorizadoId { get; set; }
        public Usuario? Motorizado { get; set; }

        public Guid? AddressId { get; set; }

        [ForeignKey("AddressId")]
        public Address? Address { get; set; }

        public double? MotoLat { get; set; }
        public double? MotoLng { get; set; }

        public bool RequiresSampling { get; set; } = false;
    }
}