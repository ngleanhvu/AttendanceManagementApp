using AttendanceManagementApp.Configs;
using AttendanceManagementApp.DTOs.Request;
using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Models;
using AttendanceManagementApp.Repositories;
using AttendanceManagementApp.Services.Interface;

namespace AttendanceManagementApp.Services.Impl
{
    public class PayrollService : IPayrollService
    {
        private readonly IRepository<Payroll> _repository;
        private readonly AppDbContext _appDbContext;
        private readonly IEmployeeService _employeeService;
        private readonly IContractService _contractService;
        private readonly IAttendanceService _attendanceService;

        public PayrollService(IRepository<Payroll> repository, AppDbContext appDbContext, 
            IEmployeeService employeeService, IContractService contractService,
            IAttendanceService attendanceService)
        {
            _repository = repository;
            _appDbContext = appDbContext;
            _contractService = contractService;
            _employeeService = employeeService;
            _attendanceService = attendanceService;
        }

        public async Task<PayrollRes> CalculatePayrollAsyncTest(int employeeId, PayrollCalculateReq req)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(employeeId);
            var contractActive = await _contractService.GetContractActiveByEmployeeIdAsync(employeeId);
            // Get base salary, insurance, allowance, tax by contract
            var baseSalary = contractActive.BaseSalary;
            var allowancePark = contractActive.AllowancePark;
            var allowanceLunch = contractActive.AllowanceLunchBreak;
            var tax = contractActive.Tax;
            var workingPerMonth = contractActive.WorkingPerMonth;
            var totalLeavingPerMonth = contractActive.TotalLeavingsPerMonth;
            // calculate salary by day
            var baseSalaryPerDay = baseSalary / workingPerMonth;
            // calculate total hour base on check in and checkout from attendance model
            var workload = await _attendanceService.GetAttendanceWorkloadAsync(employeeId, req.Month, req.Year);
            var totalWorkingDays = workload.TotalWorkingDays;
            var overtimeHours = workload.OvertimeWorkingHours;
            var totalCheckInLate = workload.TotalCheckInLates;
            // calculate total leave request
        }
    }
}
