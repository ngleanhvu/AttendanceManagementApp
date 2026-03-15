using AttendanceManagementApp.Configs;
using AttendanceManagementApp.Mappings;
using AttendanceManagementApp.Middlewares;
using AttendanceManagementApp.Repositories;
using AttendanceManagementApp.Services.Impl;
using AttendanceManagementApp.Services.Interface;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// OpenAPI
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
builder.Services.AddScoped<ShiftMapping>();
builder.Services.AddScoped<EmployeeShiftMapping>();

// Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Services
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IPositionService, PositionService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IContractService, ContractService>();
builder.Services.AddScoped<IShiftService, ShiftService>();
builder.Services.AddScoped<IEmployeeShiftService, EmployeeShiftService>();

// Cloudinary config
builder.Services.Configure<CloudinaryConfig>(
    builder.Configuration.GetSection("Cloudinary")
);

builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();

var app = builder.Build();

// Global Exception Middleware
app.UseMiddleware<ExceptionMiddleware>();

// OpenAPI
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();