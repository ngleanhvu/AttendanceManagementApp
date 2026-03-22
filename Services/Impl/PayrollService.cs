using AttendanceManagementApp.Configs;
using AttendanceManagementApp.DTOs.Request;
using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Exception;
using AttendanceManagementApp.Mappings;
using AttendanceManagementApp.Models;
using AttendanceManagementApp.Repositories;
using AttendanceManagementApp.Services.Interface;
using AttendanceManagementApp.Utils;
using Microsoft.EntityFrameworkCore;

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

        public async Task CalculatePayrollAsync(PayrollCalculateReq req)
        {
            using var transaction = await _appDbContext.Database.BeginTransactionAsync();

            try
            {
                var employees = await _employeeService.GetAllEmployeeAsync();

                var payrolls = new List<Payroll>();
                var payrollDetails = new List<PayrollDetail>();

                foreach (var employee in employees)
                {
                    var (payroll, details) = await CalculatePayrollAsyncTest(employee.Id, req);

                    payrolls.Add(payroll);
                    payrollDetails.AddRange(details);
                }

                await _appDbContext.Payrolls.AddRangeAsync(payrolls);
                await _appDbContext.PayrollDetails.AddRangeAsync(payrollDetails);

                await _appDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (IOException)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task<(Payroll, List<PayrollDetail>)> CalculatePayrollAsyncTest(int employeeId, PayrollCalculateReq req)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(employeeId);
            var contract = await _contractService.GetContractActiveByEmployeeIdAsync(employeeId);

            if (contract == null)
                throw new NotFoundException("Contract not found");

            var baseSalary = contract.BaseSalary;
            var workingPerMonth = contract.WorkingPerMonth;

            var baseSalaryPerDay = baseSalary / workingPerMonth;
            var baseSalaryPerHour = baseSalaryPerDay / 8;

            var workload = await _attendanceService
                .GetAttendanceWorkloadAsync(employeeId, req.Month, req.Year);

            var totalLeavingRequest = await _leaveRequestService
                .CalculateTotalLeavingAsync(employeeId, req.Month, req.Year);

            var holiday = await _holidayService.TotalHolidayAsync(req.Month, req.Year);

            var realSalary = (workload.TotalWorkingDays + (totalLeavingRequest <= contract.TotalLeavingsPerMonth
                ? totalLeavingRequest : 0)) * baseSalaryPerDay;

            var overtimeSalary = (decimal)workload.OvertimeWorkingHours
                                 * contract.OverTimeRate * baseSalaryPerHour;

            var allowance = contract.AllowancePark + contract.AllowanceLunchBreak;

            var deduction = workload.TotalCheckInLates * FINE_CHECK_IN_LATE_AMOUNT;

            var gross = realSalary + overtimeSalary + allowance
                        + holiday * baseSalaryPerDay - deduction;

            var net = gross - contract.Tax * gross;

            var now = DateTime.Now;

            var payroll = new Payroll
            {
                Month = req.Month,
                Year = req.Year,
                EmployeeId = employeeId,
                Insurance = 0,
                Tax = contract.Tax,
                TotalWorkingDaysInMonth = contract.WorkingPerMonth,
                ActualWorkingDays = workload.TotalWorkingDays,
                OvertimeHours = (decimal)workload.OvertimeWorkingHours,
                TotalHours = workload.TotalWorkingDays * 8,
                BasicSalary = realSalary,
                Allowance = allowance,
                Bonus = 0,
                Holidays = holiday,
                Deduction = deduction,
                GrossSalary = gross,
                NetSalary = net,
                PayrollStatus = PayrollStatus.PENDING,
                CreatedDate = now
            };

            var details = new List<PayrollDetail>
            {
                new PayrollDetail
                {
                    Payroll = payroll,
                    Description = $"Lương tháng {req.Month}/{req.Year}",
                    Amount = realSalary,
                    Type = PayrollDetailType.ERNING,
                    CreatedDate = now
                },
                new PayrollDetail
                {
                    Payroll = payroll,
                    Description = "Tăng ca",
                    Amount = overtimeSalary,
                    Type = PayrollDetailType.OVERTIME,
                    CreatedDate = now
                },
                new PayrollDetail
                {
                    Payroll = payroll,
                    Description = "Ngày lễ",
                    Amount = holiday * baseSalaryPerDay,
                    Type = PayrollDetailType.HOLIDAY,
                    CreatedDate = now
                },
                new PayrollDetail
                {
                    Payroll = payroll,
                    Description = "Trợ cấp gửi xe",
                    Amount = contract.AllowancePark,
                    Type = PayrollDetailType.ALLOWANCE,
                    CreatedDate = now
                },
                new PayrollDetail
                {
                    Payroll = payroll,
                    Description = "Trợ cấp ăn trưa",
                    Amount = contract.AllowanceLunchBreak, // FIX BUG
                    Type = PayrollDetailType.ALLOWANCE,
                    CreatedDate = now
                },
                new PayrollDetail
                {
                    Payroll = payroll,
                    Description = "Phạt đi trễ",
                    Amount = deduction,
                    Type = PayrollDetailType.DEDUCTION,
                    CreatedDate = now
                }
            };

            return (payroll, details);
        }

        public async Task<PagedResult<PayrollRes>> GetPayrollsAsync(PaginationQuery query, PayrollFilterReq filter)
        {
            var pageable = _appDbContext.Payrolls
                .Include(x => x.Employee)
                .AsNoTracking();

            if (filter.Month.HasValue)
            {
                pageable = pageable.Where(x => x.Month == filter.Month);
            }
            if (filter.Year.HasValue)
            {
                pageable = pageable.Where(x => x.Year == filter.Year);
            }
            pageable = pageable.Where(x => x.Employee.Fullname.Equals(filter.Keyword) 
            || x.Employee.Code.Equals(filter.Keyword) 
            || x.Employee.Email.Equals(filter.Keyword));
            if (filter.Status.HasValue)
            {
                pageable = pageable.Where(x => (int)x.PayrollStatus == filter.Status);
            }
            if (filter.From.HasValue)
            {
                pageable = pageable.Where(x => x.NetSalary >=  filter.From.Value);
            }
            if (filter.To.HasValue)
            {
                pageable = pageable.Where(x => x.NetSalary <= filter.To.Value);
            }
            var count = await pageable.CountAsync();
            var items = await pageable.Select(x => _payrollMapping.ToPayrollRes(x)).ToListAsync();
            return new PagedResult<PayrollRes>
            {
                Items = items,
                Page = query.Page,
                PageSize = query.PageSize,
                Total = count,
            };
        }

        public async Task ApprovePayrollAsync(PayrollCalculateReq req)
        {
            var res = await _appDbContext.Payrolls
                .ToListAsync();
            foreach (var item in res) {
                item.PayrollStatus = PayrollStatus.APPROVED;
            }
            await _appDbContext.AddRangeAsync(res);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<FullPayrollDetailRes?> GetPayrollDetailAsync(int id)
        {
            var res = await _appDbContext.Payrolls
                .Include(x => x.Employee)
                .Include(x => x.PayrollDetails)
                .FirstAsync();
            if (res == null)
            {
                throw new NotFoundException("Payroll not found");
            }
            return _payrollMapping.ToFullPayrollDetailRes(res);
        }
    }
}
