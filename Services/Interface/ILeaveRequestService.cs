using AttendanceManagementApp.DTOs.Request;
using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Utils;

namespace AttendanceManagementApp.Services.Interface
{
    public interface ILeaveRequestService
    {
        Task<LeaveRequestRes> CreateLeaveRequestAsync(LeaveRequestCreateReq req);
        Task<PagedResult<LeaveRequestRes>> GetLeaveRequestsAsync(LeaveRequestFilterReq filter, PaginationQuery query);
        Task<LeaveRequestRes> SoftDeleteLeaveRequestAsync(int id);
        Task<LeaveRequestRes> UpdateLeaveStatusAsync(int id, LeaveRequestUpdateStatusReq req);
        Task<float> CalculateTotalLeavingAsync(int employeeId, int month, int year);
        Task<int> CalculateTotalLeavedDayAsync(int employeeId, int year);
    }
}
