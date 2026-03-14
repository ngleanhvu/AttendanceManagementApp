using AttendanceManagementApp.Models.Enum;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AttendanceManagementApp.Models
{
    [Index(nameof(Code), IsUnique = true)]
    public class Employee: BaseEntity
    {
        [Required]
        [MinLength(10), MaxLength(20)]
        public string Code { get; set; }

        [Required]
        public string Fullname { get; set; }

        [Required]
        public bool Gender { get; set; }

        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [Required]
        public string Thumbnail { get; set; }

        public EmployeeDetail EmployeeDetail { get; set; }

        public UserStatus UserStatus { get; set; }

        public ICollection<Contract> Contracts { get; set; }

        public ICollection<Attendance> Attendances { get; set; }

        public ICollection<EmployeeShift> EmployeeShifts { get; set; }
    }
}
