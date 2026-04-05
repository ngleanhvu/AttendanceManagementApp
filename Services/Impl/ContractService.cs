using AttendanceManagementApp.Configs;
using AttendanceManagementApp.DTOs.Request;
using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Exception;
using AttendanceManagementApp.Mappings;
using AttendanceManagementApp.Models;
using AttendanceManagementApp.Models.Enum;
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

        // ================= ACTIVE =================
        public async Task<ContractRes> ActiveContractByEmployeeIdAsync(int contractId, int employeeId)
        {
            var contract = await _context.Contracts
                .FirstOrDefaultAsync(c => c.Id == contractId && c.EmployeeId == employeeId);

            if (contract == null)
                throw new NotFoundException("Contract not found");

            if (contract.ContractStatus == Models.Enum.ContractStatus.ACTIVE)
                throw new BadRequestException("Contract is already active");

            var currentActive = await _context.Contracts
                .FirstOrDefaultAsync(c => c.EmployeeId == employeeId &&
                    c.ContractStatus == Models.Enum.ContractStatus.ACTIVE);

            if (currentActive != null)
            {
                currentActive.ContractStatus = Models.Enum.ContractStatus.EXPIRED;
            }

            contract.ContractStatus = Models.Enum.ContractStatus.ACTIVE;

            await _context.SaveChangesAsync();

            return _contractMapping.ToContractRes(contract);
        }

        // ================= LEAVE =================
        public async Task<int> CalculateTotalLeavingBaseContractsAsync(int employeeId)
        {
            var contracts = await _context.Contracts
                .Where(c => c.EmployeeId == employeeId &&
                    (c.ContractStatus == Models.Enum.ContractStatus.ACTIVE ||
                     c.ContractStatus == Models.Enum.ContractStatus.EXPIRED))
                .ToListAsync();

            if (!contracts.Any()) return 0;

            int totalDays = 0;

            foreach (var c in contracts)
            {
                var end = c.EndDate == default
                    ? DateOnly.FromDateTime(DateTime.Now)
                    : c.EndDate;

                if (end < c.StartDate) continue;

                totalDays += (end.DayNumber - c.StartDate.DayNumber);
            }

            int totalYears = totalDays / 365;

            // 12 ngày + mỗi 5 năm thêm 1 ngày
            return 12 + (totalYears / 5);
        }

        // ================= CREATE =================
        public async Task<ContractRes> CreateContractAsync(ContractCreateReq req)
        {
            // validate
            if (req.EndDate < req.StartDate)
                throw new BadRequestException("EndDate must be after StartDate");

            var exists = await _context.Contracts
                .AnyAsync(c => c.ContractNumber == req.ContractNumber);

            if (exists)
                throw new BadRequestException("Contract number already exists");

            // check overlap
            var overlap = await _context.Contracts
                .Where(c => c.EmployeeId == req.EmployeeId)
                .AnyAsync(c =>
                    req.StartDate <= c.EndDate &&
                    req.EndDate >= c.StartDate
                );

            if (overlap)
                throw new BadRequestException("Contract time overlaps existing contract");

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

        // ================= GET ACTIVE =================
        public async Task<Contract> GetContractActiveByEmployeeIdAsync(int employeeId)
        {
            return await _context.Contracts
                .FirstOrDefaultAsync(c => c.EmployeeId == employeeId &&
                    c.ContractStatus == ContractStatus.ACTIVE);
        }

        // ================= GET ONE =================
        public async Task<ContractRes> GetContractAsync(int id)
        {
            var contract = await _context.Contracts
                .AsNoTracking()
                .Include(c => c.Employee)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contract == null)
                throw new NotFoundException("Contract not found");

            return _contractMapping.ToContractRes(contract);
        }

        // ================= LIST =================
        public async Task<PagedResult<ContractRes>> GetContractsAsync(PaginationQuery query)
        {
            var pagable = _context.Contracts
                .AsNoTracking()
                .Where(c => c.Status == true)
                .ApplySearch(query.Search, c => c.ContractNumber,
                                   c => c.ContractType.ToString(),
                                   c => c.ContractStatus.ToString())
                .ApplySorting(query.SortBy, query.Desc);

            var total = await pagable.CountAsync();

            var items = await pagable
                .ApplyPagination(query.Page, query.PageSize)
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
            await _employeeService.GetEmployeeByIdAsync(employeeId);

            var pagable = _context.Contracts
                .AsNoTracking()
                .Where(c => c.EmployeeId == employeeId && c.Status == true)
                .ApplySearch(query.Search, c => c.ContractNumber,
                                   c => c.ContractType.ToString(),
                                   c => c.ContractStatus.ToString())
                .ApplySorting(query.SortBy, query.Desc);

            var total = await pagable.CountAsync();

            var items = await pagable
                .ApplyPagination(query.Page, query.PageSize)
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

        // ================= DELETE =================
        public async Task<ContractRes> SoftDeleteContractAsync(int id)
        {
            var contract = await _contractRepository.GetByIdAsync(id);

            if (contract == null)
                throw new NotFoundException("Contract not found");

            if (contract.ContractStatus == Models.Enum.ContractStatus.ACTIVE)
                throw new BadRequestException("Cannot delete active contract");

            contract.Status = false;

            await _contractRepository.SaveAsync();

            return _contractMapping.ToContractRes(contract);
        }

        // ================= UPDATE =================
        public async Task<ContractRes> UpdateContractAsync(int id, ContractCreateReq req)
        {
            var contract = await _context.Contracts
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contract == null)
                throw new NotFoundException("Contract not found");

            if (contract.ContractStatus == Models.Enum.ContractStatus.ACTIVE)
                throw new BadRequestException("Cannot update active contract");

            if (req.EndDate < req.StartDate)
                throw new BadRequestException("EndDate must be after StartDate");

            var exists = await _context.Contracts
                .AnyAsync(c => c.ContractNumber == req.ContractNumber && c.Id != id);

            if (exists)
                throw new BadRequestException("Contract number already exists");

            // check overlap
            var overlap = await _context.Contracts
                .Where(c => c.EmployeeId == contract.EmployeeId && c.Id != id)
                .AnyAsync(c =>
                    req.StartDate <= c.EndDate &&
                    req.EndDate >= c.StartDate
                );

            if (overlap)
                throw new BadRequestException("Contract time overlaps existing contract");

            contract.ContractNumber = req.ContractNumber;
            contract.ContractType = (Models.Enum.ContractType)req.ContractType;
            contract.StartDate = req.StartDate;
            contract.EndDate = req.EndDate;
            contract.BaseSalary = req.BaseSalary;
            contract.AllowancePark = req.AllowancePark;
            contract.AllowanceLunchBreak = req.AllowanceLunchBreak;
            contract.TotalLeavingsPerMonth = req.TotalLeavingsPerMonth;
            contract.Tax = req.Tax;
            contract.Description = req.Description;
            contract.OverTimeRate = req.OverTimeRate;
            contract.SignedDate = req.SignedDate;

            await _contractRepository.SaveAsync();

            return _contractMapping.ToContractRes(contract);
        }

        // ================= UPDATE STATUS =================
        public async Task<ContractRes> UpdateContractStatusAsync(int id, int contractStatus)
        {
            var contract = await _contractRepository.GetByIdAsync(id);

            if (contract == null)
                throw new NotFoundException("Contract not found");

            var status = (Models.Enum.ContractStatus)contractStatus;

            if (status == Models.Enum.ContractStatus.ACTIVE)
            {
                var currentActive = await _context.Contracts
                    .FirstOrDefaultAsync(c => c.EmployeeId == contract.EmployeeId &&
                        c.ContractStatus == Models.Enum.ContractStatus.ACTIVE);

                if (currentActive != null && currentActive.Id != contract.Id)
                {
                    currentActive.ContractStatus = Models.Enum.ContractStatus.EXPIRED;
                }
            }

            contract.ContractStatus = status;

            await _contractRepository.SaveAsync();

            return _contractMapping.ToContractRes(contract);
        }
    }
}