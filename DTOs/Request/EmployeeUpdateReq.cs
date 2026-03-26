using System.ComponentModel.DataAnnotations;

namespace AttendanceManagementApp.DTOs.Request
{
    public class EmployeeUpdateReq
    {
        [Required(ErrorMessage = "Fullname is required.")]
        public string Fullname { get; set; }
        [Required(ErrorMessage = "Gender is required")]
        public bool Gender { get; set; }
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        public IFormFile? Thumbnail { get; set; }
        [Required(ErrorMessage = "Date of Birth is required")]
        public DateOnly DateOfBirth { get; set; }
        [Required(ErrorMessage = "Identity Number is required")]
        public string IdentityNumber { get; set; }
        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Phone is required")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Hired Date is required")]
        public DateOnly HiredDate { get; set; }
        [Required(ErrorMessage = "Department is required")]
        public int DepartmentId { get; set; }
        [Required(ErrorMessage = "Position is required")]
        public int PositionId { get; set; }
        public int UserStatus { get; set; } = 1;
    }
}
