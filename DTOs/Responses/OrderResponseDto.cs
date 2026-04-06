namespace BackendLimpio.DTOs.Responses
{
    public class OrderResponseDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? InvoicePdfUrl { get; set; }
        public string? ResultPdfUrl { get; set; }
        public Guid? AddressId { get; set; }
        public double? MotoLat { get; set; } // ← GPS
        public double? MotoLng { get; set; } // ← GPS
        public AddressDto? Address { get; set; }
        public UserSummaryDto? Motorizado { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class OrderItemDto
    {
        public string ExamName { get; set; } = string.Empty;
        public string AddressStreet { get; set; } = string.Empty;
        public string AddressDistrict { get; set; } = string.Empty;
        public string PetName { get; set; } = string.Empty;
        public string PetPhoto { get; set; } = string.Empty;
        public string PetOwner { get; set; } = string.Empty;
    }

    public class AddressDto
    {
        public Guid Id { get; set; }
        public string? Street { get; set; }
        public string? District { get; set; }
    }
}