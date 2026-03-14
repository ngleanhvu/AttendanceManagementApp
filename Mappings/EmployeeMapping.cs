using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Models;

namespace AttendanceManagementApp.Mappings
{
    public class EmployeeMapping
    {
        public EmployeeRes ToEmployeeRes(Employee employee)
        {
            return new EmployeeRes
            {
                Id = employee.Id,
                Code = employee.Code,
                Email = employee.Email,
                Fullname = employee.Fullname,
                Gender = employee.Gender,
                UserStatus = employee.UserStatus,
                Thumbnail = employee.Thumbnail
            };
        }
    }
}
