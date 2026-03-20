using AttendanceManagementApp.Models.Enum;

namespace AttendanceManagementApp.Models
{
    public enum AttendanceStatus
    {
        PRESENT = 1,
        LATE = 2
    }
    public class Attendance : BaseEntity
    {
        public DateOnly WorkDate { get; set; }

        public DateTime? CheckIn { get; set; }

        public DateTime? CheckOut { get; set; }
        public AttendanceStatus AttendanceStatus { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public string Note { get; set; }
    }
}
