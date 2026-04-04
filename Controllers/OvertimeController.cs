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
    [Route("api/v1/overtimes")]
    public class OvertimeController : ControllerBase
    {
        private readonly IOvertimeService _overtimeService;

        public OvertimeController(IOvertimeService overtimeService)
        {
            _overtimeService = overtimeService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OvertimeCreateReq req)
        {
            var res = await _overtimeService.CreateOverTimeAsync(req);
            return Ok(new ApiResponse<OvertimeRes>(res));
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] OvertimeCreateReq req, int id)
        {
            var res = await _overtimeService.UpdateOverTimeAsync(id, req);
            return Ok(new ApiResponse<OvertimeRes>(res));
        }

        [Authorize(Roles = Const.HR_ROLE_NAME)]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] OvertimeFilterReq filter, [FromQuery] PaginationQuery query)
        {
            var res = await _overtimeService.GetOverTimesAsync(filter, query);
            return Ok(new ApiResponse<PagedResult<OvertimeRes>>(res));
        }

        [Authorize(Roles = Const.HR_ROLE_NAME)]
        [HttpPatch("{id}/status/{status}")]
        public async Task<IActionResult> Approve(int id, int status)
        { 
            var res = await _overtimeService.ApprovedOverTimeAsync(id, status);
            return Ok(new ApiResponse<OvertimeRes>(res));
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var res = await _overtimeService.SoftDeleteOverTimeAsync(id);
            return Ok(new ApiResponse<OvertimeRes>(res));
        }
    }
}
