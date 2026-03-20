namespace AttendanceManagementApp.Models
{
    public class PayrollDetail : BaseEntity
    {
        public int PayrollId { get; set; }
        public Payroll Payroll { get; set; }

        public PayrollDetailType Type { get; set; }

        public string Description { get; set; }
        public float Amount { get; set; }

        // Liên kết optional (OT, Leave,...)
        public int? ReferenceId { get; set; }

        public DateTime CreatedDate { get; set; }
    }

    public enum PayrollDetailType
    {
        ERNING,    // Thu nhập
        DEDUCTION   // Khấu trừ
    }
}