using AttendanceManagementApp.DTOs.Request;
using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Services.Interface;
using AttendanceManagementApp.Utils;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceManagementApp.Controllers
{
    [ApiController]
    [Route("api/v1/leave-requests")]
    public class LeaveRequestController: ControllerBase
    {
        private readonly ILeaveRequestService _leaveRequestService;

        public LeaveRequestController(ILeaveRequestService leaveRequestService)
        {
            this._leaveRequestService = leaveRequestService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LeaveRequestCreateReq req)
        {
            var res = await _leaveRequestService.CreateLeaveRequestAsync(req);
            return Ok(new ApiResponse<LeaveRequestRes>(res));
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> Approved([FromBody] LeaveRequestUpdateStatusReq req, int id)
        {
            var res = await _leaveRequestService.UpdateLeaveStatusAsync(id, req);
            return Ok(new ApiResponse<LeaveRequestRes>(res));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await _leaveRequestService.SoftDeleteLeaveRequestAsync(id);
            return Ok(new ApiResponse<LeaveRequestRes>(res));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromBody] LeaveRequestFilterReq filter, [FromQuery] PaginationQuery query)
        {
            var res = await _leaveRequestService.GetLeaveRequestsAsync(filter, query);
            return Ok(new ApiResponse<PagedResult<LeaveRequestRes>>(res));
        }
    }
}
