using AttendanceManagementApp.DTOs.Request;
using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Models.Enum;
using AttendanceManagementApp.Services.Interface;

namespace AttendanceManagementApp.Services.Impl
{
    public class EmployeeService : IEmployeeService
    {
        public Task<EmployeeRes> CreateEmployeeAsync(EmployeeCreateReq req)
        {
            var employee = new EmployeeRes
            {
                Code = "EM"+req.IdentityNumber,
                Email = req.Email,
                Fullname = req.Fullname,
                Gender = req.Gender,
                UserStatus = (UserStatus)req.UserStatus,
            }
        }
    }
}
