using AttendanceManagementApp.Models;

namespace AttendanceManagementApp.DTOs.Response
{
    public class AttendanceRes
    {
        public int Id { get; set; }
        public DateOnly WorkDate { get; set; }
        public DateTime? CheckIn { get; set; }

        public DateTime? CheckOut { get; set; }
        public int AttendanceStatus { get; set; }
        public EmployeeRes Employee { get; set; }
    }
}
