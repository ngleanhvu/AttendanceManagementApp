namespace AttendanceManagementApp.Models
{
    public class OverTime: BaseEntity
    {
        public DateTime WorkDate { get; set; }

        public float Hours { get; set; }

        public float Rate { get; set; }

        public Employee Employee { get; set; }
    }
}
