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

        public LeaveRequestService(IRepository<LeaveRequest> leaveRequestRepository, LeaveRequestMapping leaveRequestMapping, 
            AppDbContext appDbContext, EmployeeMapping employeeMapping, LeaveTypeMapping leaveTypeMapping,
            IEmployeeService employeeService, ILeaveTypeService leaveTypeService)
        {
            _leaveRequestRepository = leaveRequestRepository;
            _leaveRequestMapping = leaveRequestMapping;
            _appDbContext = appDbContext;
            _employeeMapping = employeeMapping;
            _leaveTypeMapping = leaveTypeMapping;
            _employeeService = employeeService;
            _leaveTypeService = leaveTypeService;
        }

        public async Task<LeaveRequestRes> CreateLeaveRequestAsync(LeaveRequestCreateReq req)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(req.EmployeeId);
            var leaveType = await _leaveTypeService.GetLeaveTypeByIdAsync(employee.Id);
            var leaveRequest = new LeaveRequest
            {
                FromDate = req.FromDate,
                ToDate = req.ToDate,
                TotalDays = (req.ToDate - req.FromDate).Days,
                CreatedDate = DateTime.UtcNow,
                LeaveStatus = LeaveStatus.Pending,
                EmployeeId = employee.Id,
                LeaveType = leaveType,
            };
            await _leaveRequestRepository.AddAsync(leaveRequest);
            await _leaveRequestRepository.SaveAsync();
            return _leaveRequestMapping.ToLeaveRequestRes(leaveRequest);
        }

        public async Task<PagedResult<LeaveRequestRes>> GetLeaveRequestsAsync(LeaveRequestFilterReq filter, PaginationQuery query)
        {
            var pageable = _appDbContext.LeaveRequests
                .Include(x => x.Employee)
                .Include(x => x.LeaveType)
                .AsNoTracking()
                .AsQueryable();
            
            if (filter.FromDate.HasValue)
            {
                pageable = pageable.Where(x => x.FromDate >= filter.FromDate);
            }
            if (filter.ToDate.HasValue)
            {
                pageable = pageable.Where(x => x.ToDate <= filter.ToDate);
            }
            if (filter.EmployeeId.HasValue)
            {
                pageable = pageable.Where(x => x.EmployeeId ==  filter.EmployeeId);
            }
            if (filter.LeaveTypeId.HasValue)
            {
                pageable = pageable.Where(x => x.LeaveTypeId == filter.LeaveTypeId);
            }
            if (filter.LeaveStatus.HasValue)
            {
                pageable = pageable.Where(x => x.LeaveStatus == (LeaveStatus)filter.LeaveStatus);
            }
            var count = await pageable.CountAsync();

            var items = await pageable.Select(x => _leaveRequestMapping.ToLeaveRequestRes(x)).ToListAsync();

            return new PagedResult<LeaveRequestRes>
            {
                Total = count,
                Items = items,
                Page = query.Page,
                PageSize = query.PageSize,
            };
        }

        public async Task<LeaveRequestRes> SoftDeleteLeaveRequestAsync(int id)
        {
            var leaveRequest = await _leaveRequestRepository.GetByIdAsync(id);
            if (leaveRequest == null)
            {
                throw new NotFoundException("Leave request not found");
            }
            leaveRequest.Status = false;
            _leaveRequestRepository.SoftDelete(leaveRequest);
            await _leaveRequestRepository.SaveAsync();
            return _leaveRequestMapping.ToLeaveRequestRes(leaveRequest);
        }

        public async Task<LeaveRequestRes> UpdateLeaveStatusAsync(int id, LeaveRequestUpdateStatusReq req)
        {
            var leaveRequest = await _leaveRequestRepository.GetByIdAsync(id);
            if (leaveRequest == null)
            {
                throw new NotFoundException("Leave request not found");
            }
            switch (req.LeaveStatus)
            {
                case 2:
                    leaveRequest.ApprovedDate = DateTime.Now;
                    leaveRequest.Reason = req.Reason;
                    leaveRequest.LeaveStatus = (LeaveStatus)req.LeaveStatus;
                    break;
                case 3:
                    leaveRequest.RejectReason = req.RejectReason;
                    leaveRequest.LeaveStatus = (LeaveStatus)req.LeaveStatus;
                    break;
                default:
                    throw new BadRequestException("Leave status is not valid");
            }
            _leaveRequestRepository.Update(leaveRequest);
            await _leaveRequestRepository.SaveAsync();
            return _leaveRequestMapping.ToLeaveRequestRes(leaveRequest);
        }
    }
}
