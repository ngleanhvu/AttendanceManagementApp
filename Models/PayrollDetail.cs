namespace AttendanceManagementApp.Models
{
    public class PayrollDetail : BaseEntity
    {
        public string Type { get; set; }

        public string Description { get; set; }

        public float Amount { get; set; }

        public Payroll Payroll { get; set; }
    }
}
