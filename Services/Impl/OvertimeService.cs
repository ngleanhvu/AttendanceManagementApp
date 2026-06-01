using AttendanceManagementApp.Configs;
using AttendanceManagementApp.DTOs.Request;
using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Exception;
using AttendanceManagementApp.Mappings;
using AttendanceManagementApp.Models;
using AttendanceManagementApp.Models.Enum;
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

        public async Task<OvertimeRes> ApprovedOverTimeAsync(int id, int status)
        {
            var overtime = await _appDbContext.OverTimes
                .Include(x => x.Employee)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (overtime == null)
                throw new NotFoundException("Yêu cầu tăng ca không tồn tại");

            if (overtime.OvertimeStatus == Models.Enum.OvertimeStatus.APPROVED)
                throw new BadRequestException("Yêu cầu tăng ca đã được chấp nhận");

            if (overtime.OvertimeStatus == Models.Enum.OvertimeStatus.REJECTED)
                throw new BadRequestException("Yêu cầu tăng ca đã được từ chối");

            var newStatus = (Models.Enum.OvertimeStatus)status;

            // ================== REJECT ==================
            if (newStatus == Models.Enum.OvertimeStatus.REJECTED)
            {
                overtime.OvertimeStatus = Models.Enum.OvertimeStatus.REJECTED;
                _repository.Update(overtime);
                await _repository.SaveAsync();

                return _overtimeMapping.ToOverTimeRes(overtime);
            }

            // ================== APPROVE ==================
            if (newStatus == Models.Enum.OvertimeStatus.APPROVED)
            {
                var now = DateTime.Now;

                if (overtime.WorkDate < DateOnly.FromDateTime(now))
                    throw new BadRequestException("Không thể chấp nhận yêu cầu trong quá khứ");

                // Check overlap
                var isOverlap = await _appDbContext.OverTimes.AnyAsync(x =>
                    x.Employee.Id == overtime.Employee.Id &&
                    x.Id != overtime.Id &&
                    x.WorkDate == overtime.WorkDate &&
                    x.OvertimeStatus == Models.Enum.OvertimeStatus.APPROVED &&
                    ((overtime.From >= x.From && overtime.From < x.To) ||
                     (overtime.To > x.From && overtime.To <= x.To)) && x.Status == true);

                if (isOverlap)
                    throw new BadRequestException("Trung lặp thời gian yêu cầu tăng ca đã tồn tại");

                // Must have attendance
                var hasAttendance = await _appDbContext.Attendances.AnyAsync(x =>
                    x.Employee.Id == overtime.Employee.Id &&
                    x.WorkDate == overtime.WorkDate);
                

                var currentHours = (overtime.To - overtime.From).TotalHours;

                var totalOtHours = _appDbContext.OverTimes
                    .Where(x => x.Employee.Id == overtime.Employee.Id &&
                                x.WorkDate == overtime.WorkDate &&
                                x.OvertimeStatus == Models.Enum.OvertimeStatus.APPROVED)
                    .AsEnumerable()
                    .Sum(x => (x.To - x.From).TotalHours);

                if (totalOtHours + currentHours > 4)
                    throw new BadRequestException("Thời gian tăng ca > 4");

                var totalOtMonth = _appDbContext.OverTimes
                    .Where(x => x.Employee.Id == overtime.Employee.Id &&
                                x.WorkDate.Month == overtime.WorkDate.Month &&
                                x.WorkDate.Year == overtime.WorkDate.Year &&
                                x.OvertimeStatus == Models.Enum.OvertimeStatus.APPROVED)
                    .AsEnumerable()
                    .Sum(x => (x.To - x.From).TotalHours);

                overtime.OvertimeStatus = Models.Enum.OvertimeStatus.APPROVED;
                _repository.Update(overtime);
                await _repository.SaveAsync();

                return _overtimeMapping.ToOverTimeRes(overtime);
            }

            throw new BadRequestException("Trạng thái không hợp lệ");
        }

        public async Task<OvertimeRes> CreateOverTimeAsync(OvertimeCreateReq req)
        {
            var now = DateTime.Now;
            var today = DateOnly.FromDateTime(now);

            var employee = await _employeeService.GetEmployeeByIdAsync(req.EmployeeId);

            // 1. Validate time
            if (req.From >= req.To)
                throw new BadRequestException("Thời gian kết thúc sớm hơn thời gian bắt đầu");

            if (req.WorkDate < today)
                throw new BadRequestException("Không thể tạo yêu cầu tăng ca trong quá khứ");

            var duration = (req.To - req.From).TotalHours;

            if (duration < 1)
                throw new BadRequestException("Thời gian tăng ca < 1");

            if (duration > 4)
                throw new BadRequestException("Thòi gian tối đa cho mỗi buổi tăng ca <= 4");

            // 2. Không cho OT trong giờ hành chính

            var workStart = new TimeOnly(8, 30);
            var workEnd = new TimeOnly(17, 30);

            // 3. Check overlap (basic)
            var isOverlap = await _appDbContext.OverTimes.AnyAsync(x =>
                x.Employee.Id == req.EmployeeId &&
                x.WorkDate == req.WorkDate &&
                ((req.From >= x.From && req.From < x.To) ||
                 (req.To > x.From && req.To <= x.To)) && x.Status == true);

            if (isOverlap)
                throw new BadRequestException("Thời gian tăng ca trùng lặp với thòi gian tăng ca trong quá khứ");

            // 4. Check attendance (optional - tùy business)
            var hasAttendance = await _appDbContext.Attendances.AnyAsync(x =>
                x.Employee.Id == req.EmployeeId &&
                x.WorkDate == req.WorkDate);

            // 5. Tạo OT
            var overtime = new OverTime
            {
                Employee = employee,
                WorkDate = req.WorkDate,
                From = req.From,
                To = req.To,
                Reason = req.Reason.Trim(),
                OvertimeStatus = Models.Enum.OvertimeStatus.PENDING,
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
        public async Task<OverTime?> GetOverTimeByEmployeeIdAndWorkDateAsync(
            int employeeId,
            DateOnly workDate)
        {
            return await _appDbContext.OverTimes
                .FirstOrDefaultAsync(x =>
                    x.EmployeeId == employeeId
                    && x.WorkDate == workDate
                    && x.Status
                    && x.OvertimeStatus == OvertimeStatus.APPROVED);
        }

        public async Task<PagedResult<OvertimeRes>> GetOverTimesAsync(OvertimeFilterReq req, PaginationQuery query)
        {
            var pagable = _appDbContext.OverTimes
                .AsNoTracking()
                .Where(x => x.Status == true)
                .Include(x => x.Employee)
                .AsQueryable();

            if (req.IsApproved.HasValue)
            {
                pagable = pagable.Where(x => x.OvertimeStatus == (OvertimeStatus) req.IsApproved);
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

        public async Task SoftDeleteOverTimeAsync(int id)
        {
            var overtime = await _repository.GetByIdAsync(id);
            if (overtime == null)
                throw new NotFoundException("Yêu cầu tăng ca không tồn tại");
            overtime.Status = false;
            _repository.SoftDelete(overtime);
            await _repository.SaveAsync();
        }

        public async Task<OvertimeRes> UpdateOverTimeAsync(int id, OvertimeCreateReq req)
        {
            var overtime = await _repository.GetByIdAsync(id);
            if (overtime == null)
                throw new NotFoundException("Yêu cầu tăng ca không tồn tại");
            if (req.From > req.To)
            {
                throw new BadRequestException("Ngày kết thúc sớm hơn ngày bắt đầu");
            }
            overtime.WorkDate = req.WorkDate;
            overtime.From = req.From;
            overtime.To = req.To;
            overtime.Reason = req.Reason;
            return _overtimeMapping.ToOverTimeRes(overtime);
        }
    }
}
