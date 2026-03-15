using AttendanceManagementApp.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace AttendanceManagementApp.DTOs.Response
{
    public class ShiftRes
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ShiftType ShiftType { get; set; }
        public string Description { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int StandardHours { get; set; }
        public TimeSpan? BreakStartTime { get; set; }
        public TimeSpan? BreakEndTime { get; set; }
        public int AllowedLateMinutes { get; set; }
        public int AllowedEarlyLeaveMinutes { get; set; }
        public bool IsOvernight { get; set; }
        public bool IsActive { get; set; }
    }
}
