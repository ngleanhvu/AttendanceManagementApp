using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Mappings;
using AttendanceManagementApp.Models;

public class LeaveRequestMapping
{
    private readonly EmployeeMapping _employeeMapping;
    private readonly LeaveTypeMapping _leaveTypeMapping;

    public LeaveRequestMapping(EmployeeMapping employeeMapping, LeaveTypeMapping leaveTypeMapping)
    {
        _employeeMapping = employeeMapping;
        _leaveTypeMapping = leaveTypeMapping;
    }

    public LeaveRequestRes ToLeaveRequestRes(
        LeaveRequest req,
        int totalLeavingRequest,
        float leavedDays)
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
            LeaveStatus = (int)req.LeaveStatus,
            Employee = _employeeMapping.ToEmployeeRes(req.Employee),
            CreatedDate = req.CreatedDate,
            ApprovedDate = req.ApprovedDate,
            LeaveRequestType = (int)req.LeaveRequestType,

            // truyền từ ngoài vào
            TotalLeavingRequest = totalLeavingRequest,
            LeavedDays = leavedDays
        };
    }
}