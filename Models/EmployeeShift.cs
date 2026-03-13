namespace AttendanceManagementApp.Models
{
    public class EmployeeShift: BaseEntity
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public Employee Employee { get; set; }

        public Shift Shift { get; set; }
    }
}
