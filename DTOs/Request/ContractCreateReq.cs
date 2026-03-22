using System.ComponentModel.DataAnnotations;

namespace AttendanceManagementApp.DTOs.Request
{
    public class ContractCreateReq
    {
        [Required(ErrorMessage = "Contract number is required")]
        public string ContractNumber { get; set; }

        [Required(ErrorMessage = "Contract type is required")]
        public int ContractType { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        [Required(ErrorMessage = "Base salary is required")]
        public decimal BaseSalary { get; set; }

        public decimal Allowance { get; set; }

        public int WorkingPerMonth { get; set; }

        [Required(ErrorMessage = "Contract status is required")]
        public int ContractStatus { get; set; }

        public string Description { get; set; }

        public string SignedBy { get; set; }

        [Required(ErrorMessage = "Signed date is required")]
        public DateOnly? SignedDate { get; set; }

        [Required(ErrorMessage = "Employee ID is required")]
        public int EmployeeId { get; set; }
        public decimal AllowancePark { get; set; }
        public decimal AllowanceLunchBreak { get; set; }
        public decimal Tax { get; set; }
        public int TotalLeavingsPerMonth { get; set; }
        public decimal OverTimeRate { get; set; }
    }
}
