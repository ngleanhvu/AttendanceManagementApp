using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Models;

namespace AttendanceManagementApp.Mappings
{
    public class PayrollMapping
    {
        private readonly EmployeeMapping _employeeMapping;

        public PayrollMapping(EmployeeMapping employeeMapping)
        {
            _employeeMapping = employeeMapping;
        }

        public PayrollRes ToPayrollRes(Payroll req)
        {
            if (req == null)
                return null;
            return new PayrollRes
            {
                Id = req.Id,
                Month = req.Month,
                Year = req.Year,
                TotalWorkingDaysInMonth = req.TotalWorkingDaysInMonth,
                ActualWorkingDays = req.ActualWorkingDays,
                TotalHours = req.TotalHours,
                OvertimeHours = req.OvertimeHours,
                BasicSalary = req.BasicSalary,
                Allowance = req.Allowance,
                Bonus = req.Bonus,
                Holidays = req.Holidays,
                Deduction = req.Deduction,
                Tax = req.Tax,
                Insurance = req.Insurance,
                GrossSalary = req.GrossSalary,
                NetSalary = req.NetSalary,
                PayrollStatus = req.PayrollStatus.ToString(),
                PaidDate = req.PaidDate,
                CreatedDate = req.CreatedDate,
                Note = req.Note,
                Employee = req.Employee != null ? _employeeMapping.ToEmployeeRes(req.Employee) : null,
            };
        }

        public FullPayrollDetailRes ToFullPayrollDetailRes (Payroll payroll)
        {
            if (payroll == null)
            {
                return null;
            }

            var payrollRes = ToPayrollRes(payroll);
            var payrollDetailsRes = payroll.PayrollDetails.Select(x => new PayrollDetailRes
            {
                Id = x.Id,
                Description = x.Description,
                Amount = x.Amount,
                Type = x.Type.ToString(),
            }).ToList();
            return new FullPayrollDetailRes
            {
                Payroll = payrollRes,
                PayrollList = payrollDetailsRes,
            };
        }
    }
}
