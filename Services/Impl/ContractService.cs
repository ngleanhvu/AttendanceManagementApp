using AttendanceManagementApp.Configs;
using AttendanceManagementApp.DTOs.Request;
using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Mappings;
using AttendanceManagementApp.Models;
using AttendanceManagementApp.Repositories;
using AttendanceManagementApp.Services.Interface;
using AttendanceManagementApp.Utils;
using Microsoft.EntityFrameworkCore;
using AttendanceManagementApp.Exception;

namespace AttendanceManagementApp.Services.Impl
{
    public class ContractService : IContractService
    {
        private readonly IRepository<Contract> _contractRepository;
        private readonly AppDbContext _context;
        private readonly IEmployeeService _employeeService;
        private readonly ContractMapping _contractMapping;

        public ContractService(IRepository<Contract> contractRepository, AppDbContext context,
            IEmployeeService employeeService, ContractMapping contractMapping)
        {
            _contractRepository = contractRepository;
            _context = context;
            this._employeeService = employeeService;
            this._contractMapping = contractMapping;
        }

        public async Task<ContractRes> CreateContractAsync(ContractCreateReq req)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(req.EmployeeId);
            var contract = new Contract
            {
                Employee = employee,
                StartDate = req.StartDate,
                EndDate = req.EndDate,
                ContractStatus = Models.Enum.ContractStatus.ACTIVE,
                ContractNumber = req.ContractNumber,
                ContractType = (Models.Enum.ContractType)req.ContractType,
                Description = req.Description,
                SignedBy = req.SignedBy,
                SignedDate = req.SignedDate,
                BaseSalary = req.BaseSalary,
                InsuranceSalary = req.InsuranceSalary,
                Allowance = req.Allowance
            };
            await _contractRepository.AddAsync(contract);
            await _contractRepository.SaveAsync();

            return _contractMapping.ToContractRes(contract);
        }

        public async Task<ContractRes> GetContractAsync(int id)
        {
            var contract = _contractRepository.GetByIdAsync(id).Result;
            return _contractMapping.ToContractRes(contract);
        }

        public async Task<PagedResult<ContractRes>> GetContractsAsync(PaginationQuery query)
        {
            var pagable = _context.Contracts
                .AsNoTracking()
                .ApplySearch(query.Search, c => c.ContractNumber,
                                   c => c.ContractType.ToString(),
                                   c => c.ContractStatus.ToString())
                .ApplySorting(query.SortBy, query.Desc);
            var total = await pagable.CountAsync();
            var items = await pagable.ApplyPagination(query.Page, query.PageSize)
                .Include(c => c.Employee)
                .ToListAsync();
            var itemsRes = items.Select(c => _contractMapping.ToContractRes(c)).ToList();
            return new PagedResult<ContractRes>
            {
                Total = total,
                Items = itemsRes,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        public async Task<PagedResult<ContractRes>> GetContractsByEmployeeIdAsync(int employeeId, PaginationQuery query)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(employeeId);
            var pagable = _context.Contracts
                .AsNoTracking()
                .Where(c => c.EmployeeId == employeeId)
                .ApplySearch(query.Search, c => c.ContractNumber,
                                   c => c.ContractType.ToString(),
                                   c => c.ContractStatus.ToString())
                .ApplySorting(query.SortBy, query.Desc);
            var total = await pagable.CountAsync();
            var items = await pagable.ApplyPagination(query.Page, query.PageSize)
                .ToListAsync();
            var itemsRes = items.Select(c => _contractMapping.ToContractRes(c)).ToList();
            return new PagedResult<ContractRes>
            {
                Total = total,
                Items = itemsRes,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        public async Task<ContractRes> UpdateContractAsync(int id, ContractCreateReq req)
        {
            var contract = _contractRepository.GetByIdAsync(id).Result;
            if (contract == null)
                throw new NotFoundException("Contract not found");
            contract.ContractNumber = req.ContractNumber;
            contract.ContractType = (Models.Enum.ContractType)req.ContractType;
            contract.StartDate = req.StartDate;
            contract.EndDate = req.EndDate;
            contract.BaseSalary = req.BaseSalary;
            contract.Allowance = req.Allowance;
            contract.InsuranceSalary = req.InsuranceSalary;
            contract.ContractStatus = (Models.Enum.ContractStatus)req.ContractStatus;
            contract.Description = req.Description;
            contract.SignedBy = req.SignedBy;
            contract.SignedDate = req.SignedDate;

            _contractRepository.Update(contract);
            await _contractRepository.SaveAsync();

            return _contractMapping.ToContractRes(contract);
        }

        public async Task<ContractRes> UpdateContractStatusAsync(int id, int contractStatus)
        {
            var contract = _contractRepository.GetByIdAsync(id).Result;
            if (contract == null)
                throw new NotFoundException("Contract not found");
            var status = (Models.Enum.ContractStatus)contractStatus;
            contract.ContractStatus = status;
            _contractRepository.Update(contract);
            await _contractRepository.SaveAsync();
            return _contractMapping.ToContractRes(contract);
        }
    }
}
