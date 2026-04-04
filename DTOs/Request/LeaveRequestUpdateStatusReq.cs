namespace AttendanceManagementApp.DTOs.Request
{
    public class LeaveRequestUpdateStatusReq
    {
        public int LeaveStatus {  get; set; }
        public string? Reason { get; set; }
        public string? RejectReason { get; set; }
    }
}
