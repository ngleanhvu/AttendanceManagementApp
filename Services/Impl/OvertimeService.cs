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
    public class OvertimeService : IOvertimeService
    {
        private readonly IRepository<OverTime> _repository;
        private readonly AppDbContext _appDbContext;
        private readonly OvertimeMapping _overtimeMapping;
        private readonly IEmployeeService _employeeService;

        public OvertimeService(IRepository<OverTime> repository, AppDbContext appDbContext,
            OvertimeMapping overtimeMapping, IEmployeeService employeeService)
        {
            _repository = repository;
            _appDbContext = appDbContext;
            _overtimeMapping = overtimeMapping;
            _employeeService = employeeService;
        }

        public async Task<OvertimeRes> ApprovedOverTimeAsync(int id)
        {
            var overtime = await _repository.GetByIdAsync(id);
            if (overtime == null)
                throw new NotFoundException("Overtime not found");
            overtime.IsApproved = true;
            _repository.Update(overtime);
            await _repository.SaveAsync();
            return _overtimeMapping.ToOverTimeRes(overtime);
        }

        public async Task<OvertimeRes> CreateOverTimeAsync(OvertimeCreateReq req)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(req.EmployeeId);
            if (req.From > req.To)
            {
                throw new BadRequestException("From date > To date");
            }
            var overtime = new OverTime
            {
                Employee = employee,
                WorkDate = req.WorkDate,
                From = req.From,
                To = req.To,
                Reason = req.Reason,
                IsApproved = false
            };
            await _repository.AddAsync(overtime);
            await _repository.SaveAsync();
            return _overtimeMapping.ToOverTimeRes(overtime);
        }

        public async Task<bool> ExistOverTimeAsync(int id, DateOnly workDate)
        {
            return await _appDbContext.OverTimes
                .AnyAsync(x => x.EmployeeId == id
                            && x.WorkDate == workDate
                            && x.Status == true);
        }

        public async Task<OverTime?> GetOverTimeByEmployeeIdAndWorkDateAsync(int employeeId, DateOnly workDate)
        {
            return await _appDbContext.OverTimes
                .FirstOrDefaultAsync(x => x.EmployeeId == employeeId && x.WorkDate == workDate && x.Status == true);
        }

        public async Task<PagedResult<OvertimeRes>> GetOverTimesAsync(OvertimeFilterReq req, PaginationQuery query)
        {
            var pagable = _appDbContext.OverTimes
                .AsNoTracking()
                .Where(x => x.Status == true)
                .AsQueryable();

            if (req.IsApproved.HasValue)
            {
                pagable = pagable.Where(x => x.Status == req.IsApproved);
            }
            if (req.EmployeeId.HasValue)
            {
                pagable = pagable.Where(x => x.Employee.Id == req.EmployeeId);
            }
            if (req.WorkDate.HasValue)
            {
                pagable = pagable.Where(x => x.WorkDate == req.WorkDate);
            }
            var count = await pagable.CountAsync();
            var items = await pagable.Select(x => _overtimeMapping.ToOverTimeRes(x)).ToListAsync();
            return new PagedResult<OvertimeRes>
            {
                Items = items,
                Page = query.Page,
                PageSize = query.PageSize,
                Total = count
            };
        }

        public async Task<OvertimeRes> SoftDeleteOverTimeAsync(int id)
        {
            var overtime = await _repository.GetByIdAsync(id);
            if (overtime == null)
                throw new NotFoundException("Overtime not found");
            overtime.Status = false;
            _repository.SoftDelete(overtime);
            await _repository.SaveAsync();
            return _overtimeMapping.ToOverTimeRes(overtime);
        }

        public async Task<OvertimeRes> UpdateOverTimeAsync(int id, OvertimeCreateReq req)
        {
            var overtime = await _repository.GetByIdAsync(id);
            if (overtime == null)
                throw new NotFoundException("Overtime not found");
            if (req.From > req.To)
            {
                throw new BadRequestException("From date > To date");
            }
            overtime.WorkDate = req.WorkDate;
            overtime.From = req.From;
            overtime.To = req.To;
            overtime.Reason = req.Reason;
            return _overtimeMapping.ToOverTimeRes(overtime);
        }
    }
}
