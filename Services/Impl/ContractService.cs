using AttendanceManagementApp.Configs;
using AttendanceManagementApp.DTOs.Request;
using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Exception;
using AttendanceManagementApp.Mappings;
using AttendanceManagementApp.Models;
using AttendanceManagementApp.Repositories;
using AttendanceManagementApp.Services.Interface;
using AttendanceManagementApp.Utils;
using Microsoft.EntityFrameworkCore;


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

        public async Task<ContractRes> ActiveContractByEmployeeIdAsync(int contractId, int employeeId)
        {
            var contract = await _contractRepository.GetByIdAsync(contractId);
            if (contract == null)
            {
                throw new NotFoundException("Contract not found");
            }
            var contractActive = await GetContractActiveByEmployeeIdAsync(employeeId);
            if (contractActive != null)
            {
                contractActive.ContractStatus = Models.Enum.ContractStatus.NOT_ACTIVE;
                _contractRepository.Update(contractActive);
            }
            contract.ContractStatus = Models.Enum.ContractStatus.ACTIVE;
            _contractRepository.Update(contract);
            await _contractRepository.SaveAsync();
            return _contractMapping.ToContractRes(contract);
        }

        public async Task<int> CalculateTotalLeavingBaseContractsAsync(int employeeId)
        {
            var contracts = await _context.Contracts
                .Where(x => x.ContractStatus == Models.Enum.ContractStatus.ACTIVE 
                || x.ContractStatus == Models.Enum.ContractStatus.EXPIRED)
                .ToListAsync();

            if (contracts == null || !contracts.Any())
                return 0;

            int totalDays = 0;

            foreach (var c in contracts)
            {
                var end = c.EndDate == default
                    ? DateOnly.FromDateTime(DateTime.Now)
                    : c.EndDate;

                if (end < c.StartDate)
                    continue;

                totalDays += (end.DayNumber - c.StartDate.DayNumber);
            }

            int totalYears = totalDays / 365;

            return 12 + (totalYears / 12);
        }

        public async Task<ContractRes> CreateContractAsync(ContractCreateReq req)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(req.EmployeeId);
            var contract = new Models.Contract
            {
                Employee = employee,
                StartDate = req.StartDate,
                EndDate = req.EndDate,
                ContractStatus = Models.Enum.ContractStatus.NOT_ACTIVE,
                ContractNumber = req.ContractNumber,
                ContractType = (Models.Enum.ContractType)req.ContractType,
                Description = req.Description,
                SignedDate = req.SignedDate,
                BaseSalary = req.BaseSalary,
                AllowanceLunchBreak = req.AllowanceLunchBreak,
                AllowancePark = req.AllowancePark,
                Tax = req.Tax,
                OverTimeRate = req.OverTimeRate,
                TotalLeavingsPerMonth = req.TotalLeavingsPerMonth,
                WorkingPerMonth = req.WorkingPerMonth,
            };
            await _contractRepository.AddAsync(contract);
            await _contractRepository.SaveAsync();

            return _contractMapping.ToContractRes(contract);
        }

        public async Task<Contract> GetContractActiveByEmployeeIdAsync(int employeeId)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(employeeId);
            var contractActive = await _context.Contracts.Where(x => x.Status == true).FirstAsync();
            return contractActive;
        }

        public async Task<ContractRes> GetContractAsync(int id)
        {
            var contract = _context.Contracts
                .AsNoTracking()
                .Include(c => c.Employee)
                .FirstOrDefault(c => c.Id == id);
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
                .Where(c => c.Status == true)
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
            var contract = _context.Contracts
                .Include(c => c.Employee)
                .FirstOrDefault(c => c.Id == id);
            if (contract == null)
                throw new NotFoundException("Contract not found");
            contract.ContractNumber = req.ContractNumber;
            contract.ContractType = (Models.Enum.ContractType)req.ContractType;
            contract.StartDate = req.StartDate;
            contract.EndDate = req.EndDate;
            contract.BaseSalary = req.BaseSalary;
            contract.AllowancePark = req.AllowancePark;
            contract.AllowanceLunchBreak = req.AllowanceLunchBreak;
            contract.TotalLeavingsPerMonth = req.TotalLeavingsPerMonth;
            contract.Tax = req.Tax;
            contract.ContractStatus = (Models.Enum.ContractStatus)req.ContractStatus;
            contract.Description = req.Description;
            contract.OverTimeRate = req.OverTimeRate;
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
