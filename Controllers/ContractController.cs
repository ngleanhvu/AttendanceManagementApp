using AttendanceManagementApp.DTOs.Request;
using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Models;
using AttendanceManagementApp.Services.Interface;
using AttendanceManagementApp.Utils;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceManagementApp.Controllers
{
    [ApiController]
    [Route("api/v1/contracts")]
    public class ContractController : ControllerBase
    {
        private readonly IContractService _contractService;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ContractCreateReq req)
        {
            var contract = await _contractService.CreateContractAsync(req);
            return Ok(new ApiResponse<ContractRes>(contract));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ContractCreateReq req)
        {
            var contract = await _contractService.UpdateContractAsync(id, req);
            return Ok(new ApiResponse<ContractRes>(contract));
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromQuery] int contractStatus)
        {
            var contract = await _contractService.UpdateContractStatusAsync(id, contractStatus);
            return Ok(new ApiResponse<ContractRes>(contract));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationQuery query)
        {
            var contracts = await _contractService.GetContractsAsync(query);
            return Ok(new ApiResponse<PagedResult<ContractRes>>(contracts));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var contract = await _contractService.GetContractAsync(id);
            return Ok(new ApiResponse<ContractRes>(contract));
        }

        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult> GetByEmployeeId(int employeeId, [FromQuery] PaginationQuery query)
        {
            var contracts = await _contractService.GetContractsByEmployeeIdAsync(employeeId, query);
            return Ok(new ApiResponse<PagedResult<ContractRes>>(contracts));
        }
}
