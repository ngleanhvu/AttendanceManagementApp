namespace AttendanceManagementApp.Models
{
    public class EmployeeShift : BaseEntity
    {
        public Employee Employee { get; set; }
        public Shift Shift { get; set; }

        public DateOnly FromDate { get; set; }  // Ngày bắt đầu áp dụng ca
        public DateOnly ToDate { get; set; }    // Ngày kết thúc áp dụng ca

        public bool IsActive { get; set; }      // Phân ca còn hiệu lực không

        public string? Note { get; set; }       // Ghi chú (ví dụ: tăng ca tạm thời)

        public int AssignedBy { get; set; }     // ID HR/Admin phân ca

        public DateTime AssignedAt { get; set; } // Thời điểm phân ca
        public int EmployeeId { get; set; }
    }
}