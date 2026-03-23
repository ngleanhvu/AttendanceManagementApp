using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceManagementApp.Models
{
    [Index(nameof(Phone), IsUnique = true)]
    [Index(nameof(IdentityNumber), IsUnique = true)]
    public class EmployeeDetail: BaseEntity
    {
        public DateOnly DateOfBirth { get; set; }
        [MaxLength(20)]
        public string IdentityNumber { get; set; }
        public string Address { get; set; }
        [Required]
        [StringLength(10)]
        public string Phone { get; set; }
        public DateOnly HireDate { get; set; }
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
        public int PositionId { get; set; }
        public Position Position { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}