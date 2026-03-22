using AttendanceManagementApp.DTOs.Request;
using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Models;
using AttendanceManagementApp.Utils;

namespace AttendanceManagementApp.Services.Interface
{
    public interface IOvertimeService
    {
        Task<OvertimeRes> CreateOverTimeAsync(OvertimeCreateReq req);
        Task<OvertimeRes> UpdateOverTimeAsync(int id, OvertimeCreateReq req);
        Task<OvertimeRes> SoftDeleteOverTimeAsync(int id);
        Task<PagedResult<OvertimeRes>> GetOverTimesAsync(OvertimeFilterReq req, PaginationQuery query);
        Task<OvertimeRes> ApprovedOverTimeAsync(int id);
        Task<bool> ExistOverTimeAsync(int id, DateOnly workDate);
        Task<OverTime> GetOverTimeByEmployeeIdAndWorkDateAsync(int employeeId, DateOnly workDate);
    }
}
