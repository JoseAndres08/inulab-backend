namespace BackendLimpio.DTOs.Responses
{
    public class UserSummaryDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}