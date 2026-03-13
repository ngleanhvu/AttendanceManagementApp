namespace AttendanceManagementApp.Models
{
    public class LeaveRequest: BaseEntity
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string Reason { get; set; }

        public DateTime CreatedDate { get; set; }

        public Employee Employee { get; set; }

        public LeaveType LeaveType { get; set; }
    }
}
