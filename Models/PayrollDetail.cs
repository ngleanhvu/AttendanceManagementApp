namespace AttendanceManagementApp.Models
{
    public class PayrollDetail : BaseEntity
    {
        public int PayrollId { get; set; }
        public Payroll Payroll { get; set; }
        public PayrollDetailType Type { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public enum PayrollDetailType
    {
        ERNING = 1,
        OVERTIME = 2,
        DEDUCTION = 3,
        INSURANCE = 4,
        ALLOWANCE = 5,
        HOLIDAY = 6,
    }
}