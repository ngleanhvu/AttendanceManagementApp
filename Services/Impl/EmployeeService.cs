using AttendanceManagementApp.Configs;
using AttendanceManagementApp.DTOs.Request;
using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Mappings;
using AttendanceManagementApp.Models;
using AttendanceManagementApp.Models.Enum;
using AttendanceManagementApp.Repositories;
using AttendanceManagementApp.Services.Interface;

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
    }
}
