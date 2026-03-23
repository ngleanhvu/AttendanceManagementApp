using AttendanceManagementApp.Models;

namespace AttendanceManagementApp.Services.Interface
{
    public interface IJwtService
    {
        string GenerateToken(Account account);
    }
}
