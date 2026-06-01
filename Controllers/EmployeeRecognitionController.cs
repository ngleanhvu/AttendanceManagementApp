using AttendanceManagementApp.DTOs.Request;
using AttendanceManagementApp.Services.Interface;
using AttendanceManagementApp.Utils;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceManagementApp.Controllers;

[ApiController]
[Route("api/v1/recognitions")]
public class EmployeeRecognitionController:  ControllerBase
{
    private readonly IEmployeeRecognitionService _employeeRecognitionService;

    public EmployeeRecognitionController(IEmployeeRecognitionService employeeRecognitionService)
    {
        _employeeRecognitionService = employeeRecognitionService;
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] EmployeeRecognitionCreateReq req)
    {
         await _employeeRecognitionService.RegisterFaceAsync(req);
        return Ok(new ApiResponse<string>("Register face successfully"));
    }

    [HttpPost("check-in")]
    public async Task<IActionResult> CheckIn([FromBody] EmployeeRecognitionCreateReq req)
    {
        var res = await _employeeRecognitionService.CheckInByFaceAsync(req);
        return Ok(new ApiResponse<string>(res));
    }
}