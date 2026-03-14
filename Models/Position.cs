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

        public ICollection<Employee> Employees { get; set; }
    }
}
