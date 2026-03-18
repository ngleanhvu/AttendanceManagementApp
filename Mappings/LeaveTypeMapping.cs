using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Models;

namespace AttendanceManagementApp.Mappings
{
    public class LeaveTypeMapping
    {
        public LeaveTypeRes ToLeaveTypeRes(LeaveType leaveType)
        {
            if (leaveType == null)
                return null;
            return new LeaveTypeRes
            {
                Id = leaveType.Id,
                Name = leaveType.Name,
                Description = leaveType.Description,
                MaxDaysPerMonth = leaveType.MaxDaysPerMonth,
                MaxDaysPerYear = leaveType.MaxDaysPerYear,
                IsPaid = leaveType.IsPaid,
                RequiresApproval = leaveType.RequiresApproval
            };
        }
    }
}
