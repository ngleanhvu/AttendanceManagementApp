using AttendanceManagementApp.DTOs.Request;
using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Utils;

namespace AttendanceManagementApp.Services.Interface
{
    public interface IPositionService
    {
        Task<PositionRes> CreatePositionAsync(PositionCreateReq request);

        Task<PositionRes> UpdatePositionAsync(int id, PositionCreateReq request);

        Task<PagedResult<PositionRes>> GetPositionsAsync(PaginationQuery query);

        Task<PositionRes> SoftDeletePositionAsync(int id);
    }
}