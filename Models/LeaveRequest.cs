namespace AttendanceManagementApp.Models
{
    public class LeaveRequest : BaseEntity
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public float TotalDays { get; set; } 
        public string Reason { get; set; }
        public LeaveStatus LeaveStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? RejectReason { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public LeaveRequestType LeaveRequestType { get; set; }
    }

    public enum LeaveStatus
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3,
    }

    public enum LeaveRequestType
    {
        FULLDAY = 1,
        PART_DAY_AM = 2,
        PART_DAY_PM = 3,
    }
}