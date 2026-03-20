using AttendanceManagementApp.DTOs.Request;
using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Utils;

namespace AttendanceManagementApp.Services.Interface
{
    public interface IAttendanceService
    {
        Task<AttendanceRes> CheckInAsync(AttendanceCheckInReq req);
        Task<AttendanceRes> CheckOutAsync(int attendanceId);
        Task<PagedResult<AttendanceRes>> GetAttendancesAsync(AttendanceFilterReq filter, PaginationQuery query);
        Task<AttendanceWorkloadRes> GetAttendanceWorkloadAsync(int employeeId, int month, int year);
    }
}
