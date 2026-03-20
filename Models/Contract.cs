
using AttendanceManagementApp.Models.Enum;
using System.ComponentModel.DataAnnotations;
namespace AttendanceManagementApp.Models
{
    public class Contract : BaseEntity
    {
        [Required]
        public string ContractNumber { get; set; }
        [Required]
        public ContractType ContractType { get; set; }
        [Required]
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public decimal BaseSalary { get; set; }
        public decimal? AllowanceLunchBreak { get; set; }
        public decimal? AllowancePark {  get; set; }
        public decimal? InsuranceSalary { get; set; }
        public int TotalLeavingsPerMonth { get; set; }
        public decimal? Tax {  get; set; }
        public ContractStatus ContractStatus { get; set; }
        public string? Description { get; set; }
        public string? SignedBy { get; set; }
        public DateOnly? SignedDate { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}