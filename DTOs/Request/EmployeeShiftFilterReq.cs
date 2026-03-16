namespace AttendanceManagementApp.DTOs.Request
{
    public class EmployeeShiftFilter
    {
        public int? EmployeeId { get; set; }

        public int? Month { get; set; }

        public int? Day { get; set; }

        public int? Year { get; set; }

        public DateOnly? FromDate { get; set; }

        public DateOnly? ToDate { get; set; }
    }
}
