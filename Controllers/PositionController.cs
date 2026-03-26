using AttendanceManagementApp.Configs;
using AttendanceManagementApp.DTOs.Request;
using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Services.Interface;
using AttendanceManagementApp.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceManagementApp.Controllers
{
    [ApiController]
    [Route("api/v1/positions")]
    public class PositionController : ControllerBase
    {
        private readonly IPositionService _positionService;

        public PositionController(IPositionService positionService)
        {
            _positionService = positionService;
        }

        [Authorize(Roles = Const.HR_ROLE_NAME)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PositionCreateReq positionCreateReq)
        {
            var position = await _positionService.CreatePositionAsync(positionCreateReq);
            return Ok(new ApiResponse<PositionRes>(position));
        }

        [Authorize(Roles = Const.HR_ROLE_NAME)]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationQuery query)
        {
            var positions = await _positionService.GetPositionsAsync(query);
            return Ok(new ApiResponse<PagedResult<PositionRes>>(positions));
        }

        [Authorize(Roles = Const.HR_ROLE_NAME)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PositionCreateReq positionCreateReq)
        {
            var position = await _positionService.UpdatePositionAsync(id, positionCreateReq);
            return Ok(new ApiResponse<PositionRes>(position));
        }

        [Authorize(Roles = Const.HR_ROLE_NAME)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _positionService.SoftDeletePositionAsync(id);
            return Ok(new ApiResponse<string>("Position deleted successfully"));
        }
    }
}
