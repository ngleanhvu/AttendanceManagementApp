
using AttendanceManagementApp.Models;
using AttendanceManagementApp.Models.Enum;

namespace AttendanceManagementApp.DTOs.Request
{
    public class AttendanceCheckInReq
    {
        public DateOnly WorkDate { get; set; }
        public int EmployeeId { get; set; }
        public int ShiftId { get; set; }
        public string Note { get; set; }
    }
}