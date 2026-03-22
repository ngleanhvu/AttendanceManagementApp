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
    public class HolidayService : IHolidayService
    {
        private readonly IRepository<Holiday> _holidayRepository;
        private readonly HolidayMapping _holidayMapping;
        private readonly AppDbContext _appDbContext;

        public HolidayService(IRepository<Holiday> holidayRepository, 
            HolidayMapping holidayMapping, AppDbContext appDbContext)
        {
            _holidayRepository = holidayRepository;
            _holidayMapping = holidayMapping;
            _appDbContext = appDbContext;
        }

        public async Task<HolidayRes> CreateHolidayAsync(HolidayCreateReq req)
        {
            var holiday = new Holiday()
            {
                Name = req.Name,
                TotalDay = req.TotalDay,
                Description = req.Description,
                Month = req.Month,
                Year = req.Year,
                FromDate = req.FromDate,
                ToDate = req.ToDate,
            };
            await _holidayRepository.AddAsync(holiday);
            await _holidayRepository.SaveAsync();

            return _holidayMapping.ToHolidayRes(holiday);
        }

        public async Task<HolidayRes> GetHolidayAsync(int id)
        {
            var hoilday = await _holidayRepository.GetByIdAsync(id);
            if (hoilday == null)
            {
                throw new NotFoundException("Holiday not found");
            }
            return _holidayMapping.ToHolidayRes(hoilday);
        }

        public async Task<PagedResult<HolidayRes>> GetHolidaysAsync(PaginationQuery query)
        {
            var pageable = _appDbContext.Holidays
                .AsQueryable()
                .Where(x => x.Status == true)
                .ApplySearch(query.Search, x => x.Name, x => x.Description)
                .ApplySorting(query.SortBy, query.Desc);
            var count = await pageable.CountAsync();
            var items = await pageable
                .ApplyPagination(query.Page, query.PageSize)
                .ToListAsync();
            var holidayRes = items.Select(x => _holidayMapping.ToHolidayRes(x)).ToList();
            return new PagedResult<HolidayRes>()
            {
                Total = count,
                Page = query.Page,
                PageSize = query.PageSize,
                Items = holidayRes
            };
        }

        public async Task<HolidayRes> SoftDeleteHolidayAsync(int id)
        {
            var holiday = await _holidayRepository.GetByIdAsync(id);
            if (holiday == null)
            {
                throw new NotFoundException("Holiday not found");
            }
            _holidayRepository.SoftDelete(holiday);
            await _holidayRepository.SaveAsync();
            return _holidayMapping.ToHolidayRes(holiday);
        }

        public async Task<int> TotalHolidayAsync(int month, int year)
        {
            var totalDay = await _appDbContext.Holidays
                .Where(x => x.Month == month)
                .Where(x => x.Year == year)
                .AsNoTracking()
                .ToListAsync();
            int total = 0;
            foreach (var item  in totalDay)
            {
                total += item.TotalDay;
            }
            return total;
        }

        public async Task<HolidayRes> UpdateHolidayAsync(int id, HolidayCreateReq req)
        {
            var holiday = await _holidayRepository.GetByIdAsync(id);
            if (holiday == null)
            {
                throw new NotFoundException("Holiday not found");
            }
            holiday.Name = req.Name;
            holiday.TotalDay = req.TotalDay;
            holiday.Description = req.Description;
            holiday.Month = req.Month;
            holiday.Year = req.Year;
            holiday.FromDate = req.FromDate;
            holiday.ToDate = req.ToDate;
            _holidayRepository.Update(holiday);
            await _holidayRepository.SaveAsync();
            return _holidayMapping.ToHolidayRes(holiday);
        }
    }
}
