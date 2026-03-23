using AttendanceManagementApp.Configs;
using AttendanceManagementApp.DTOs.Request;
using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Models;
using AttendanceManagementApp.Services.Interface;
using AttendanceManagementApp.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceManagementApp.Controllers
{
    [ApiController]
    [Route("api/v1/leave-types")]
    public class LeaveTypeController : ControllerBase
    {
        private readonly ILeaveTypeService _leaveTypeService;

        public LeaveTypeController(ILeaveTypeService leaveTypeService)
        {
            _leaveTypeService = leaveTypeService;
        }

        [Authorize(Roles = Const.HR_ROLE_NAME)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LeaveTypeCreateReq req)
        {
            var res = await _leaveTypeService.CreateLeaveTypeAsync(req);
            return Ok(new ApiResponse<LeaveTypeRes>(res));
        }

        [Authorize(Roles = Const.HR_ROLE_NAME)]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromBody] PaginationQuery query)
        {
            var res = await _leaveTypeService.GetLeaveTypesAsync(query);
            return Ok(new ApiResponse<PagedResult<LeaveTypeRes>>(res));
        }

        [Authorize(Roles = Const.HR_ROLE_NAME)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] LeaveTypeCreateReq req)
        {
            var res = await _leaveTypeService.UpdateLeaveTypeAsync(id, req);
            return Ok(new ApiResponse<LeaveTypeRes>(res));
        }

        [Authorize(Roles = Const.HR_ROLE_NAME)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await _leaveTypeService.SoftDeleteLeaveTypeAsync(id);
            return Ok(new ApiResponse<LeaveType>(res));
        }
    }
}
