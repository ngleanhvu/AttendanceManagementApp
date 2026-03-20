using AttendanceManagementApp.Models;

namespace AttendanceManagementApp.DTOs.Request
{
    public class AttendanceCheckOutReq
    {
        public int EmployeeId { get; set; }

        public string Note { get; set; }
    }
}
