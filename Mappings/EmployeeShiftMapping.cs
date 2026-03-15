using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Models;

namespace AttendanceManagementApp.Mappings
{
    public class EmployeeShiftMapping
    {
        public EmployeeShiftRes ToEmployeeShiftRes(EmployeeShift employeeShift)
        {
            if (employeeShift == null) return null;
            return new EmployeeShiftRes
            {
                Id = employeeShift.Id,
                Employee = new EmployeeRes
                {
                    Id = employeeShift.Employee.Id,
                    Code = employeeShift.Employee.Code,
                    Fullname = employeeShift.Employee.Fullname,
                    Email = employeeShift.Employee.Email,
                    Thumbnail = employeeShift.Employee.Thumbnail,
                    Gender = employeeShift.Employee.Gender,
                    UserStatus = employeeShift.Employee.UserStatus,
                },
                FromDate = employeeShift.FromDate,
                ToDate = employeeShift.ToDate,
                Note = employeeShift.Note,
                AssignedAt = employeeShift.AssignedAt,
                AssignedBy = employeeShift.AssignedBy,
                IsActive = employeeShift.IsActive,
            };
        }
    }
}
