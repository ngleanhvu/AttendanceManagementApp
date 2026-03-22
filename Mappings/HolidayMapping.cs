using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Models;

namespace AttendanceManagementApp.Mappings
{
    public class HolidayMapping
    {
        public HolidayRes ToHolidayRes(Holiday holiday)
        {
            return new HolidayRes
            {
                Id = holiday.Id,
                Name = holiday.Name,
                TotalDay = holiday.TotalDay,
                Description = holiday.Description,
                Month = holiday.Month,
                Year = holiday.Year,
                FromDate = holiday.FromDate,
                ToDate = holiday.ToDate,
            };
        }
    }
}
