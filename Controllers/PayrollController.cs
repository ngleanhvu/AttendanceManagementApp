using AttendanceManagementApp.Configs;
using AttendanceManagementApp.DTOs.Request;
using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Services.Interface;
using AttendanceManagementApp.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceManagementApp.Controllers
{
    [ApiController]
    [Route("api/v1/payrolls")]
    public class PayrollController: ControllerBase
    {
        private readonly IPayrollService _payrollService;

        public PayrollController(IPayrollService payrollService)
        {
            _payrollService = payrollService;
        }

        [Authorize(Roles = Const.HR_ROLE_NAME)]
        [HttpPatch("/approve")]
        public async Task<IActionResult> Approve(PayrollCalculateReq req)
        {
             await _payrollService.ApprovePayrollAsync(req);
            return Ok(new ApiResponse<string>("Approve successfully"));
        }

        [Authorize(Roles = Const.HR_ROLE_NAME)]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromBody] PayrollFilterReq req, [FromQuery] PaginationQuery query)
        {
            var res = await _payrollService.GetPayrollsAsync(query, req);
            return Ok(new ApiResponse<PagedResult<PayrollRes>>(res));
        }

        [Authorize(Roles = Const.HR_ROLE_NAME)]
        [HttpPost]
        public async Task<IActionResult> Calculate([FromBody] PayrollCalculateReq req)
        {
            await _payrollService.CalculatePayrollAsync(req);
            return Ok(new ApiResponse<string>("Calculate payroll successfully"));
        }

        [Authorize(Roles = Const.HR_ROLE_NAME)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var res = await _payrollService.GetPayrollDetailAsync(id);
            return Ok(new ApiResponse<FullPayrollDetailRes>(res));
        }
    }
}
