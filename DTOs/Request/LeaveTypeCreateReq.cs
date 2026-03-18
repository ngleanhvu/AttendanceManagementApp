using System.ComponentModel.DataAnnotations;

namespace AttendanceManagementApp.DTOs.Request
{
    public class LeaveTypeCreateReq
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } // VD: Annual Leave, Sick Leave
        [Required(ErrorMessage ="Description is required")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Is paid is required")]
        public bool IsPaid { get; set; } // Có lương hay không
        [Required(ErrorMessage = "Max day year is required")]
        public int MaxDaysPerYear { get; set; } // Số ngày tối đa/năm
        [Required(ErrorMessage = "Max day month is required")]
        public int MaxDaysPerMonth { get; set; }
        public bool RequiresApproval { get; set; } // Có cần duyệt không
    }
}
