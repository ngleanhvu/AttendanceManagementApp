using AttendanceManagementApp.DTOs.Request;
using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Utils;

namespace AttendanceManagementApp.Services.Interface
{
    public interface IEmployeeService
    {
        Task<EmployeeRes> CreateEmployeeAsync(EmployeeCreateReq req);
        Task<EmployeeDetailRes> UpdateEmployeeAsync(int id, EmployeeUpdateReq req);
        Task<PagedResult<EmployeeRes>> GetEmployeesAsync(PaginationQuery query);
        Task<EmployeeDetailRes> GetEmployeeAsync(int id);
        Task<EmployeeRes> SoftDeleteEmployeeAsync(int id);
    }
}
