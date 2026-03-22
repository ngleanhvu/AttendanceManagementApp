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
        private readonly IOvertimeService _overtimeSerivce;
        public AttendanceService(IEmployeeService employeeService,
            IRepository<Attendance> attendanceRepository, AppDbContext appDbContext,
            AttendanceMapping attendanceMapping, IOvertimeService overtimeSerivce)
        {
            this._employeeService = employeeService;
            this._appDbContext = appDbContext;
            this._attendanceRepository = attendanceRepository;
            this._attendanceMapping = attendanceMapping;
            _overtimeSerivce = overtimeSerivce;
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
                    .Where(x => x.Status == true)
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

                    var standardCheckOutTime = new TimeSpan(HOUR_CHECK_OUT, MINUTE_CHECK_OUT, 0);

                    var overtime = await _overtimeSerivce
                        .GetOverTimeByEmployeeIdAndWorkDateAsync(employeeId, item.WorkDate);

                    if (overtime != null
                        && overtime.IsApproved
                        && item.CheckOut.HasValue)
                    {
                        var checkOutTime = item.CheckOut.Value.TimeOfDay;

                        // Convert TimeOnly → TimeSpan
                        var otFrom = overtime.From.ToTimeSpan();
                        var otTo = overtime.To.ToTimeSpan();

                        // Lấy thời điểm bắt đầu OT thực tế (max giữa checkout chuẩn và OT from)
                        var start = checkOutTime > otFrom ? otFrom : standardCheckOutTime;

                        // Lấy thời điểm kết thúc OT (không vượt quá OT To)
                        var end = checkOutTime < otTo ? checkOutTime : otTo;

                        if (end > start)
                        {
                            overtimeWorkingHours += (end - start).TotalHours;
                        }
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
