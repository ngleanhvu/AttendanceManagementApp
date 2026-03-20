using AttendanceManagementApp.DTOs.Request;
using AttendanceManagementApp.DTOs.Response;

namespace AttendanceManagementApp.Services.Interface
{
    public interface IPayrollService
    {
        Task<PayrollRes> CalculatePayrollAsyncTest(int employeeId, PayrollCalculateReq req);
    }
}
