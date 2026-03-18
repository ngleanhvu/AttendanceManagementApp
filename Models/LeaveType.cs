namespace AttendanceManagementApp.Models
{
    public class LeaveType : BaseEntity
    {
        public string Name { get; set; } // VD: Annual Leave, Sick Leave

        public string Description { get; set; }

        public bool IsPaid { get; set; } // Có lương hay không

        public int MaxDaysPerYear { get; set; } // Số ngày tối đa/năm
        public int MaxDaysPerMonth { get; set; }

        public bool RequiresApproval { get; set; } // Có cần duyệt không
    }
}