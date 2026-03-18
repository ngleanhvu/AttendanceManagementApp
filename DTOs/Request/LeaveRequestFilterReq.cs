namespace AttendanceManagementApp.DTOs.Request
{
    public class LeaveRequestFilterReq
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? LeaveStatus { get; set; }
        public int? EmployeeId { get; set; }
        public int? LeaveTypeId { get; set; }
    }
}
