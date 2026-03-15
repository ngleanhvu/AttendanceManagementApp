using AttendanceManagementApp.Models;

namespace AttendanceManagementApp.DTOs.Response
{
    public class EmployeeShiftRes
    {
        public int Id { get; set; }
        public EmployeeRes Employee { get; set; }
        public DateOnly FromDate { get; set; }  
        public DateOnly ToDate { get; set; }    
        public bool IsActive { get; set; }      
        public string? Note { get; set; }      
        public int AssignedBy { get; set; }     
        public DateTime AssignedAt { get; set; } 
    }
}
