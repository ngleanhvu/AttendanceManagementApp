namespace AttendanceManagementApp.DTOs.Response
{
    public class HolidayRes
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TotalDay { get; set; }
        public string Description { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
    }
}
