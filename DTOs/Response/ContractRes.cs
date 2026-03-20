using AttendanceManagementApp.Models;
using AttendanceManagementApp.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace AttendanceManagementApp.DTOs.Response
{
    public class ContractRes
    {
        public int Id { get; set; }
        public string ContractNumber { get; set; }
        public ContractType ContractType { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public decimal BaseSalary { get; set; }
        public decimal? AllowancePark { get; set; }
        public decimal? AllowanceLunchBreak { get; set; }
        public int WorkingPerMonth { get; set; }
        public ContractStatus Status { get; set; }
        public string? Description { get; set; }
        public string? SignedBy { get; set; }
        public DateOnly? SignedDate { get; set; }
        public EmployeeRes? Employee { get; set; }
        public decimal? Tax { get; set; }
        public int TotalLeavingsPerMonth { get; set; }
    }
}
