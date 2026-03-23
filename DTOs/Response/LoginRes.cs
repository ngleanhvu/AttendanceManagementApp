namespace AttendanceManagementApp.DTOs.Response
{
    public class LoginRes
    {
        public string Token { get; set; }
        public int EmployeeId { get; set; }
        public int AccountId { get; set; }
        public string Role {  get; set; }
    }
}
