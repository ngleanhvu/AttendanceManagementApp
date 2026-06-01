using AttendanceManagementApp.DTOs.Request;

namespace AttendanceManagementApp.Services.Interface;

public interface IEmployeeRecognitionService
{
    Task RegisterFaceAsync(EmployeeRecognitionCreateReq req);

    Task<string> CheckInByFaceAsync(EmployeeRecognitionCreateReq req);
    // Task CheckOutAsync(EmployeeRecognitionCreateReq req);
}