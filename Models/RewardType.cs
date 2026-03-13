namespace AttendanceManagementApp.Models
{
    public class RewardType : BaseEntity
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public ICollection<EmployeeReward> EmployeeRewards { get; set; }
    }
}
