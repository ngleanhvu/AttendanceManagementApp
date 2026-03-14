using AttendanceManagementApp.Models.Enum;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AttendanceManagementApp.Models
{
    [Index(nameof(Code), IsUnique = true)]
    [Index(nameof(Phone), IsUnique =true)]
    public class Employee: BaseEntity
    {
        [Required]
        [MinLength(10), MaxLength(20)]
        public string Code { get; set; }

        [Required]
        public string Fullname { get; set; }

        [Required]
        public bool Gender { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [StringLength(10)]
        public string Phone { get; set; }

        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public DateTime HireDate { get; set; }

        public UserStatus UserStatus { get; set; }

        public Department Department { get; set; }

        public Position Position { get; set; }

        public ICollection<Contract> Contracts { get; set; }

        public ICollection<Attendance> Attendances { get; set; }

        public ICollection<EmployeeShift> EmployeeShifts { get; set; }
    }
}
