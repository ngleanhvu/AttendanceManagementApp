using AttendanceManagementApp.DTOs.Request;
using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Models;
using AttendanceManagementApp.Utils;

namespace AttendanceManagementApp.Services.Interface
{
    public interface IDepartmentService
    {
        Task<PagedResult<DepartmentRes>> GetDepartmentsAsync(PaginationQuery query);
        Task<DepartmentRes> CreateDepartmentAsync(DepartmentCreateReq req);
        Task<DepartmentRes> UpdateDepartmentAsync(int id, DepartmentCreateReq req);
        Task<DepartmentRes> SoftDeleteDepartmentAsync(int id);
        Task<Department> GetDepartmentAsync(int id);
    }
}
