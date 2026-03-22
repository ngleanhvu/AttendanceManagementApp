namespace AttendanceManagementApp.DTOs.Request
{
    public class PayrollFilterReq
    {
        public int? Month { get; set; }
        public int? Year { get; set; }
        public int? Status { get; set; }
        public string? Keyword { get; set; }
        public decimal? From { get; set; }
        public decimal? To { get; set; }
    }
}
