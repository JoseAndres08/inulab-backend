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
        public double? MotoLat { get; set; }
        public double? MotoLng { get; set; }
        public string? Comment { get; set; }
        public string? StaffComment { get; set; }
        public bool RequiresSampling { get; set; } = false;
        public bool IsManual { get; set; } = false;
        public bool InvoiceRequested { get; set; } = false;
        public string DocumentType { get; set; } = "boleta";
        public AddressDto? Address { get; set; }
        public UserSummaryDto? Motorizado { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
        public List<StatusHistoryDto> StatusHistory { get; set; } = new();
    }

    public class StatusHistoryDto
    {
        public int Status { get; set; }
        public DateTime ChangedAt { get; set; }
    }

    public class OrderItemDto
    {
        public string ExamName { get; set; } = "";
        public string AddressStreet { get; set; } = "";
        public string AddressDistrict { get; set; } = "";
        public Guid? PetId { get; set; }
        public string PetName { get; set; } = "";
        public string PetPhoto { get; set; } = "";
        public string PetOwner { get; set; } = "";
        public string? PdfUrl { get; set; }
    }

    public class AddressDto
    {
        public Guid Id { get; set; }
        public string? Street { get; set; }
        public string? District { get; set; }
    }
}