using AttendanceManagementApp.DTOs.Request;
using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Models;
using AttendanceManagementApp.Utils;

namespace AttendanceManagementApp.Services.Interface
{
    public interface ILeaveTypeService
    {
        Task<LeaveTypeRes> CreateLeaveTypeAsync(LeaveTypeCreateReq req);
        Task<LeaveTypeRes> UpdateLeaveTypeAsync(int id, LeaveTypeCreateReq req);
        Task<PagedResult<LeaveTypeRes>> GetLeaveTypesAsync(PaginationQuery query);
        Task<LeaveType> GetLeaveTypeByIdAsync(int id);
        Task<LeaveType> SoftDeleteLeaveTypeAsync(int id);
    }
}
