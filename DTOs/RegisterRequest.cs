namespace BackendLimpio.DTOs
{
    public class RegisterRequest
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Type { get; set; }

        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Dni { get; set; }
    }
}