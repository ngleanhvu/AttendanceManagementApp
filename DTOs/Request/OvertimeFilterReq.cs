namespace AttendanceManagementApp.DTOs.Request
{
    public class OvertimeFilterReq
    {
        public int? EmployeeId { get; set; }
        public DateOnly? WorkDate {  get; set; }
        public bool? IsApproved { get; set; }
    }
}
