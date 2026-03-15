using AttendanceManagementApp.DTOs.Request;
using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Utils;

namespace AttendanceManagementApp.Services.Interface
{
    public interface IShiftService
    {
        Task<ShiftRes> CreateShiftAsync(ShiftCreateReq req);
        Task<PagedResult<ShiftRes>> GetShiftsAsync(PaginationQuery query);
        Task<ShiftRes> UpdateShiftAsync(int id, ShiftCreateReq req);
        Task<ShiftRes> DeleteShiftAsync(int id);
        Task<ShiftRes> GetShiftAsync(int id);
    }
}
