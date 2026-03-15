using AttendanceManagementApp.Models.Enum;

namespace AttendanceManagementApp.Models
{
    public class Shift: BaseEntity
    {
        public string Name { get; set; }

        public ShiftType ShiftType { get; set; } // Mã ca (VD: MORNING, NIGHT)

        public string Description { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public int StandardHours { get; set; } // Số giờ tiêu chuẩn của ca

        public TimeSpan? BreakStartTime { get; set; } // Giờ bắt đầu nghỉ

        public TimeSpan? BreakEndTime { get; set; } // Giờ kết thúc nghỉ

        public int AllowedLateMinutes { get; set; } // Cho phép đi trễ bao nhiêu phút

        public int AllowedEarlyLeaveMinutes { get; set; } // Cho phép về sớm bao nhiêu phút

        public bool IsOvernight { get; set; } // Ca qua ngày (ví dụ 22:00 - 06:00)

        public bool IsActive { get; set; } // Ca còn hoạt động không 

        public ICollection<EmployeeShift> EmployeeShifts { get; set; }

        public ICollection<Attendance> Attendances { get; set; }
    }
}
