namespace AttendanceManagementApp.Services.Interface
{
    public interface IPasswordService
    {
        string Hash(string password);
        bool Verify(string hash, string raw);
    }
}
