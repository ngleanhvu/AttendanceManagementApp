using AttendanceManagementApp.DTOs.Request;
using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Services.Interface;
using AttendanceManagementApp.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceManagementApp.Controllers
{
    [Route("api/v1/attendances")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;

        public AttendanceController(IAttendanceService attendanceService)
        {
            this._attendanceService = attendanceService; 
        }

        [HttpPost]
        public async Task<IActionResult> CheckIn(AttendanceCheckInReq req)
        {
            var checkIn = await _attendanceService.CheckInAsync(req);
            return Ok(new ApiResponse<AttendanceRes>(checkIn));
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> CheckOut(int id)
        {
            var checkOut = await _attendanceService.CheckOutAsync(id);
            return Ok(new ApiResponse<AttendanceRes>(checkOut));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromBody] AttendanceFilterReq filter, [FromQuery] PaginationQuery query)
        {
            var attendances = await _attendanceService.GetAttendancesAsync(filter, query);
            return Ok(new ApiResponse<PagedResult<AttendanceRes>>(attendances));
        }
    }
}
