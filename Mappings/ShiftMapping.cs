using AttendanceManagementApp.DTOs.Response;

namespace AttendanceManagementApp.Mappings
{
    public class ShiftMapping
    {
        public ShiftRes ToShiftRes(Models.Shift shift)
        {
            return new ShiftRes
            {
                Id = shift.Id,
                Name = shift.Name,
                ShiftType = shift.ShiftType,
                Description = shift.Description,
                StartTime = shift.StartTime,
                EndTime = shift.EndTime,
                StandardHours = shift.StandardHours,
                BreakStartTime = shift.BreakStartTime,
                BreakEndTime = shift.BreakEndTime,
                AllowedLateMinutes = shift.AllowedLateMinutes,
                AllowedEarlyLeaveMinutes = shift.AllowedEarlyLeaveMinutes,
                IsOvernight = shift.IsOvernight,
                IsActive = shift.IsActive
            };
        }
    }
}
