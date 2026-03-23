using AttendanceManagementApp.Configs;
using AttendanceManagementApp.Models;
using AttendanceManagementApp.Services.Interface;
using Quartz;

namespace AttendanceManagementApp.Cronjob
{
    public class PayrollJob : IJob
    {
        private readonly AppDbContext _context;
        private readonly IPayrollService _payrollService;

        public PayrollJob(AppDbContext context, IPayrollService payrollService)
        {
            _context = context;
            _payrollService = payrollService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var now = DateTime.Now;

            int month = now.AddMonths(-1).Month; 
            int year = now.AddMonths(-1).Year;

            await _payrollService.CalculatePayrollAsync(new DTOs.Request.PayrollCalculateReq
            {
                Month = month,
                Year = year,
            });
        }
    }
}
