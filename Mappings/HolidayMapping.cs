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
                Date = holiday.Date != null ? holiday.Date : null,
                Description = holiday.Description,
                Month = holiday.Month,
                IsPaidHoliday = holiday.IsPaidHoliday,
                AllowWork = holiday.AllowWork,
                SalaryCoefficient = holiday.SalaryCoefficient
            };
        }
    }
}
