namespace BackendLimpio.DTOs
{
    public class CreateAddressDto
    {
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;

        public string District { get; set; } = string.Empty;

        public string Reference { get; set; } = string.Empty;
    }
}