using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendLimpio.Models
{
    public class OrderStatusHistory
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid OrderId { get; set; }

        [ForeignKey("OrderId")]
        public Order? Order { get; set; }

        [Required]
        public OrderStatus PreviousStatus { get; set; }

        [Required]
        public OrderStatus NewStatus { get; set; }

        [Required]
        public Guid ChangedByUserId { get; set; }

        public string? ChangedByUsername { get; set; }

        public string? ChangedByRole { get; set; }

        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    }
}