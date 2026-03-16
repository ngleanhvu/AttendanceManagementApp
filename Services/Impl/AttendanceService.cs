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
    public class AttendanceService : IAttendanceService
    {
        private readonly IEmployeeService _employeeService;
        private readonly IShiftService _shiftService;
        private readonly IRepository<Attendance> _attendanceRepository;
        private readonly AppDbContext _appDbContext;
        private readonly AttendanceMapping _attendanceMapping;
        public AttendanceService(IEmployeeService employeeService, IShiftService shiftService,
            IRepository<Attendance> attendanceRepository, AppDbContext appDbContext,
            AttendanceMapping attendanceMapping)
        {
            this._employeeService = employeeService;
            this._shiftService = shiftService;
            this._appDbContext = appDbContext;
            this._attendanceRepository = attendanceRepository;
            this._attendanceMapping = attendanceMapping;
        }
        public async Task<AttendanceRes> CheckInAsync(AttendanceCheckInReq req)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(req.EmployeeId);
            var shift = await _shiftService.GetShiftByIdAsync(req.ShiftId);
            var today = DateOnly.FromDateTime(DateTime.Now);
            var existAttendance = _appDbContext.Attendances.Include(x => x.Employee)
                .MinAsync(x => x.Employee.Id == req.EmployeeId && x.WorkDate == today);

            if (existAttendance != null)
                throw new BadRequestException("Employee already checked in today");

            var attendance = new Attendance
            {
                WorkDate = today,
                CheckIn = DateTime.Now,
                Employee = employee,
                Shift = shift,
                Note = req.Note
            };

            await _attendanceRepository.AddAsync(attendance);
            await _attendanceRepository.SaveAsync();

            return _attendanceMapping.ToAttendanceRes(attendance);
        }

        public async Task<AttendanceRes> CheckOutAsync(int attendanceId)
        {
            var attendance = await _attendanceRepository.GetByIdAsync(attendanceId);
            if (attendance.CheckOut.HasValue == true)
            {
                throw new BadRequestException("Employee already checked out today");
            }
            attendance.CheckOut = DateTime.Now;
            _attendanceRepository.Update(attendance);
            await _attendanceRepository.SaveAsync();
            return _attendanceMapping.ToAttendanceRes(attendance);
        }

        public async Task<PagedResult<AttendanceRes>> GetAttendancesAsync(AttendanceFilterReq req, PaginationQuery query)
        {
            var pageable = _appDbContext.Attendances
                    .AsNoTracking()
                    .AsQueryable();
            
            if (req.AttendanceStatus.HasValue)
            {
                pageable = pageable.Where(x => x.Equals(req.AttendanceStatus.Value));
            }

            if (req.FromDate.HasValue)
            {
                pageable = pageable.Where(x => req.FromDate <= x.WorkDate);
            }

            if (req.ToDate.HasValue)
            {
                pageable = pageable.Where(x => req.ToDate >= x.WorkDate);
            }

            if (req.EmployeeId.HasValue)
            {
                pageable = pageable.Include(x => x.EmployeeId).Where(x => x.EmployeeId == req.EmployeeId);
            }

            var count = await pageable.CountAsync();

            var items = await pageable.Select(x => _attendanceMapping.ToAttendanceRes(x)).ToListAsync();

            return new PagedResult<AttendanceRes>
            {
                Total = count,
                Items = items,
                Page = query.Page,
                PageSize = query.PageSize,
            };
        }
    }
}
