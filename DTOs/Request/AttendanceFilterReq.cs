namespace AttendanceManagementApp.DTOs.Request
{
    public class AttendanceFilterReq
    {
        public DateTime? CheckIn { get; set; }

        public DateTime? CheckOut { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public int? AttendanceStatus { get; set; }
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
        public int? EmployeeId { get; set; }
    }
}
