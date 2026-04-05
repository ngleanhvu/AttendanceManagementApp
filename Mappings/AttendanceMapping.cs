using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Models;

namespace AttendanceManagementApp.Mappings
{
    public class AttendanceMapping
    {
        private readonly EmployeeMapping _employeeMapping;

        public AttendanceMapping(EmployeeMapping employeeMapping)
        {
            this._employeeMapping = employeeMapping;
        }
        public AttendanceRes ToAttendanceRes(Attendance attendance)
        {
            if (attendance == null)
                return null;
            return new AttendanceRes
            {
                Id = attendance.Id,
                WorkDate = attendance.WorkDate,
                CheckIn = attendance.CheckIn,
                CheckOut = attendance.CheckOut,
                AttendanceStatus = (int)attendance.AttendanceStatus,
                Employee = attendance.Employee != null ? _employeeMapping.ToEmployeeRes(attendance.Employee) : null,
            };
        }
    }
}
