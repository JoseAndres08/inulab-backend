namespace BackendLimpio.DTOs.Responses
{
    public class MonthlyOrdersDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public int Total { get; set; }
    }
}