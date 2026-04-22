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
                    // 1️⃣ Kiểm tra trùng payroll
                    var existPayroll = await _appDbContext.Payrolls
                        .AnyAsync(x => x.EmployeeId == employee.Id &&
                                       x.Month == req.Month &&
                                       x.Year == req.Year);
                    if (existPayroll)
                        continue; // Skip, idempotent

                    // 2️⃣ Tính payroll chi tiết
                    var (payroll, details) = await CalculatePayrollForEmployeeAsync(employee.Id, req);

                    payrolls.Add(payroll);
                    payrollDetails.AddRange(details);
                }

                if (payrolls.Any())
                {
                    await _appDbContext.Payrolls.AddRangeAsync(payrolls);
                    await _appDbContext.PayrollDetails.AddRangeAsync(payrollDetails);
                    await _appDbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task<(Payroll, List<PayrollDetail>)> CalculatePayrollForEmployeeAsync(int employeeId, PayrollCalculateReq req)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(employeeId);
            var contract = await _contractService.GetContractActiveByEmployeeIdAsync(employeeId);

            if (contract == null)
                throw new NotFoundException("Contract not found");

            if (contract.BaseSalary <= 0 || contract.WorkingPerMonth <= 0)
                throw new BadRequestException("Invalid contract data");

            // Lương cơ bản
            var baseSalaryPerDay = contract.BaseSalary / contract.WorkingPerMonth;
            var baseSalaryPerHour = baseSalaryPerDay / 8;

            // Workload & leave
            var workload = await _attendanceService.GetAttendanceWorkloadAsync(employeeId, req.Month, req.Year);
            var totalLeavingRequest = await _leaveRequestService.CalculateTotalLeavingAsync(employeeId, req.Month, req.Year);
            var paidLeaveDays = Math.Min((decimal)totalLeavingRequest, contract.TotalLeavingsPerMonth);

            var realSalary = (workload.TotalWorkingDays + paidLeaveDays) * baseSalaryPerDay;

            // OT chỉ tính approved
            var overtimeSalary = (decimal)workload.OvertimeWorkingHours * contract.OverTimeRate * baseSalaryPerHour;

            // Ngày lễ
            var holidayDays = await _holidayService.TotalHolidayAsync(req.Month, req.Year);
            var holidaySalary = holidayDays * baseSalaryPerDay;

            // Trợ cấp
            var allowance = contract.AllowancePark + contract.AllowanceLunchBreak;

            // Phạt đi trễ
            var deduction = workload.TotalCheckInLates * FINE_CHECK_IN_LATE_AMOUNT;

            // Gross
            var gross = realSalary + overtimeSalary + holidaySalary + allowance - deduction;

            // Bảo hiểm (BHXH + BHYT + BHTN)
            var insurance = Math.Round(gross * 0.105m, 0);

            // Thuế (simple)
            var taxableIncome = gross - insurance;
            var taxAmount = Math.Round(taxableIncome * contract.Tax, 0);

            // Net
            var net = Math.Max(0, gross - insurance - taxAmount);

            var now = DateTime.UtcNow;

            var payroll = new Payroll
            {
                Month = req.Month,
                Year = req.Year,
                EmployeeId = employeeId,
                Insurance = insurance,
                Tax = contract.Tax,
                TotalWorkingDaysInMonth = contract.WorkingPerMonth,
                ActualWorkingDays = workload.TotalWorkingDays,
                OvertimeHours = (decimal)workload.OvertimeWorkingHours,
                TotalHours = workload.TotalWorkingDays * 8,
                BasicSalary = realSalary,
                Allowance = allowance,
                Bonus = 0,
                Holidays = holidayDays,
                Deduction = deduction,
                GrossSalary = Math.Round(gross, 0),
                NetSalary = net,
                PayrollStatus = PayrollStatus.PENDING,
                CreatedDate = now
            };

            // Tạo chi tiết
            var details = new List<PayrollDetail>
    {
        new PayrollDetail { Payroll = payroll, Description = $"Lương tháng {req.Month}/{req.Year}", Amount = realSalary, Type = PayrollDetailType.ERNING, CreatedDate = now },
        new PayrollDetail { Payroll = payroll, Description = "Tăng ca", Amount = overtimeSalary, Type = PayrollDetailType.OVERTIME, CreatedDate = now },
        new PayrollDetail { Payroll = payroll, Description = "Ngày lễ", Amount = holidaySalary, Type = PayrollDetailType.HOLIDAY, CreatedDate = now },
        new PayrollDetail { Payroll = payroll, Description = "Trợ cấp", Amount = allowance, Type = PayrollDetailType.ALLOWANCE, CreatedDate = now },
        new PayrollDetail { Payroll = payroll, Description = "Bảo hiểm", Amount = insurance, Type = PayrollDetailType.DEDUCTION, CreatedDate = now },
        new PayrollDetail { Payroll = payroll, Description = "Thuế", Amount = taxAmount, Type = PayrollDetailType.DEDUCTION, CreatedDate = now },
        new PayrollDetail { Payroll = payroll, Description = "Phạt đi trễ", Amount = deduction, Type = PayrollDetailType.DEDUCTION, CreatedDate = now }
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

            if (filter.Keyword != null)
            {
                pageable = pageable.Where(x => x.Employee.Fullname.Contains(filter.Keyword) 
                                               || x.Employee.Code.Contains(filter.Keyword) 
                                               || x.Employee.Email.Contains(filter.Keyword));
            }
            
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
                .Where(x => x.Month == req.Month && x.Year == req.Year)
                .ToListAsync();

            foreach (var item in res)
            {
                item.PayrollStatus = PayrollStatus.APPROVED;
            }

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
