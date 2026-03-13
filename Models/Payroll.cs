namespace AttendanceManagementApp.Models
{
    public class Payroll: BaseEntity
    {
        public int Month { get; set; }

        public int Year { get; set; }

        public float TotalWorkDays { get; set; }

        public float TotalHours { get; set; }

        public float OvertimeHours { get; set; }

        public float GrossSalary { get; set; }

        public float NetSalary { get; set; }

        public DateTime CreatedDate { get; set; }

        public Employee Employee { get; set; }

        public ICollection<PayrollDetail> PayrollDetails { get; set; }
    }
}
