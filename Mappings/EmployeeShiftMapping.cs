using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Models;

namespace AttendanceManagementApp.Mappings
{
    public class EmployeeShiftMapping
    {
        private readonly EmployeeMapping _employeeMapping;
        public EmployeeShiftRes ToEmployeeShiftRes(EmployeeShift employeeShift)
        {
            if (employeeShift == null) return null;
            Employee employee = employeeShift.Employee;
            return new EmployeeShiftRes
            {
                Id = employeeShift.Id,
                Employee = employee != null ? _employeeMapping.ToEmployeeRes(employee) : null,
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
