using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Models;

namespace AttendanceManagementApp.Mappings
{
    public class OvertimeMapping
    {
        private readonly EmployeeMapping _employeeMapping;

        public OvertimeMapping(EmployeeMapping employeeMapping) { _employeeMapping = employeeMapping; }
        public OvertimeRes ToOverTimeRes(OverTime req)
        {
            if (req == null)
                return null;
            return new OvertimeRes
            {
                Id = req.Id,
                From = req.From,
                To = req.To,
                Reason = req.Reason,
                IsApproved = req.IsApproved,
                WorkDate = req.WorkDate,
                Employee = req.Employee != null ? _employeeMapping.ToEmployeeRes(req.Employee) : null
            };
        }
    }
}
