namespace AttendanceManagementApp.DTOs.Request
{
    public class LeaveRequestCreateReq
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Reason { get; set; }
        public int EmployeeId { get; set; }
        public int LeaveTypeId { get; set; }
        public int LeaveRequestTypeId { get; set; }
    }
}
