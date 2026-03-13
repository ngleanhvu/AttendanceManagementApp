namespace AttendanceManagementApp.Models
{
    public class Shift: BaseEntity
    {
        public string Name { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public int StandardHours { get; set; }

        public ICollection<EmployeeShift> EmployeeShifts { get; set; }

        public ICollection<Attendance> Attendances { get; set; }
    }
}
