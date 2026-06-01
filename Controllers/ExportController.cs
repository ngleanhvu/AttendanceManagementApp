using AttendanceManagementApp.Configs;
using AttendanceManagementApp.Services.Interface;
using AttendanceManagementApp.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceManagementApp.Controllers;

[ApiController]
[Route("api/v1/exports")]
public class ExportController: ControllerBase
{
    private readonly IExportService _exportService;
    
    public ExportController(IExportService exportService)
    {
        _exportService = exportService;
    }

    [Authorize(Roles = Const.HR_ROLE_NAME)]
    [HttpGet("{payrollId}")] public async Task<IActionResult> ExportPayrollPdfAsync(int payrollId) 
    { var file = await _exportService.ExportPayrollPdfAsync(payrollId); 
        return File(file, "application/pdf", $"payslip-{payrollId}.pdf"); }
}