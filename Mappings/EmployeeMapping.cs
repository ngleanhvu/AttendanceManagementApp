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
    

    public EmployeeDetailRes ToEmployeeDetailRes(Employee employee)
        {
            return new EmployeeDetailRes
            {
                Id = employee.Id,
                Code = employee.Code,
                Email = employee.Email,
                Fullname = employee.Fullname,
                Gender = employee.Gender,
                UserStatus = employee.UserStatus,
                Thumbnail = employee.Thumbnail,
                DateOfBirth = employee.EmployeeDetail.DateOfBirth,
                IdentityNumber = employee.EmployeeDetail.IdentityNumber,
                Phone = employee.EmployeeDetail.Phone,
                HireDate = employee.EmployeeDetail.HireDate,
                Address = employee.EmployeeDetail.Address,
                Department = new DepartmentRes
                {
                    Id = employee.EmployeeDetail.Department.Id,
                    Name = employee.EmployeeDetail.Department.Name
                },
                Position = new PositionRes
                {
                    Id = employee.EmployeeDetail.Position.Id,
                    Name = employee.EmployeeDetail.Position.Name
                }
            };
        }
    }
}
