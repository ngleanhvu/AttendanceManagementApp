namespace AttendanceManagementApp.Models
{
    public class Payroll : BaseEntity
    {
        public int Month { get; set; }
        public int Year { get; set; }

        // Quan hệ
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public decimal Insurance { get; set; }

        // Thông tin công
        public int TotalWorkingDaysInMonth { get; set; }
        public decimal ActualWorkingDays { get; set; }
        public decimal TotalHours { get; set; }
        public decimal OvertimeHours { get; set; }

        // Lương
        public decimal BasicSalary { get; set; }
        public decimal Allowance { get; set; }
        public decimal Bonus { get; set; }
        public decimal Holidays { get; set; }
        public decimal Deduction { get; set; }

        // Thuế + bảo hiểm
        public decimal Tax { get; set; }

        // Tổng
        public decimal GrossSalary { get; set; }
        public decimal NetSalary { get; set; }

        // Trạng thái
        public PayrollStatus PayrollStatus { get; set; }
        public DateTime? PaidDate { get; set; }

        public DateTime CreatedDate { get; set; }
        public string? Note { get; set; }

        public ICollection<PayrollDetail> PayrollDetails { get; set; }
    }

    public enum PayrollStatus
    {
        PENDING,
        APPROVED,
        PAID
    }
}