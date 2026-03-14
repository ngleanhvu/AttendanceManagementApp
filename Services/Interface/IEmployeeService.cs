using AttendanceManagementApp.DTOs.Request;
using AttendanceManagementApp.DTOs.Response;

namespace AttendanceManagementApp.Services.Interface
{
    public interface IEmployeeService
    {
        Task<EmployeeRes> CreateEmployeeAsync(EmployeeCreateReq req);
    }
}
