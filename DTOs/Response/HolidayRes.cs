namespace AttendanceManagementApp.DTOs.Response
{
    public class HolidayRes
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateOnly? Date { get; set; }
        public string Description { get; set; }
        public int Month { get; set; }
        public bool IsPaidHoliday { get; set; } = true;
        public bool AllowWork { get; set; } = false;
        public decimal SalaryCoefficient { get; set; } = 3.0m;
    }
}
