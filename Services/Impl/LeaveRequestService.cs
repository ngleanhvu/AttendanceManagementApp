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
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly IRepository<LeaveRequest> _leaveRequestRepository;
        private readonly LeaveRequestMapping _leaveRequestMapping;
        private readonly AppDbContext _appDbContext;
        private readonly EmployeeMapping _employeeMapping;
        private readonly LeaveTypeMapping _leaveTypeMapping;
        private readonly IEmployeeService _employeeService;
        private readonly ILeaveTypeService _leaveTypeService;
        private readonly IContractService _contractService;
        private readonly int MAX_LEAVED_DAY = 3;
        private readonly int MAX_YEAR_DAY = 12;

        public LeaveRequestService(IRepository<LeaveRequest> leaveRequestRepository, LeaveRequestMapping leaveRequestMapping, 
            AppDbContext appDbContext, EmployeeMapping employeeMapping, LeaveTypeMapping leaveTypeMapping,
            IEmployeeService employeeService, ILeaveTypeService leaveTypeService, IContractService contractService)
        {
            _leaveRequestRepository = leaveRequestRepository;
            _leaveRequestMapping = leaveRequestMapping;
            _appDbContext = appDbContext;
            _employeeMapping = employeeMapping;
            _leaveTypeMapping = leaveTypeMapping;
            _employeeService = employeeService;
            _leaveTypeService = leaveTypeService;
            _contractService = contractService;
        }

        public async Task<int> CalculateTotalLeavedDayAsync(int employeeId, int year)
        {
            return await _appDbContext.LeaveRequests
                .Where(x => x.EmployeeId == employeeId && x.LeaveStatus == LeaveStatus.Approved && x.Status == true)
                .CountAsync();
        }

        public async Task<float> CalculateTotalLeavingAsync(int employeeId, int month, int year)
        {
            var startOfMonth = new DateTime(year, month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            var items = await _appDbContext.LeaveRequests
                .Where(x => x.EmployeeId == employeeId)
                .Where(x => x.LeaveStatus == LeaveStatus.Approved)
                .Where(x => x.FromDate <= endOfMonth && x.ToDate >= startOfMonth)
                .AsNoTracking()
                .ToListAsync();

            var days = new Dictionary<DateTime, float>();

            foreach (var item in items)
            {
                var actualFrom = item.FromDate < startOfMonth ? startOfMonth : item.FromDate;
                var actualTo = item.ToDate > endOfMonth ? endOfMonth : item.ToDate;

                for (var d = actualFrom.Date; d <= actualTo.Date; d = d.AddDays(1))
                {
                    if (!days.ContainsKey(d))
                        days[d] = 0;

                    if (item.LeaveRequestType == LeaveRequestType.FULLDAY)
                        days[d] = 1;
                    else
                        days[d] = Math.Min(1, days[d] + 0.5f);
                }
            }

            return days.Values.Sum();
        }

        public async Task<LeaveRequestRes> CreateLeaveRequestAsync(LeaveRequestCreateReq req)
        {
            var now = DateTime.UtcNow.Date;

            // 1. Validate basic
            if (req.FromDate > req.ToDate)
                throw new BadRequestException("Ngày kết thúc sớm hơn ngày bắt đầu");

            if (req.FromDate.Date < now)
                throw new BadRequestException("Không thể tạo yêu cầu từ quá khứ");

            if (string.IsNullOrWhiteSpace(req.Reason))
                throw new BadRequestException("Vui lòng nhập lý do");

            var employee = await _employeeService.GetEmployeeByIdAsync(req.EmployeeId);

            // 2. Calculate total days
            decimal totalDays = (decimal)(req.ToDate.Date - req.FromDate.Date).TotalDays + 1;

            // 3. Validate partial leave
            var isPartial = req.LeaveRequestTypeId == (int)LeaveRequestType.PART_DAY_AM ||
                            req.LeaveRequestTypeId == (int)LeaveRequestType.PART_DAY_PM;

            if (isPartial)
            {
                if (req.FromDate.Date != req.ToDate.Date)
                    throw new BadRequestException("Phép nửa ngày phải diễn ra trong cùng 1 ngày");

                totalDays = 0.5m;
            }

            // 4. Check overlap (basic - chưa cần approved)
            var isOverlap = await _appDbContext.LeaveRequests.AnyAsync(x =>
                x.EmployeeId == req.EmployeeId &&
                x.LeaveStatus != LeaveStatus.Rejected &&
                req.FromDate <= x.ToDate &&
                req.ToDate >= x.FromDate && x.Status == true);

            if (isOverlap)
                throw new BadRequestException("Yêu cầu nghỉ phép bị trùng với một yêu cầu nghỉ phép đã tồn tại.");
            
            // 6. Check request in advance (ví dụ: ít nhất 1 ngày)
            if ((req.FromDate.Date - now).TotalDays < 1)
                throw new BadRequestException("Vui lòng gửi yêu cầu trước 1 ngày");

            // 7. Optional: không cho xin nghỉ cuối tuần
            if (req.FromDate.DayOfWeek == DayOfWeek.Saturday ||
                req.FromDate.DayOfWeek == DayOfWeek.Sunday)
            {
                throw new BadRequestException("Không thể tạo yêu cầu nghỉ phép vào thứ 7 chủ nhật");
            }

            // 8. Create entity
            var leaveRequest = new LeaveRequest
            {
                FromDate = req.FromDate,
                ToDate = req.ToDate,
                TotalDays = (float)totalDays,
                CreatedDate = DateTime.UtcNow,
                LeaveStatus = LeaveStatus.Pending,
                EmployeeId = employee.Id,
                Reason = req.Reason.Trim(),
            };

            await _leaveRequestRepository.AddAsync(leaveRequest);
            await _leaveRequestRepository.SaveAsync();

            return _leaveRequestMapping.ToLeaveRequestRes(leaveRequest, 0, 0);
        }

        public async Task<PagedResult<LeaveRequestRes>> GetLeaveRequestsAsync(
                        LeaveRequestFilterReq filter,
                        PaginationQuery query)
        {
            var pageable = _appDbContext.LeaveRequests
                .Include(x => x.Employee)
                .Where(x => x.Status == true)
                .AsNoTracking()
                .AsQueryable();

            if (filter.FromDate.HasValue)
                pageable = pageable.Where(x => x.FromDate >= filter.FromDate);

            if (filter.ToDate.HasValue)
                pageable = pageable.Where(x => x.ToDate <= filter.ToDate);

            if (filter.EmployeeId.HasValue)
                pageable = pageable.Where(x => x.EmployeeId == filter.EmployeeId);

            if (filter.LeaveStatus.HasValue)
                pageable = pageable.Where(x => x.LeaveStatus == (LeaveStatus)filter.LeaveStatus);

            var count = await pageable.CountAsync();

            var data = await pageable
                .OrderByDescending(x => x.CreatedDate)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            var cache = new Dictionary<int, (float year, float month)>();

            var items = await Task.WhenAll(
                data.Select(async x =>
                {
                    if (!cache.ContainsKey(x.EmployeeId))
                    {
                        var totalYear = await CalculateTotalLeavedDayAsync(
                            x.EmployeeId, x.FromDate.Year);

                        var totalMonth = await CalculateTotalLeavingAsync(
                            x.EmployeeId, x.FromDate.Month, x.FromDate.Year);

                        cache[x.EmployeeId] = (totalYear, totalMonth);
                    }

                    var val = cache[x.EmployeeId];

                    return _leaveRequestMapping.ToLeaveRequestRes(
                        x,
                        (int) val.year,
                        val.month
                    );
                })
            );

            return new PagedResult<LeaveRequestRes>
            {
                Total = count,
                Items = items.ToList(),
                Page = query.Page,
                PageSize = query.PageSize,
            };
        }

        public async Task SoftDeleteLeaveRequestAsync(int id)
        {
            var leaveRequest = await _leaveRequestRepository.GetByIdAsync(id);
            if (leaveRequest == null)
            {
                throw new NotFoundException("Phép nghỉ không tồn tại");
            }
            leaveRequest.Status = false;
            _leaveRequestRepository.SoftDelete(leaveRequest);
            await _leaveRequestRepository.SaveAsync();
        }

        public async Task<LeaveRequestRes> UpdateLeaveStatusAsync(int id, LeaveRequestUpdateStatusReq req)
        {
            var leaveRequest = await _appDbContext.LeaveRequests
                .Include(x => x.Employee)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (leaveRequest == null)
                throw new NotFoundException("Phép nghỉ không tồn tại");

            if (leaveRequest.LeaveStatus != LeaveStatus.Pending)
                throw new BadRequestException("Only pending request can be updated");

            if (req.LeaveStatus == (int)LeaveStatus.Approved)
            {
                // Check past
                if (leaveRequest.FromDate.Date < DateTime.UtcNow.Date)
                    throw new BadRequestException("Không thể duyệt yêu cầu nghỉ trong quá khứ");

                // Check overlap
                var isOverlap = await _appDbContext.LeaveRequests.AnyAsync(x =>
                    x.EmployeeId == leaveRequest.EmployeeId &&
                    x.Id != leaveRequest.Id &&
                    x.LeaveStatus == LeaveStatus.Approved &&
                    leaveRequest.FromDate <= x.ToDate &&
                    leaveRequest.ToDate >= x.FromDate && x.Status == true);

                if (isOverlap)
                    throw new BadRequestException("Yêu cầu nghỉ phép bị trùng với một yêu cầu nghỉ phép đã tồn tại.");

                // Check quota
                var totalMonth = await CalculateTotalLeavingAsync(
                    leaveRequest.EmployeeId,
                    leaveRequest.FromDate.Month,
                    leaveRequest.FromDate.Year);

                if (totalMonth + leaveRequest.TotalDays > MAX_LEAVED_DAY)
                    throw new BadRequestException("Số ngày nghỉ phép trong tháng vượt mức cho phép");

                var totalYear = await CalculateTotalLeavedDayAsync(
                    leaveRequest.EmployeeId,
                    leaveRequest.FromDate.Year);

                if (totalYear + leaveRequest.TotalDays > MAX_YEAR_DAY)
                    throw new BadRequestException("Số ngày nghỉ phép trong năm vượt mức cho phép");

                leaveRequest.LeaveStatus = LeaveStatus.Approved;
                leaveRequest.ApprovedDate = DateTime.UtcNow;
            }
            else if (req.LeaveStatus == (int)LeaveStatus.Rejected)
            {
                if (string.IsNullOrEmpty(req.RejectReason))
                    throw new BadRequestException("Vui lòng nhập lý do");

                leaveRequest.LeaveStatus = LeaveStatus.Rejected;
                leaveRequest.RejectReason = req.RejectReason;
            }
            else
            {
                throw new BadRequestException("Trạng thái không hợp lệ");
            }

            _leaveRequestRepository.Update(leaveRequest);
            await _leaveRequestRepository.SaveAsync();

            return _leaveRequestMapping.ToLeaveRequestRes(leaveRequest, 0, 0);
        }

    }
}
