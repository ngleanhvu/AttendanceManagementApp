using AttendanceManagementApp.Configs;
using AttendanceManagementApp.Models.Enum;
using AttendanceManagementApp.Repositories;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace AttendanceManagementApp.Cronjob
{
    public class ContractChangeExpiredStatusJob : IJob
    {
        private readonly AppDbContext _context;
        public ContractChangeExpiredStatusJob(AppDbContext context)
        {
            _context = context;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var now = DateOnly.FromDateTime(DateTime.Now);

            var contracts = await _context.Contracts
                .Where(x => x.EndDate <= now && x.ContractStatus != ContractStatus.EXPIRED)
                .ToListAsync();

            foreach (var contract in contracts)
            {
                contract.ContractStatus = ContractStatus.EXPIRED;
            }

            await _context.SaveChangesAsync();
        }
    }
}
