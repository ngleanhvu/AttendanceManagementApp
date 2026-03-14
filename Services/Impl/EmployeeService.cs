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
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _context;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IRepository<Employee> _repo;
        private readonly IRepository<EmployeeDetail> _detailRepository;
        private readonly IDepartmentService _departmentService;
        private readonly IPositionService _positionService;
        private readonly EmployeeMapping _employeeMapping;

        public EmployeeService(AppDbContext context, ICloudinaryService cloudinaryService,
            IRepository<Employee> repo, IRepository<EmployeeDetail> detailRepository,
            IDepartmentService departmentService, IPositionService positionService,
            EmployeeMapping employeeMapping)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
            _repo = repo;
            _detailRepository = detailRepository;
            _departmentService = departmentService;
            _positionService = positionService;
            _employeeMapping = employeeMapping;
        }

        public async Task<EmployeeRes> CreateEmployeeAsync(EmployeeCreateReq req)
        {
            // 1. Validate input
            var department = await _departmentService.GetDepartmentAsync(req.DepartmentId);
            var position = await _positionService.GetPositionAsync(req.PositionId);
            if (req.Thumbnail == null || req.Thumbnail.Length == 0)
                throw new BadHttpRequestException("Thumbnail is required");
            // 2. Upload image (ngoài transaction)
            var thumbnailUrl = await _cloudinaryService.UploadImageAsync(req.Thumbnail);
            // 3. Create entities
            var employee = new Employee
            {
                Code = $"EM{req.IdentityNumber}",
                Email = req.Email,
                Fullname = req.Fullname,
                Gender = req.Gender,
                UserStatus = (UserStatus)req.UserStatus,
                Thumbnail = thumbnailUrl
            };

            var employeeDetail = new EmployeeDetail
            {
                DateOfBirth = req.DateOfBirth,
                IdentityNumber = req.IdentityNumber,
                Address = req.Address,
                Phone = req.Phone,
                HireDate = req.HiredDate,
                Department = department,
                Position = position,
                Employee = employee
            };
            // 4. Transaction
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _repo.AddAsync(employee);
                await _detailRepository.AddAsync(employeeDetail);

                await _repo.SaveAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
            // 5. Return response
            return _employeeMapping.ToEmployeeRes(employee);
        }

        public async Task<PagedResult<EmployeeRes>> GetEmployeesAsync(PaginationQuery query)
        {
            var queryable = _context.Employees
                .AsNoTracking()
                .ApplySearch(query.Search,
                    x => x.Fullname,
                    x => x.Email,
                    x => x.Code)
                .ApplySorting(query.SortBy, query.Desc);

            var total = await queryable.CountAsync();

            var items = await queryable
                .ApplyPagination(query.Page, query.PageSize)
                .ToListAsync();

            var itemsRes = items.Select(x => _employeeMapping.ToEmployeeRes(x)).ToList();
            
            return new PagedResult<EmployeeRes>
            {
                Total = total,
                Items = itemsRes,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        public async Task<EmployeeDetailRes> GetEmployeeAsync(int id)
        {
            var employee = await _context.Employees
                .Include(x => x.EmployeeDetail)
                    .ThenInclude(x => x.Department)
                .Include(x => x.EmployeeDetail)
                    .ThenInclude(x => x.Position)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
            if (employee == null)
                throw new NotFoundException("Employee not found");
            return _employeeMapping.ToEmployeeDetailRes(employee);
        }

        public async Task<EmployeeRes> SoftDeleteEmployeeAsync(int id)
        {
            var employee = await _context.Employees.Include(x => x.EmployeeDetail).FirstOrDefaultAsync(x => x.Id == id);
            var employeeDetail = employee?.EmployeeDetail;
            employee.Status = false;
            employeeDetail.Status = false;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _repo.Update(employee);
                _detailRepository.Update(employeeDetail);
                await _repo.SaveAsync();
                return _employeeMapping.ToEmployeeRes(employee);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<EmployeeDetailRes> UpdateEmployeeAsync(int id, EmployeeUpdateReq req)
        {
            var employee = await _context.Employees
                .Include(x => x.EmployeeDetail)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (employee == null)
                throw new NotFoundException("Employee not found");
            // 1. Validate input
            var department = await _departmentService.GetDepartmentAsync(req.DepartmentId);
            var position = await _positionService.GetPositionAsync(req.PositionId);
            // 2. Upload image (ngoài transaction)
            var thumbnailUrl = employee.Thumbnail;
            if (req.Thumbnail != null && req.Thumbnail.Length != 0)
            {
                thumbnailUrl = await _cloudinaryService.UploadImageAsync(req.Thumbnail);
            } 
            // 3. Update entities
            employee.Fullname = req.Fullname;
            employee.Gender = req.Gender;
            employee.Email = req.Email;
            employee.Thumbnail = thumbnailUrl;
            employee.UserStatus = (UserStatus)req.UserStatus;
            employee.EmployeeDetail.DateOfBirth = req.DateOfBirth;
            employee.EmployeeDetail.IdentityNumber = req.IdentityNumber;
            employee.EmployeeDetail.Address = req.Address;
            employee.EmployeeDetail.Phone = req.Phone;
            employee.EmployeeDetail.HireDate = req.HiredDate;
            employee.EmployeeDetail.Department = department;
            employee.EmployeeDetail.Position = position;
            // 4. Transaction
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _repo.Update(employee);
                _detailRepository.Update(employee.EmployeeDetail);

                await _repo.SaveAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
            // 5. Return response
            return _employeeMapping.ToEmployeeDetailRes(employee);
        }
    }
}
