using Microsoft.EntityFrameworkCore;

namespace AttendanceManagementApp.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class Department: BaseEntity
    {
        [System.ComponentModel.DataAnnotations.Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<EmployeeDetail> EmployeeDetails { get; set; }
    }
}
    