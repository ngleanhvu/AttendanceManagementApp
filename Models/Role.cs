using System.ComponentModel.DataAnnotations;

namespace AttendanceManagementApp.Models
{
    public class Role: BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        public ICollection<User> Users { get; set; }
    }
}
