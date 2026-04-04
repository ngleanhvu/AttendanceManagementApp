namespace AttendanceManagementApp.DTOs.Response
{
    public class OvertimeRes
    {
        public int Id { get; set; }
        public DateOnly WorkDate { get; set; }
        public TimeOnly From { get; set; }
        public TimeOnly To { get; set; }
        public string Reason { get; set; }
        public int IsApproved { get; set; }
        public EmployeeRes Employee { get; set; }
    }
}
