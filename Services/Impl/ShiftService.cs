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
    public class ShiftService : IShiftService
    {
        private readonly AppDbContext _context;
        private readonly IRepository<Shift> _shiftRepository;
        private readonly ShiftMapping _shiftMapping;

        public ShiftService(AppDbContext context, IRepository<Shift> shiftRepository, ShiftMapping shiftMapping)
        {
            _context = context;
            _shiftRepository = shiftRepository;
            this._shiftMapping = shiftMapping;
        }

        public async Task<ShiftRes> CreateShiftAsync(ShiftCreateReq req)
        {
            var shift = new Shift
            {
                Name = req.Name,
                ShiftType = (ShiftType) req.ShiftType,
                Description = req.Description,
                StartTime = req.StartTime,
                EndTime = req.EndTime,
                StandardHours = req.StandardHours,
                BreakStartTime = req.BreakStartTime,
                BreakEndTime = req.BreakEndTime,
                AllowedLateMinutes = req.AllowedLateMinutes,
                AllowedEarlyLeaveMinutes = req.AllowedEarlyLeaveMinutes,
                IsOvernight = req.IsOvernight,
                IsActive = req.IsActive
            };

            await _shiftRepository.AddAsync(shift);
            await _shiftRepository.SaveAsync();

            return _shiftMapping.ToShiftRes(shift);
        }

        public async Task<ShiftRes> DeleteShiftAsync(int id)
        {
            var shift = _shiftRepository.GetByIdAsync(id).Result;
            if (shift == null)
                throw new NotFoundException("Shift not found.");
            
            _shiftRepository.SoftDelete(shift);
            await _shiftRepository.SaveAsync();
            return _shiftMapping.ToShiftRes(shift);
        }

        public Task<ShiftRes> GetShiftAsync(int id)
        {
            var shift = _shiftRepository.GetByIdAsync(id).Result;
            if (shift == null)
                throw new NotFoundException("Shift not found.");
            return Task.FromResult(_shiftMapping.ToShiftRes(shift));
        }

        public async Task<Shift> GetShiftByIdAsync(int id)
        {
            var shift = await _shiftRepository.GetByIdAsync(id);
            if (shift == null)
                throw new NotFoundException("Shift not found.");
            return shift;
        }

        public async Task<PagedResult<ShiftRes>> GetShiftsAsync(PaginationQuery query)
        {
            var pageble = _context.Shifts
                .AsNoTracking()
                .ApplySearch(query.Search, s => s.Name, s => s.ShiftType.ToString())
                .ApplySorting(query.SortBy, query.Desc);

            var count = await pageble.CountAsync();
            var items = await pageble
                .ApplyPagination(query.Page, query.PageSize)
                .ToListAsync();
            var itemsRes = items.Select(s => _shiftMapping.ToShiftRes(s)).ToList();
            return new PagedResult<ShiftRes>
            {
                Total = count,
                Items = itemsRes,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        public async Task<ShiftRes> UpdateShiftAsync(int id, ShiftCreateReq req)
        {
            var shift = _shiftRepository.GetByIdAsync(id).Result;
            if (shift == null)
                throw new NotFoundException("Shift not found.");
            shift.Name = req.Name;
            shift.ShiftType = (ShiftType) req.ShiftType;
            shift.Description = req.Description;
            shift.StartTime = req.StartTime;
            shift.EndTime = req.EndTime;
            shift.StandardHours = req.StandardHours;
            shift.BreakStartTime = req.BreakStartTime;
            shift.BreakEndTime = req.BreakEndTime;
            shift.AllowedLateMinutes = req.AllowedLateMinutes;
            shift.AllowedEarlyLeaveMinutes = req.AllowedEarlyLeaveMinutes;
            shift.IsOvernight = req.IsOvernight;
            shift.IsActive = req.IsActive;

            _shiftRepository.Update(shift);
            await _shiftRepository.SaveAsync();

            return _shiftMapping.ToShiftRes(shift);
        }
    }
}
