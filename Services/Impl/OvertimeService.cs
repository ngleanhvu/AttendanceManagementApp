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
            var overtime = await _appDbContext.OverTimes
                .Include(x => x.Employee)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (overtime == null)
                throw new NotFoundException("Overtime not found");

            // 1. Chỉ approve khi chưa approve
            if (overtime.IsApproved)
                throw new BadRequestException("Overtime already approved");

            var now = DateTime.Now;

            // 2. Không approve OT trong quá khứ
            if (overtime.WorkDate < DateOnly.FromDateTime(now))
                throw new BadRequestException("Cannot approve past overtime");

            // 3. Check overlap với OT đã approve
            var isOverlap = await _appDbContext.OverTimes.AnyAsync(x =>
                x.Employee.Id == overtime.Employee.Id &&
                x.Id != overtime.Id &&
                x.WorkDate == overtime.WorkDate &&
                x.IsApproved &&
                ((overtime.From >= x.From && overtime.From < x.To) ||
                 (overtime.To > x.From && overtime.To <= x.To)));

            if (isOverlap)
                throw new BadRequestException("Overlaps with approved overtime");

            // 4. Check attendance (phải có chấm công)
            var hasAttendance = await _appDbContext.Attendances.AnyAsync(x =>
                x.Employee.Id == overtime.Employee.Id &&
                x.WorkDate == overtime.WorkDate);

            if (!hasAttendance)
                throw new BadRequestException("Must check-in before approving overtime");

            // 5. Giới hạn OT theo ngày (sum lại)
            var totalOtHours = await _appDbContext.OverTimes
                .Where(x => x.Employee.Id == overtime.Employee.Id &&
                            x.WorkDate == overtime.WorkDate &&
                            x.IsApproved)
                .SumAsync(x => (x.To - x.From).TotalHours);

            var currentHours = (overtime.To - overtime.From).TotalHours;

            if (totalOtHours + currentHours > 4)
                throw new BadRequestException("Exceeded daily overtime limit");

            // 6. Giới hạn OT theo tháng (optional)
            var totalOtMonth = await _appDbContext.OverTimes
                .Where(x => x.Employee.Id == overtime.Employee.Id &&
                            x.WorkDate.Month == overtime.WorkDate.Month &&
                            x.WorkDate.Year == overtime.WorkDate.Year &&
                            x.IsApproved)
                .SumAsync(x => (x.To - x.From).TotalHours);

            if (totalOtMonth + currentHours > 40)
                throw new BadRequestException("Exceeded monthly overtime limit");

            // 7. Approve
            overtime.IsApproved = true;

            _repository.Update(overtime);
            await _repository.SaveAsync();

            return _overtimeMapping.ToOverTimeRes(overtime);
        }

        public async Task<OvertimeRes> CreateOverTimeAsync(OvertimeCreateReq req)
        {
            var now = DateTime.Now;
            var today = DateOnly.FromDateTime(now);

            var employee = await _employeeService.GetEmployeeByIdAsync(req.EmployeeId);

            // 1. Validate time
            if (req.From >= req.To)
                throw new BadRequestException("Invalid time range");

            if (req.WorkDate < today)
                throw new BadRequestException("Cannot register overtime in the past");

            var duration = (req.To - req.From).TotalHours;

            if (duration < 1)
                throw new BadRequestException("Minimum overtime is 1 hour");

            if (duration > 4)
                throw new BadRequestException("Max overtime per request is 4 hours");

            // 2. Không cho OT trong giờ hành chính
            var workStart = new TimeSpan(8, 0, 0);
            var workEnd = new TimeSpan(17, 0, 0);

            if (req.From.CompareTo(workEnd) < 0 && req.To.CompareTo(workStart) > 0)
                throw new BadRequestException("Overtime must be outside working hours");

            // 3. Check overlap (basic)
            var isOverlap = await _appDbContext.OverTimes.AnyAsync(x =>
                x.Employee.Id == req.EmployeeId &&
                x.WorkDate == req.WorkDate &&
                ((req.From >= x.From && req.From < x.To) ||
                 (req.To > x.From && req.To <= x.To)));

            if (isOverlap)
                throw new BadRequestException("Overtime overlaps");

            // 4. Check attendance (optional - tùy business)
            var hasAttendance = await _appDbContext.Attendances.AnyAsync(x =>
                x.Employee.Id == req.EmployeeId &&
                x.WorkDate == req.WorkDate);

            if (!hasAttendance)
                throw new BadRequestException("Must check-in before registering overtime");

            // 5. Tạo OT
            var overtime = new OverTime
            {
                Employee = employee,
                WorkDate = req.WorkDate,
                From = req.From,
                To = req.To,
                Reason = req.Reason.Trim(),
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
