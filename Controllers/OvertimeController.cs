using AttendanceManagementApp.DTOs.Request;
using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Services.Interface;
using AttendanceManagementApp.Utils;
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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OvertimeCreateReq req)
        {
            var res = await _overtimeService.CreateOverTimeAsync(req);
            return Ok(new ApiResponse<OvertimeRes>(res));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] OvertimeCreateReq req, int id)
        {
            var res = await _overtimeService.UpdateOverTimeAsync(id, req);
            return Ok(new ApiResponse<OvertimeRes>(res));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromBody] OvertimeFilterReq filter, [FromQuery] PaginationQuery query)
        {
            var res = await _overtimeService.GetOverTimesAsync(filter, query);
            return Ok(new ApiResponse<PagedResult<OvertimeRes>>(res));
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Approve(int id)
        { 
            var res = await _overtimeService.ApprovedOverTimeAsync(id);
            return Ok(new ApiResponse<OvertimeRes>(res));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var res = await _overtimeService.SoftDeleteOverTimeAsync(id);
            return Ok(new ApiResponse<OvertimeRes>(res));
        }
    }
}
