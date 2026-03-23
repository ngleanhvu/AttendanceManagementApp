namespace AttendanceManagementApp.Models
{
    public class OverTime : BaseEntity
    {
        public DateOnly  WorkDate { get; set; }
        public TimeOnly From { get; set; }
        public TimeOnly To { get; set; }
        public string Reason { get; set; }
        public bool IsApproved { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}