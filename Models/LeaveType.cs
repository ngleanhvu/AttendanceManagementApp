namespace AttendanceManagementApp.Models
{
    public class LeaveType : BaseEntity
    {
        public string Name { get; set; }

        public bool IsPaid { get; set; }

        public ICollection<LeaveRequest> LeaveRequests { get; set; }
    }
}
