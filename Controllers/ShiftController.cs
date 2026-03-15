using AttendanceManagementApp.DTOs.Request;
using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Services.Interface;
using AttendanceManagementApp.Utils;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceManagementApp.Controllers
{
    [ApiController]
    [Route("api/v1/shifts")]
    public class ShiftController : ControllerBase
    {
        private readonly IShiftService _shiftService;

        public ShiftController(IShiftService shiftService)
        {
            _shiftService = shiftService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ShiftCreateReq req)
        {
            var shift = await _shiftService.CreateShiftAsync(req);
            return Ok(new ApiResponse<ShiftRes>(shift));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ShiftCreateReq req)
        {
            var shift = await _shiftService.UpdateShiftAsync(id, req);
            return Ok(new ApiResponse<ShiftRes>(shift));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var shift = await _shiftService.DeleteShiftAsync(id);
            return Ok(new ApiResponse<ShiftRes>(shift));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var shift = await _shiftService.GetShiftAsync(id);
            return Ok(new ApiResponse<ShiftRes>(shift));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationQuery query)
        {
            var shifts = await _shiftService.GetShiftsAsync(query);
            return Ok(new ApiResponse<PagedResult<ShiftRes>>(shifts));
        }
    }
}
