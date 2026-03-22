using AttendanceManagementApp.DTOs.Request;
using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Utils;

namespace AttendanceManagementApp.Services.Interface
{
    public interface IPayrollService
    {
        Task CalculatePayrollAsync(PayrollCalculateReq req);
        Task ApprovePayrollAsync(PayrollCalculateReq req);
        Task<PagedResult<PayrollRes>> GetPayrollsAsync(PaginationQuery query, PayrollFilterReq filter);
        Task<FullPayrollDetailRes> GetPayrollDetailAsync(int id);
    }
}
