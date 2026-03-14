using AttendanceManagementApp.Models.Enum;

namespace AttendanceManagementApp.DTOs.Response
{
    public class EmployeeRes
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Email { get; set; }
        public bool Gender { get; set; }
        public UserStatus UserStatus { get; set; }
        public string Fullname { get; set; }
        public string Thumbnail { get; set; }
    }
}
