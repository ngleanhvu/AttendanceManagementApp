using AttendanceManagementApp.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace AttendanceManagementApp.DTOs.Request
{
    public class ShiftCreateReq
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "ShiftType is required")]
        public int ShiftType { get; set; }
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }
        [Required(ErrorMessage = "StartTime is required")]
        public TimeSpan StartTime { get; set; }
        [Required(ErrorMessage = "EndTime is required")]
        public TimeSpan EndTime { get; set; }
        [Required(ErrorMessage = "StandardHours is required")]
        public int StandardHours { get; set; } 
        [Required(ErrorMessage = "BreakDuration is required")]
        public TimeSpan? BreakStartTime { get; set; } 
        [Required(ErrorMessage = "BreakDuration is required")]
        public TimeSpan? BreakEndTime { get; set; } 
        [Required(ErrorMessage = "AllowedLateMinutes is required")]
        public int AllowedLateMinutes { get; set; } 
        [Required(ErrorMessage = "AllowedEarlyLeaveMinutes is required")]
        public int AllowedEarlyLeaveMinutes { get; set; } 
        [Required(ErrorMessage = "IsOvernight is required")]
        public bool IsOvernight { get; set; } 
        [Required(ErrorMessage = "IsActive is required")]
        public bool IsActive { get; set; } 
    }
}
