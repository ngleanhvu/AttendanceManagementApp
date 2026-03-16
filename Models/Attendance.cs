using AttendanceManagementApp.Models.Enum;

namespace AttendanceManagementApp.Models
{
    public class Attendance : BaseEntity
    {
        public DateOnly WorkDate { get; set; }

        public DateTime? CheckIn { get; set; }

        public DateTime? CheckOut { get; set; }

        public AttendanceStatus Status { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        
        public Shift Shift { get; set; }

        public int LateMinutes { get; set; }

        public int EarlyLeaveMinutes { get; set; }

        public string Note { get; set; }
    }
}
