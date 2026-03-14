using AttendanceManagementApp.DTOs.Request;
using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Services.Interface;
using AttendanceManagementApp.Utils;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceManagementApp.Controllers
{
    [ApiController]
    [Route("api/v1/employees")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] EmployeeCreateReq req)
        {
            var employee = await _employeeService.CreateEmployeeAsync(req);
            return Ok(new ApiResponse<EmployeeRes>(employee));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] EmployeeUpdateReq req)
        {
            var employee = await _employeeService.UpdateEmployeeAsync(id, req);
            return Ok(new ApiResponse<EmployeeDetailRes>(employee));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var employee = await _employeeService.GetEmployeeAsync(id);
            return Ok(new ApiResponse<EmployeeDetailRes>(employee));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromBody] PaginationQuery query)
        {
            var employees = await _employeeService.GetEmployeesAsync(query);
            return Ok(new ApiResponse<PagedResult<EmployeeRes>>(employees));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var employee = await _employeeService.SoftDeleteEmployeeAsync(id);
            return Ok(new ApiResponse<EmployeeRes>(employee));
        }

    }
}
