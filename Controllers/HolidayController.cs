

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
    [Route("api/v1/holidays")]
    public class HolidayController : ControllerBase
    {
        private readonly IHolidayService _holidayService;

        public HolidayController(IHolidayService holidayService)
        {
            _holidayService = holidayService;
        }

        [Authorize(Roles = Const.HR_ROLE_NAME)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] HolidayCreateReq req)
        {
            var result = await _holidayService.CreateHolidayAsync(req);
            return Ok(new ApiResponse<HolidayRes>(result));
        }

        [Authorize(Roles = Const.HR_ROLE_NAME)]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _holidayService.GetHolidayAsync(id);
            return Ok(new ApiResponse<HolidayRes>(result));
        }

        [Authorize(Roles = Const.HR_ROLE_NAME)]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationQuery query)
        {
            var result = await _holidayService.GetHolidaysAsync(query);
            return Ok(new ApiResponse<PagedResult<HolidayRes>>(result));
        }

        [Authorize(Roles = Const.HR_ROLE_NAME)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] HolidayCreateReq req)
        {
            var result = await _holidayService.UpdateHolidayAsync(id, req);
            return Ok(new ApiResponse<HolidayRes>(result));
        }

        [Authorize(Roles = Const.HR_ROLE_NAME)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _holidayService.SoftDeleteHolidayAsync(id);
            return Ok(new ApiResponse<string>("Holiday deleted successfully"));
        }
    }
}
