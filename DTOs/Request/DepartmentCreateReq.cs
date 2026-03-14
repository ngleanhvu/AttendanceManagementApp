using System.ComponentModel.DataAnnotations;

namespace AttendanceManagementApp.DTOs.Request
{
    public class DepartmentCreateReq
    {
        [Required(ErrorMessage = "Department name is required.")]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
