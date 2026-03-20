using AttendanceManagementApp.Models;

namespace AttendanceManagementApp.DTOs.Response
{
    public class PayrollRes
    {
        public int Id { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
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
        public decimal Insurance { get; set; }
        // Tổng
        public decimal GrossSalary { get; set; }
        public decimal NetSalary { get; set; }
        // Trạng thái
        public string PayrollStatus { get; set; }
        public DateTime? PaidDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? Note { get; set; }
        public EmployeeRes Employee {  get; set; }
    }
}
