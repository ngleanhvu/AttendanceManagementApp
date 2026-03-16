using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Models;

namespace AttendanceManagementApp.Mappings
{
    public class AttendanceMapping
    {
        private readonly EmployeeMapping _employeeMapping;
        private readonly ShiftMapping _shiftMapping;

        public AttendanceMapping(EmployeeMapping employeeMapping, ShiftMapping shiftMapping)
        {
            this._employeeMapping = employeeMapping;
            this._shiftMapping = shiftMapping;
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
                Note = attendance.Note,
                Employee = attendance.Employee != null ? _employeeMapping.ToEmployeeRes(attendance.Employee) : null,
                Shift = attendance.Shift != null ? _shiftMapping.ToShiftRes(attendance.Shift) : null
            };
        }
    }
}
