using AttendanceManagementApp.Models;
using System.ComponentModel.DataAnnotations;

namespace AttendanceManagementApp.DTOs.Request
{
    public class LeaveRequestCreateReq
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public float TotalDays { get; set; } // Tổng số ngày nghỉ
        public string Reason { get; set; }
        public int LeaveStatus { get; set; } // Pending, Approved, Rejected
        public DateTime CreatedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? RejectReason { get; set; }
        public int EmployeeId { get; set; }
        public int LeaveTypeId { get; set; }
    }
}
