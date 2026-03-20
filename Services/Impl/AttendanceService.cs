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
        public readonly int HOUR_CHECK_OUT = 17;
        public readonly int MINUTE_CHECK_OUT = 30;
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
                CheckIn = DateTime.Now,
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
                    .Include(x => x.Employee)
                    .AsQueryable();

            if (req.AttendanceStatus.HasValue)
            {
                pageable = pageable.Where(x => x.Equals(req.AttendanceStatus.Value));
            }

            if (req.FromDate.HasValue)
            {
                pageable = pageable.Where(x => req.FromDate <= x.WorkDate);
            }
            if (req.Month.HasValue)
            {
                pageable = pageable.Where(x => x.WorkDate.Month == req.Month);
            }
            if (req.Year.HasValue)
            {
                pageable = pageable.Where(x => x.WorkDate.Year == req.Year);
            }
            if (req.ToDate.HasValue)
            {
                pageable = pageable.Where(x => req.ToDate >= x.WorkDate);
            }

            if (req.EmployeeId.HasValue)
            {
                pageable = pageable.Where(x => x.EmployeeId == req.EmployeeId);
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

        public async Task<AttendanceWorkloadRes> GetAttendanceWorkloadAsync(int employeeId, int month, int year)
        {
            int totalWorkingDays = 0;
            double overtimeWorkingHours = 0;
            int totalCheckInLate = 0;

            var filter = new AttendanceFilterReq
            {
                EmployeeId = employeeId,
                Month = month,
                Year = year
            };

            var query = new PaginationQuery
            {
                PageSize = 32
            };

            var res = await GetAttendancesAsync(filter, query);

            if (res?.Items == null || !res.Items.Any()) // ✅ Guard null/empty
            {
                return new AttendanceWorkloadRes
                {
                    TotalWorkingDays = 0,
                    TotalCheckInLates = 0,
                    OvertimeWorkingHours = 0
                };
            }

            var standardCheckOutTime = new TimeSpan(HOUR_CHECK_OUT, MINUTE_CHECK_OUT, 0);

            foreach (var item in res.Items)
            {
                if (item.CheckIn.HasValue)
                {
                    totalWorkingDays++;
                }

                if (item.AttendanceStatus == AttendanceStatus.LATE.ToString())
                {
                    totalCheckInLate++;
                }

                // ✅ Chỉ tính OT khi checkout sau giờ chuẩn
                var standardCheckInTime = new TimeSpan(HOUR_CHECK_IN, MINUTE_CHECK_IN, 0);

                // ✅ So sánh TimeOfDay
                if (item.CheckIn.HasValue && item.CheckIn.Value.TimeOfDay > standardCheckInTime)
                {
                    totalCheckInLate++;
                }
            }

            return new AttendanceWorkloadRes
            {
                TotalWorkingDays = totalWorkingDays,
                TotalCheckInLates = totalCheckInLate,
                OvertimeWorkingHours = (float)Math.Round(overtimeWorkingHours, 2)
            };
        }
    }   
    }
