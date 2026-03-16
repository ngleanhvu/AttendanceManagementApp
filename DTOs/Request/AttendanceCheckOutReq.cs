using AttendanceManagementApp.Models;

namespace AttendanceManagementApp.DTOs.Request
{
    public class AttendanceCheckOutReq
    {
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public int ShiftId { get; set; }
        public Shift Shift { get; set; }

        public string Note { get; set; }
    }
}
