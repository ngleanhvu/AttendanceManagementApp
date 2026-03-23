using Microsoft.EntityFrameworkCore; 
using AttendanceManagementApp.Models;

namespace AttendanceManagementApp.Configs
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Holiday> Holidays { get; set; }
        public DbSet<LeaveType> LeaveTypes { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Payroll> Payrolls { get; set; }
        public DbSet<OverTime> OverTimes { get; set; }
        public DbSet<PayrollDetail> PayrollDetails { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<Account> Accounts  { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.EmployeeDetail)
                .WithOne(d => d.Employee)
                .HasForeignKey<EmployeeDetail>(d => d.EmployeeId);
        }

    }
}
