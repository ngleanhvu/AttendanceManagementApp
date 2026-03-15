using AttendanceManagementApp.DTOs.Request;
using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Utils;

namespace AttendanceManagementApp.Services.Interface
{
    public interface IContractService
    {
        Task<ContractRes> CreateContractAsync(ContractCreateReq req);
        Task<PagedResult<ContractRes>> GetContractsAsync(PaginationQuery query);
        Task<ContractRes> GetContractAsync(int id);
        Task<PagedResult<ContractRes>> GetContractsByEmployeeIdAsync(int employeeId, PaginationQuery query);
        Task<ContractRes> UpdateContractAsync(int id, ContractCreateReq req);
        Task<ContractRes> UpdateContractStatusAsync(int id, int contractStatus);

    }
}
