using AttendanceManagementApp.Models;

namespace AttendanceManagementApp.DTOs.Response
{
    public class LeaveRequestRes
    {
        public int Id { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public float TotalDays { get; set; } // Tổng số ngày nghỉ
        public string Reason { get; set; }
        public LeaveStatus LeaveStatus { get; set; } // Pending, Approved, Rejected
        public DateTime CreatedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? RejectReason { get; set; }
        public EmployeeRes Employee { get; set; }
        public LeaveTypeRes LeaveType { get; set; }
    }
}
