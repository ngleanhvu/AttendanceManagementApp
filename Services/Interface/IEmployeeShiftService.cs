using AttendanceManagementApp.DTOs.Request;
using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Utils;

namespace AttendanceManagementApp.Services.Interface
{
    public interface IEmployeeShiftService
    {
        Task CreateEmployeeShiftAsync(EmployeeShiftCreateReq req);
        Task<EmployeeShiftRes> UpdateEmploymentsAsync(int id, EmployeeShiftUpdateReq req);
        Task<EmployeeShiftRes> GetEmployeeShiftAsync(int id);
        Task<List<EmployeeShiftRes>> GetEmployeeShiftsAsync(PaginationQuery query, EmployeeShiftFilter filter);
        Task<EmployeeShiftRes> SoftDeleteEmployeeShiftAsync(int id);
    }
}
