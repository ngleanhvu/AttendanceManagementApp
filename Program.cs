using AttendanceManagementApp.Configs;
using AttendanceManagementApp.Cronjob;
using AttendanceManagementApp.Mappings;
using AttendanceManagementApp.Middlewares;
using AttendanceManagementApp.Repositories;
using AttendanceManagementApp.Services.Impl;
using AttendanceManagementApp.Services.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ======================
// Add services
// ======================

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Mapping
builder.Services.AddScoped<DepartmentMapping>();
builder.Services.AddScoped<PositionMapping>();
builder.Services.AddScoped<EmployeeMapping>();
builder.Services.AddScoped<ContractMapping>();
builder.Services.AddScoped<HolidayMapping>();
builder.Services.AddScoped<AttendanceMapping>();
builder.Services.AddScoped<LeaveTypeMapping>();
builder.Services.AddScoped<LeaveRequestMapping>();
builder.Services.AddScoped<OvertimeMapping>();
builder.Services.AddScoped<PayrollMapping>();

// Repository
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Services
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IPositionService, PositionService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IContractService, ContractService>();
builder.Services.AddScoped<IHolidayService, HolidayService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<ILeaveTypeService, LeaveTypeService>();
builder.Services.AddScoped<ILeaveRequestService, LeaveRequestService>();
builder.Services.AddScoped<IPayrollService, PayrollService>();
builder.Services.AddScoped<IOvertimeService, OvertimeService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IEmployeeRecognitionService, EmployeeRecognitionService> ();

// Cloudinary
builder.Services.Configure<CloudinaryConfig>(
    builder.Configuration.GetSection("Cloudinary")
);
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();

// ======================
// JWT CONFIG (🔥 PHẢI Ở ĐÂY)
// ======================
var jwtSettings = builder.Configuration.GetSection("Jwt");
var keyString = jwtSettings["Key"] ?? throw new Exception("JWT Key missing");
var key = Encoding.UTF8.GetBytes(keyString);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();

// ======================
// Quartz Job
// ======================
builder.Services.AddQuartz(q =>
{
    var contactExpiredStatusChangeKey = new JobKey("ContactExpiredStatusKey");
    var payrollCalKey = new JobKey("PayrollCalKey");

    q.AddJob<ContractChangeExpiredStatusJob>(opts => opts.WithIdentity(contactExpiredStatusChangeKey));
    q.AddJob<PayrollJob>(opts => opts.WithIdentity(payrollCalKey));

    q.AddTrigger(opts => opts
        .ForJob(contactExpiredStatusChangeKey)
        .WithIdentity("ContactExpiredStatus-trigger")
        .WithCronSchedule("0 0 8 * * ?")
    );

    q.AddTrigger(opts => opts
        .ForJob(payrollCalKey)
        .WithIdentity("PayrollJob-trigger")
        .WithCronSchedule("0 0 2 1 * ?")
    );
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

// ======================
// Build app
// ======================
var app = builder.Build();

// ======================
// Middleware pipeline
// ======================

// Exception
app.UseMiddleware<ExceptionMiddleware>();

// OpenAPI
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseCors("AllowFrontend");
// 🔥 QUAN TRỌNG
app.UseAuthentication();  // xác thực JWT
app.UseAuthorization();   // phân quyền

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();