using AttendanceManagementApp.Models;
using AttendanceManagementApp.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace AttendanceManagementApp.DTOs.Response
{
    public class EmployeeDetailRes
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Email { get; set; }
        public bool Gender { get; set; }
        public UserStatus UserStatus { get; set; }
        public string Fullname { get; set; }
        public string Thumbnail { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string IdentityNumber { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public DateOnly HireDate { get; set; }
        public DepartmentRes Department { get; set; }
        public PositionRes Position { get; set; }
    }
}
