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
    [Route("api/v1/departments")]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DepartmentCreateReq req)
        {
            var result = await _departmentService.CreateDepartmentAsync(req);

            return Ok(new ApiResponse<DepartmentRes>(result));
        }

        [Authorize(Roles = Const.HR_ROLE_NAME)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] DepartmentCreateReq req)
        {
            var result = await _departmentService.UpdateDepartmentAsync(id, req);
            return Ok(new ApiResponse<DepartmentRes>(result));
        }

        [Authorize(Roles = Const.HR_ROLE_NAME)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _departmentService.SoftDeleteDepartmentAsync(id);
            return Ok(new ApiResponse<string>("Department deleted successfully"));
        }

        // [Authorize(Roles = Const.HR_ROLE_NAME)]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationQuery query)
        {
            var result = await _departmentService.GetDepartmentsAsync(query);
            return Ok(new ApiResponse<PagedResult<DepartmentRes>>(result));
        }
    }
}