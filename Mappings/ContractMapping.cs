using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Models;

namespace AttendanceManagementApp.Mappings
{
    public class ContractMapping
    {
        private readonly EmployeeMapping _employeeMapping;

        public ContractMapping(EmployeeMapping employeeMapping)
        {
            _employeeMapping = employeeMapping;
        }

        public ContractRes ToContractRes(Contract contract)
        {
            if (contract == null) return null;
            Employee employee = contract.Employee;
           
            return new ContractRes
            {
                Id = contract.Id,
                ContractNumber = contract.ContractNumber,
                ContractType = contract.ContractType,
                StartDate = contract.StartDate,
                EndDate = contract.EndDate,
                BaseSalary = contract.BaseSalary,
                AllowancePark = contract.AllowancePark,
                Tax = contract.Tax,
                AllowanceLunchBreak = contract.AllowanceLunchBreak,
                WorkingPerMonth = contract.WorkingPerMonth,
                Status = contract.ContractStatus,
                Description = contract.Description,
                SignedDate = contract.SignedDate,
                Employee = employee != null ? _employeeMapping.ToEmployeeRes(employee) : null,
                TotalLeavingsPerMonth = contract.TotalLeavingsPerMonth,
                OverTimeRate = contract.OverTimeRate,
            };
        }
    }
}
