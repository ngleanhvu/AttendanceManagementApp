using System.ComponentModel.DataAnnotations;

namespace AttendanceManagementApp.DTOs.Request
{
    public class HolidayCreateReq
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        public DateOnly? Date { get; set; }
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Month is required")]
        public int Month { get; set; }
        [Required(ErrorMessage = "IsPaidHoliday is required")]
        public bool IsPaidHoliday { get; set; } = true;
        [Required(ErrorMessage = "AllowWork is required")]
        public bool AllowWork { get; set; } = false;
        [Required(ErrorMessage = "SalaryCoefficient is required")]
        public decimal SalaryCoefficient { get; set; } = 3.0m;
    }
}
