using AttendanceManagementApp.Configs;
using AttendanceManagementApp.DTOs.Request;
using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Mappings;
using AttendanceManagementApp.Models;
using AttendanceManagementApp.Repositories;
using AttendanceManagementApp.Services.Interface;

namespace AttendanceManagementApp.Services.Impl
{
    public class PayrollService : IPayrollService
    {
        private readonly IRepository<Payroll> _repository;
        private readonly PayrollMapping _payrollMapping;
        private readonly AppDbContext _appDbContext;
        private readonly IEmployeeService _employeeService;
        private readonly IContractService _contractService;
        private readonly IAttendanceService _attendanceService;
        private readonly ILeaveRequestService _leaveRequestService;
        private readonly decimal FINE_CHECK_IN_LATE_AMOUNT = 200000;
        private readonly IHolidayService _holidayService;

        public PayrollService(IRepository<Payroll> repository, AppDbContext appDbContext, 
            IEmployeeService employeeService, IContractService contractService,
            IAttendanceService attendanceService, ILeaveRequestService leaveRequestService,
            PayrollMapping payrollMapping, IHolidayService holidayService
            )
        {
            _repository = repository;
            _appDbContext = appDbContext;
            _contractService = contractService;
            _employeeService = employeeService;
            _attendanceService = attendanceService;
            _leaveRequestService = leaveRequestService;
            _payrollMapping = payrollMapping;
            _holidayService = holidayService;
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
            var totalLeavingRequest = await _leaveRequestService.CalculateTotalLeavingAsync(employeeId,req.Month, req.Year);
            // calcaulate net and gross
            var hoilday = await _holidayService.TotalHolidayAsync(req.Month, req.Year);
            var realSalary = (totalWorkingDays + totalLeavingRequest
                <= contractActive.TotalLeavingsPerMonth ? totalLeavingRequest : 0 + hoilday)
                * baseSalaryPerDay;   
            var gross = realSalary + contractActive.OverTimeRate * (decimal)overtimeHours
                + contractActive.AllowanceLunchBreak
                + contractActive.AllowancePark - totalCheckInLate * FINE_CHECK_IN_LATE_AMOUNT;
            var net = realSalary - contractActive.Tax * gross;
            var payroll = new Payroll
            {
                Month = req.Month,
                Year = req.Year,
                Employee = employee,
                Insurance = 0,
                Tax = contractActive.Tax,
                TotalWorkingDaysInMonth = contractActive.WorkingPerMonth,
                ActualWorkingDays = totalWorkingDays,
                OvertimeHours = (decimal)overtimeHours,
                TotalHours = totalWorkingDays * 8,
                BasicSalary = realSalary,
                Allowance = contractActive.AllowancePark + contractActive.AllowanceLunchBreak,
                Bonus = 0,
                Holidays = hoilday,
                Deduction = totalCheckInLate*FINE_CHECK_IN_LATE_AMOUNT,
                GrossSalary = gross,
                NetSalary = net,
                PayrollStatus = PayrollStatus.PENDING,
                CreatedDate = DateTime.Now,
                Note = ""
            };
            await _repository.AddAsync(payroll);
            await _repository.SaveAsync();
            return _payrollMapping.ToPayrollRes(payroll);
        }
    }
}
