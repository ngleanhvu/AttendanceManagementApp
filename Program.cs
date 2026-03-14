using AttendanceManagementApp.Configs;
using AttendanceManagementApp.Mappings;
using AttendanceManagementApp.Middlewares;
using AttendanceManagementApp.Repositories;
using AttendanceManagementApp.Services.Impl;
using AttendanceManagementApp.Services.Interface;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Add controller
builder.Services.AddControllers();
// Add mapping
builder.Services.AddScoped<DepartmentMapping>();
builder.Services.AddScoped<PositionMapping>();
builder.Services.AddScoped<EmployeeMapping>();
// Add repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IPositionService, PositionService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
// Add services

var app = builder.Build();
// Add global exception handling middleware
app.UseMiddleware<ExceptionMiddleware>();


app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
