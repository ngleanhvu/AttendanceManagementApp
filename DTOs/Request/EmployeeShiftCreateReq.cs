using System.ComponentModel.DataAnnotations;

namespace AttendanceManagementApp.DTOs.Request
{
    public class EmployeeShiftCreateReq
    {
        [Required(ErrorMessage = "Employee ID is required.")]
        public List<int> EmployeeIds { get; set; }
        [Required(ErrorMessage = "Shift ID is required.")]
        public int Shift { get; set; }
        [Required(ErrorMessage = "FromDate is required.")]
        public DateOnly FromDate { get; set; }
        [Required(ErrorMessage = "ToDate is required.")]
        public DateOnly ToDate { get; set; }
        [Required(ErrorMessage = "IsActive is required.")]
        public bool IsActive { get; set; }
        [Required(ErrorMessage = "IsDefault is required.")]
        public string? Note { get; set; }
        [Required(ErrorMessage = "AssignedBy is required.")]
        public int AssignedBy { get; set; }
        [Required(ErrorMessage = "AssignedAt is required.")]
        public DateTime AssignedAt { get; set; } 
    }
}
