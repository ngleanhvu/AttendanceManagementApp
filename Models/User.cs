using System.ComponentModel.DataAnnotations;

namespace AttendanceManagementApp.Models
{
    public class User: BaseEntity
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(100, MinimumLength = 8)]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string PasswordHash { get; set; }
        public bool IsActive { get; set; }
        public virtual Role Role { get; set; }
    }
}
