using AttendanceManagementApp.DTOs.Request;
using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Utils;

namespace AttendanceManagementApp.Services.Interface
{
    public interface IHolidayService
    {
        Task<HolidayRes> CreateHolidayAsync(HolidayCreateReq req);
        Task<HolidayRes> UpdateHolidayAsync(int id, HolidayCreateReq req);
        Task<HolidayRes> GetHolidayAsync(int id);
        Task<PagedResult<HolidayRes>> GetHolidaysAsync(PaginationQuery query);
        Task<HolidayRes> SoftDeleteHolidayAsync(int id);
        Task<int> TotalHolidayAsync(int month, int year);
    }
}
