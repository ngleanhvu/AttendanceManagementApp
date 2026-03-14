using System.ComponentModel.DataAnnotations;

namespace AttendanceManagementApp.DTOs.Request
{
    public class PositionCreateReq
    {
        [Required(ErrorMessage = "Position name is required.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Position description is required.")]
        public string description { get; set; }
    }
}
