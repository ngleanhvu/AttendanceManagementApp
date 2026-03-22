using System.ComponentModel.DataAnnotations;

namespace AttendanceManagementApp.DTOs.Request
{
    public class HolidayCreateReq
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        public int TotalDay { get; set; }
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Month is required")]
        public int Month { get; set; }
        [Required(ErrorMessage = "IsPaidHoliday is required")]
        public int Year { get; set; }
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
    }
}
