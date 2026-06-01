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
        public readonly int HOUR_CHECK_IN = 8;
        public readonly int MINUTE_CHECK_IN = 30;
        public readonly int HOUR_CHECK_OUT = 17;
        public readonly int MINUTE_CHECK_OUT = 30;
        private readonly IEmployeeService _employeeService;
        private readonly IRepository<Attendance> _attendanceRepository;
        private readonly AppDbContext _appDbContext;
        private readonly AttendanceMapping _attendanceMapping;
        private readonly IOvertimeService _overtimeSerivce;
        private readonly ILeaveRequestService _leaveRequestService;

        public AttendanceService(IEmployeeService employeeService,
            IRepository<Attendance> attendanceRepository, AppDbContext appDbContext,
            AttendanceMapping attendanceMapping, IOvertimeService overtimeSerivce,
            ILeaveRequestService leaveRequestService)
        {
            this._employeeService = employeeService;
            this._appDbContext = appDbContext;
            this._attendanceRepository = attendanceRepository;
            this._attendanceMapping = attendanceMapping;
            _overtimeSerivce = overtimeSerivce;
            _leaveRequestService = leaveRequestService;
        }

        public async Task<AttendanceRes> CheckInAsync(AttendanceCheckInReq req)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(req.EmployeeId);
            var now = DateTime.Now;
            var today = DateOnly.FromDateTime(now);

            // Rule: thời gian hợp lệ
            if (now.TimeOfDay < new TimeSpan(6, 0, 0))
                throw new BadRequestException("Thời gian quá sớm để checkin");

            // Rule: weekend
            if (today.DayOfWeek == DayOfWeek.Saturday || today.DayOfWeek == DayOfWeek.Sunday)
                throw new BadRequestException("Không thể checkin vào thứ 7 hoặc chủ nhật");

            // Lấy attendance hôm nay (nếu có)
            var attendance = await _appDbContext.Attendances
                .FirstOrDefaultAsync(x => x.Employee.Id == req.EmployeeId && x.WorkDate == today);

            // Nếu đã tồn tại => xử lý CHECK-OUT
            if (attendance != null)
            {
                if (attendance.CheckOut != null)
                    throw new BadRequestException("Nhân viên đã checkout");

                attendance.CheckOut = now;

                await _attendanceRepository.SaveAsync();

                return _attendanceMapping.ToAttendanceRes(attendance);
            }
            
            var res = await _leaveRequestService.GetLeaveRequestsAsync(
                new LeaveRequestFilterReq
                {
                    FromDate = now,
                    ToDate = now,
                    EmployeeId = employee.Id
                },
                new PaginationQuery { PageSize = 1 });

            var standardTime = new TimeSpan(HOUR_CHECK_IN, MINUTE_CHECK_IN, 0);
            bool isLate = now.TimeOfDay > standardTime;

            var newAttendance = new Attendance
            {
                WorkDate = today,
                CheckIn = now,
                Employee = employee,
                AttendanceStatus = isLate ? AttendanceStatus.LATE : AttendanceStatus.PRESENT
            };

            await _attendanceRepository.AddAsync(newAttendance);
            await _attendanceRepository.SaveAsync();

            return _attendanceMapping.ToAttendanceRes(newAttendance);
        }

        public async Task<AttendanceRes> CheckOutAsync(int attendanceId)
        {
            var attendance = await _attendanceRepository.GetByIdAsync(attendanceId);
            if (attendance.CheckOut.HasValue == true)
            {
                throw new BadRequestException("Nhân viên đã checkout");
            }

            attendance.CheckOut = DateTime.Now;
            _attendanceRepository.Update(attendance);
            await _attendanceRepository.SaveAsync();
            return _attendanceMapping.ToAttendanceRes(attendance);
        }

        public async Task<PagedResult<AttendanceRes>> GetAttendancesAsync(
            AttendanceFilterReq req,
            PaginationQuery query)
        {
            var pageable = _appDbContext.Attendances
                .AsNoTracking()
                .Where(x => x.Status == true)
                .Include(x => x.Employee)
                .AsQueryable();

            if (req.AttendanceStatus.HasValue)
            {
                pageable = pageable.Where(x =>
                    x.AttendanceStatus == (AttendanceStatus)req.AttendanceStatus.Value);
            }

            if (req.FromDate.HasValue)
            {
                pageable = pageable.Where(x => x.WorkDate >= req.FromDate.Value);
            }

            if (req.ToDate.HasValue)
            {
                pageable = pageable.Where(x => x.WorkDate <= req.ToDate.Value);
            }

            if (req.Month.HasValue)
            {
                pageable = pageable.Where(x => x.WorkDate.Month == req.Month.Value);
            }

            if (req.Year.HasValue)
            {
                pageable = pageable.Where(x => x.WorkDate.Year == req.Year.Value);
            }

            if (req.EmployeeId.HasValue)
            {
                pageable = pageable.Where(x => x.EmployeeId == req.EmployeeId.Value);
            }

            var count = await pageable.CountAsync();

            var items = await pageable
                .OrderByDescending(x => x.WorkDate)
                .ThenByDescending(x => x.Id)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(x => _attendanceMapping.ToAttendanceRes(x))
                .ToListAsync();

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
            double totalWorkingDays = 0;
            double overtimeWorkingHours = 0;
            int totalCheckInLate = 0;

            var filter = new AttendanceFilterReq
            {
                EmployeeId = employeeId,
                Month = month,
                Year = year
            };

            var query = new PaginationQuery { PageSize = 1000 }; 
            var res = await GetAttendancesAsync(filter, query);

            if (res?.Items == null || !res.Items.Any())
            {
                return new AttendanceWorkloadRes();
            }

            foreach (var item in res.Items)
            {
                // Không đủ dữ liệu → bỏ
                if (!item.CheckIn.HasValue || !item.CheckOut.HasValue)
                    continue;

                var checkIn = item.CheckIn.Value;
                var checkOut = item.CheckOut.Value;

                var workedHours = (checkOut - checkIn).TotalHours;
                
                // 1. TÍNH NGÀY CÔNG
                if (workedHours >= 7)
                {
                    totalWorkingDays += 1;
                }
                else if (workedHours >= 3)
                {
                    totalWorkingDays += 0.5;
                }
               
                // ĐI TRỄ
                if (item.AttendanceStatus == (int)AttendanceStatus.LATE && workedHours >= 3)
                {
                    totalCheckInLate++;
                }
                
                // OVERTIME (chuẩn hơn)
                var overtime = await _overtimeSerivce
                    .GetOverTimeByEmployeeIdAndWorkDateAsync(employeeId, item.WorkDate);

                if (overtime != null)
                {
                    var otFrom = overtime.From.ToTimeSpan();
                    var otTo = overtime.To.ToTimeSpan();

                    var actualCheckOut = checkOut.TimeOfDay;

                    // OT bắt đầu sau giờ làm chuẩn (17h30 chẳng hạn)
                    var standardEnd = new TimeSpan(HOUR_CHECK_OUT, MINUTE_CHECK_OUT, 0);

                    // Start = max(standardEnd, OT From)
                    var start = otFrom > standardEnd ? otFrom : standardEnd;

                    // End = min(CheckOut, OT To)
                    var end = actualCheckOut < otTo ? actualCheckOut : otTo;

                    if (end > start)
                    {
                        overtimeWorkingHours += (end - start).TotalHours;
                    }
                }
            }

            return new AttendanceWorkloadRes
            {
                TotalWorkingDays = (int)Math.Round(totalWorkingDays, 2),
                TotalCheckInLates = totalCheckInLate,
                OvertimeWorkingHours = (float)Math.Round(overtimeWorkingHours, 2)
            };
        }
    }
}