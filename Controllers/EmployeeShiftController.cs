
using AttendanceManagementApp.DTOs.Request;
using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Services.Interface;
using AttendanceManagementApp.Utils;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceManagementApp.Controllers
{
    [ApiController]
    [Route("api/v1/employee-shifts")]
    public class EmployeeShiftController : ControllerBase
    {
        private readonly IEmployeeShiftService _employeeShiftService;

        public EmployeeShiftController(IEmployeeShiftService employeeShiftService)
        {
            _employeeShiftService = employeeShiftService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EmployeeShiftCreateReq req)
        {
            await _employeeShiftService.CreateEmployeeShiftAsync(req);
            return Ok(new ApiResponse<string>("Employee shift created successfully"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EmployeeShiftUpdateReq req)
        {
            var result = await _employeeShiftService.UpdateEmploymentsAsync(id, req);
            return Ok(new ApiResponse<EmployeeShiftRes>(result));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _employeeShiftService.SoftDeleteEmployeeShiftAsync(id);
            return Ok(new ApiResponse<EmployeeShiftRes>(result));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _employeeShiftService.GetEmployeeShiftAsync(id);
            return Ok(new ApiResponse<EmployeeShiftRes>(result));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
                [FromQuery] PaginationQuery query,
                [FromQuery] EmployeeShiftFilter filter)
        {
            var result = await _employeeShiftService.GetEmployeeShiftsAsync(query, filter);

            return Ok(new ApiResponse<List<EmployeeShiftRes>>(result));
        }
    }
}
