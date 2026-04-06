namespace BackendLimpio.DTOs.Responses
{
    public class DashboardResponseDto
    {
        public int TotalOrders { get; set; }
        public int Pending { get; set; }
        public int Assigned { get; set; }
        public int InProcess { get; set; }
        public int Completed { get; set; }
        public int TodayOrders { get; set; }
    }
}