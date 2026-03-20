using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Models;

namespace AttendanceManagementApp.Mappings
{
    public class LeaveRequestMapping
    {
        private readonly EmployeeMapping _employeeMapping;
        private readonly LeaveTypeMapping _leaveTypeMapping;

        public LeaveRequestMapping(EmployeeMapping employeeMapping, LeaveTypeMapping leaveTypeMapping)
        {
            this._employeeMapping = employeeMapping;
            this._leaveTypeMapping = leaveTypeMapping;
        }

        public LeaveRequestRes ToLeaveRequestRes(LeaveRequest req)
        {
            if (req == null)
                return null;
            return new LeaveRequestRes
            {
                Id = req.Id,
                FromDate = req.FromDate,
                ToDate = req.ToDate,
                TotalDays = req.TotalDays,
                Reason = req.Reason,
                RejectReason = req.RejectReason,
                LeaveStatus = req.LeaveStatus.ToString(),
                Employee = _employeeMapping.ToEmployeeRes(req.Employee),
                LeaveType = _leaveTypeMapping.ToLeaveTypeRes(req.LeaveType),
                CreatedDate = req.CreatedDate,
                ApprovedDate = req.ApprovedDate,
            };
        }
    }
}
