using System.ComponentModel.DataAnnotations;

namespace AttendanceManagementApp.Models
{
    public class Account: BaseEntity
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(100, MinimumLength = 8)]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string PasswordHash { get; set; }
        public bool IsActive { get; set; }
        public int PositionId { get; set; }
        public Position Position { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
