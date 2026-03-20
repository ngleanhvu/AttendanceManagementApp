using AttendanceManagementApp.Configs;
using AttendanceManagementApp.DTOs.Request;
using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Exception;
using AttendanceManagementApp.Mappings;
using AttendanceManagementApp.Models;
using AttendanceManagementApp.Repositories;
using AttendanceManagementApp.Services.Interface;
using AttendanceManagementApp.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AttendanceManagementApp.Services.Impl
{
    public class AttendanceService : IAttendanceService
    {
        public readonly int HOUR_CHECK_IN = 8;
        public readonly int MINUTE_CHECK_IN = 30;
        private readonly IEmployeeService _employeeService;
        private readonly IRepository<Attendance> _attendanceRepository;
        private readonly AppDbContext _appDbContext;
        private readonly AttendanceMapping _attendanceMapping;
        public AttendanceService(IEmployeeService employeeService, 
            IRepository<Attendance> attendanceRepository, AppDbContext appDbContext,
            AttendanceMapping attendanceMapping)
        {
            this._employeeService = employeeService;
            this._appDbContext = appDbContext;
            this._attendanceRepository = attendanceRepository;
            this._attendanceMapping = attendanceMapping;
        }
        public async Task<AttendanceRes> CheckInAsync(AttendanceCheckInReq req)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(req.EmployeeId);
            var now = DateTime.Now;
            var today = DateOnly.FromDateTime(now);

            // ✅ Check đã chấm công chưa
            var existAttendance = await _appDbContext.Attendances
                .AnyAsync(x => x.Employee.Id == req.EmployeeId && x.WorkDate == today);

            if (existAttendance)
                throw new BadRequestException("Employee already checked in today");
            var standardTime = new TimeSpan(HOUR_CHECK_IN, MINUTE_CHECK_IN, 0);
            bool isLate = now.TimeOfDay > standardTime;

            var attendance = new Attendance
            {
                WorkDate = today,
                CheckIn = now,
                Employee = employee,
                AttendanceStatus = isLate ? AttendanceStatus.LATE : AttendanceStatus.PRESENT
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

        [HttpGet]
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
