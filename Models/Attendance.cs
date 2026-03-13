namespace AttendanceManagementApp.Models
{
    public class Attendance : BaseEntity
    {
        public DateTime WorkDate { get; set; }

        public DateTime? CheckIn { get; set; }

        public DateTime? CheckOut { get; set; }

        public float TotalHours { get; set; }

        public string Status { get; set; }

        public Employee Employee { get; set; }

        public Shift Shift { get; set; }
    }
}
