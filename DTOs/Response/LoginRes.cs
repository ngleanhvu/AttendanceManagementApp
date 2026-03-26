namespace AttendanceManagementApp.DTOs.Response
{
    public class LoginRes
    {
        public string Token { get; set; }
        public EmployeeRes Info { get; set; }
        public int AccountId { get; set; }
        public string Role {  get; set; }
       
    }
}
