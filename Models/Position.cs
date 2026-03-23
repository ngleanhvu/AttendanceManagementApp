using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
namespace AttendanceManagementApp.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class Position: BaseEntity
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<EmployeeDetail> EmployeeDetails { get; set; }
    }
}
