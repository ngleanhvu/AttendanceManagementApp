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
    public class EmployeeShiftService : IEmployeeShiftService
    {
        private readonly IEmployeeService _employeeService;
        private readonly IShiftService _shiftService;
        private readonly IRepository<EmployeeShift> _employeeShiftRepo;
        private readonly AppDbContext _appDbContext;
        private readonly EmployeeShiftMapping _employeeShiftMapping;
        
        public EmployeeShiftService(IEmployeeService employeeService, IShiftService shiftService, 
            IRepository<EmployeeShift> employeeShiftRepo, 
            AppDbContext appDbContext, EmployeeShiftMapping employeeShiftMapping)
        {
            _employeeService = employeeService;
            _shiftService = shiftService;
            _employeeShiftRepo = employeeShiftRepo;
            _appDbContext = appDbContext;
            _employeeShiftMapping = employeeShiftMapping;
        }
        public async Task CreateEmployeeShiftAsync(EmployeeShiftCreateReq req)
        {
            var employees = await _appDbContext.Employees
                .Where(x => req.EmployeeIds.Contains(x.Id))
                .ToListAsync();

            var shift = await _shiftService.GetShiftByIdAsync(req.Shift);

            List<EmployeeShift> employeeShifts = new();

            foreach (var employee in employees)
            {
                employeeShifts.Add(new EmployeeShift
                {
                    Employee = employee,
                    Shift = shift,
                    FromDate = req.FromDate,
                    ToDate = req.ToDate,
                    IsActive = req.IsActive,
                    Note = req.Note,
                    AssignedAt = req.AssignedAt,
                    AssignedBy = req.AssignedBy
                });
            }

            await _appDbContext.EmployeeShifts.AddRangeAsync(employeeShifts);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<EmployeeShiftRes> GetEmployeeShiftAsync(int id)
        {
            var employeeShift = _employeeShiftRepo.GetByIdAsync(id).Result;
            if (employeeShift == null) 
                throw new NotFoundException("Employee shift not found");
            return _employeeShiftMapping.ToEmployeeShiftRes(employeeShift);
        }

        public async Task<List<EmployeeShiftRes>> GetEmployeeShiftsAsync(PaginationQuery query)
        {
            var pageble = _appDbContext.EmployeeShifts
                .AsNoTracking()
                .Where(x => x.FromDate <= DateOnly.FromDateTime(DateTime.Now) 
                        && x.ToDate >= DateOnly.FromDateTime(DateTime.Now)
                        || x.Employee.Id.ToString().Equals(query.Search)
                )
                .ApplyPagination(query.Page, query.PageSize);

            var count = await pageble.CountAsync();

            var items = await pageble.Select(x => _employeeShiftMapping.ToEmployeeShiftRes(x)).ToListAsync();

            return items;
        }

        public async Task<EmployeeShiftRes> SoftDeleteEmployeeShiftAsync(int id)
        {
            var employeeShift = _employeeShiftRepo.GetByIdAsync(id).Result;
            if (employeeShift == null) 
                throw new NotFoundException("Employee shift not found");
            _employeeShiftRepo.SoftDelete(employeeShift);
            await _employeeShiftRepo.SaveAsync();
            return _employeeShiftMapping.ToEmployeeShiftRes(employeeShift);
        }

        public async Task<EmployeeShiftRes> UpdateEmploymentsAsync(int id, EmployeeShiftUpdateReq req)
        {
            var shift = _shiftService.GetShiftByIdAsync(req.ShiftId).Result;
            var employeeShift = _employeeShiftRepo.GetByIdAsync(id).Result;
            if (employeeShift == null) 
                throw new NotFoundException("Employee shift not found");
            employeeShift.FromDate = req.FromDate;
            employeeShift.ToDate = req.ToDate;
            employeeShift.IsActive = req.IsActive;
            employeeShift.Note = req.Note;
            employeeShift.AssignedBy = req.AssignedBy;
            employeeShift.AssignedAt = req.AssignedAt;
            employeeShift.Shift = shift;
            _employeeShiftRepo.Update(employeeShift);
            await _employeeShiftRepo.SaveAsync();
            return _employeeShiftMapping.ToEmployeeShiftRes(employeeShift);
        }
    }
}
