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
    public class LeaveTypeService : ILeaveTypeService
    {
        private readonly IRepository<LeaveType> _leaveTypeRepository;
        private readonly LeaveTypeMapping _leaveTypeMapping;
        private readonly AppDbContext _appDbContext;


        public LeaveTypeService(IRepository<LeaveType> leaveTypeRepository, LeaveTypeMapping leaveTypeMapping,
            AppDbContext appDbContext)
        {
            _leaveTypeRepository = leaveTypeRepository;
            _leaveTypeMapping = leaveTypeMapping;
            _appDbContext = appDbContext;
        }

        public async Task<LeaveTypeRes> CreateLeaveTypeAsync(LeaveTypeCreateReq req)
        {
            var leaveType = new LeaveType
            {
                Name = req.Name,
                Description = req.Description,
                RequiresApproval = req.RequiresApproval,
                IsPaid = req.IsPaid,
                MaxDaysPerMonth = req.MaxDaysPerMonth,
                MaxDaysPerYear = req.MaxDaysPerYear,
            };
            await _leaveTypeRepository.AddAsync(leaveType);
            await _leaveTypeRepository.SaveAsync();

            return _leaveTypeMapping.ToLeaveTypeRes(leaveType);
        }

        public async Task<LeaveType> GetLeaveTypeByIdAsync(int id)
        {
            var leaveType = await _leaveTypeRepository.GetByIdAsync(id);
            if (leaveType == null)
                throw new NotFoundException("Leave type not found");
            return leaveType;
        }

        public async Task<PagedResult<LeaveTypeRes>> GetLeaveTypesAsync(PaginationQuery query)
        {
            var pageable = _appDbContext.LeaveTypes
                 .AsNoTracking()
                 .ApplySearch(query.Search, s => s.Name, s => s.Description)
                 .ApplySorting(query.SortBy, query.Desc);
            var count = await pageable.CountAsync();
            var items = await pageable.Select(x => _leaveTypeMapping.ToLeaveTypeRes(x)).ToListAsync();
            return new PagedResult<LeaveTypeRes>
            {
                Items = items,
                Page = query.Page,
                Total = count,
                PageSize = query.PageSize,
            };
        }

        public async Task<LeaveType> SoftDeleteLeaveTypeAsync(int id)
        {
            var leaveType = await _leaveTypeRepository.GetByIdAsync(id);
            if (leaveType == null)
                throw new NotFoundException("Leave type not found");
            leaveType.Status = false;
            _leaveTypeRepository.SoftDelete(leaveType);
            await _leaveTypeRepository.SaveAsync();
            return leaveType;
        }

        public async Task<LeaveTypeRes> UpdateLeaveTypeAsync(int id, LeaveTypeCreateReq req)
        {
            var leaveType = await _leaveTypeRepository.GetByIdAsync(id);
            if (leaveType == null)
                throw new NotFoundException("Leave type not found");
            leaveType.Name = req.Name;
            leaveType.Description = req.Description;
            leaveType.MaxDaysPerYear = req.MaxDaysPerYear;
            leaveType.IsPaid = req.IsPaid;
            leaveType.RequiresApproval = req.RequiresApproval;
            leaveType.MaxDaysPerMonth = req.MaxDaysPerMonth;

            _leaveTypeRepository.Update(leaveType);
            await _leaveTypeRepository.SaveAsync();
            return _leaveTypeMapping.ToLeaveTypeRes(leaveType);
        }
    }
}
