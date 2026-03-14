using AttendanceManagementApp.DTOs.Request;
using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Mappings;
using AttendanceManagementApp.Models;
using AttendanceManagementApp.Repositories;
using AttendanceManagementApp.Services.Interface;
using AttendanceManagementApp.Utils;
using AttendanceManagementApp.Exception;
using AttendanceManagementApp.Configs;
using Microsoft.EntityFrameworkCore;

namespace AttendanceManagementApp.Services.Impl
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IRepository<Department> _repo;
        private readonly DepartmentMapping _departmentMapping;
        private readonly AppDbContext _context;

        public DepartmentService(IRepository<Department> repo,
            DepartmentMapping departmentMapping,
            AppDbContext context)
        {
            _repo = repo;
            _departmentMapping = departmentMapping;
            _context = context;
        }

        public async Task<DepartmentRes> CreateDepartmentAsync(DepartmentCreateReq req)
        {
            var department = new Department
            {
                Name = req.Name,
                Description = req.Description
            };

            await _repo.AddAsync(department);
            await _repo.SaveAsync();

            return _departmentMapping.ToDepartmentRes(department);
        }

        public async Task<Department> GetDepartmentAsync(int id)
        {
            var department = _repo.GetByIdAsync(id).Result;
            if (department == null)
            {
                throw new NotFoundException("Department with not found.");
            }
            return department;
        }

        public async Task<PagedResult<DepartmentRes>> GetDepartmentsAsync(PaginationQuery query)
        {
            var queryable = _context.Departments
                .AsNoTracking()
                .ApplySearch(query.Search, x => x.Name)
                .ApplySorting(query.SortBy, query.Desc);

            var total = await queryable.CountAsync();

            var items = await queryable
                .ApplyPagination(query.Page, query.PageSize)
                .ToListAsync();


            var itemsRes = items.Select(x => _departmentMapping.ToDepartmentRes(x)).ToList();

            return new PagedResult<DepartmentRes>             {
                Total = total,
                Page = query.Page,
                PageSize = query.PageSize,
                Items = itemsRes
            };

        }

        public async Task<DepartmentRes> SoftDeleteDepartmentAsync(int id)
        {
            var department = _repo.GetByIdAsync(id).Result;
            if (department == null)
            {
                throw new NotFoundException("Department with not found.");
            }
            _repo.SoftDelete(department);
            await _repo.SaveAsync();

            return _departmentMapping.ToDepartmentRes(department);
        }

        public async Task<DepartmentRes> UpdateDepartmentAsync(int id, DepartmentCreateReq req)
        {
            var department = _repo.GetByIdAsync(id).Result;
            if (department == null)
            {
                throw new NotFoundException("Department with not found.");
            }
            department.Name = req.Name;
            department.Description = req.Description;
            _repo.Update(department);
            await _repo.SaveAsync();
            return _departmentMapping.ToDepartmentRes(department);
        }
    }
}
