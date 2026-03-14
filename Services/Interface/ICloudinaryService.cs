using Microsoft.AspNetCore.Http;
namespace AttendanceManagementApp.Services.Interface
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(IFormFile file);
    }
}
