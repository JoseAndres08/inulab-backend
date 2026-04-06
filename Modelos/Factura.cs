using System;

namespace BackendLimpio.Models
{
    public class Factura
    {
        public int Id { get; set; }
        public Guid OrderId { get; set; }
        public Order? Order { get; set; }
        public decimal Total { get; set; }
        public string? TipoComprobante { get; set; }
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public string? PdfPath { get; set; }
    }
}