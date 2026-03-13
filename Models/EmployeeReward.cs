namespace AttendanceManagementApp.Models
{
    public class EmployeeReward : BaseEntity
    {
        public float Amount { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }

        public string Note { get; set; }

        public Employee Employee { get; set; }

        public RewardType RewardType { get; set; }
    }
}
