using AttendanceManagementApp.Models;

namespace AttendanceManagementApp.DTOs.Request
{
    public class OvertimeCreateReq
    {
        public DateOnly WorkDate { get; set; }
        public TimeOnly From { get; set; }
        public TimeOnly To { get; set; }
        public string Reason { get; set; }
        public bool IsApproved { get; set; }
        public int EmployeeId { get; set; }
        
    }
}
