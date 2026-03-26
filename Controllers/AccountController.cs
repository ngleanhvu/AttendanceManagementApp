using AttendanceManagementApp.Configs;
using AttendanceManagementApp.DTOs.Request;
using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Exceptions;
using AttendanceManagementApp.Mappings;
using AttendanceManagementApp.Services.Interface;
using AttendanceManagementApp.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AttendanceManagementApp.Controllers
{
    [ApiController]
    [Route("api/v1/accounts")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly IPasswordService _passwordService;
        private readonly EmployeeMapping _employeeMapping;

        public AuthController(AppDbContext context, IJwtService jwtService,
            IPasswordService passwordService, EmployeeMapping employeeMapping)
        {
            _context = context;
            _jwtService = jwtService;
            _passwordService = passwordService;
            _employeeMapping = employeeMapping;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var account = await _context.Accounts
            .Include(x => x.Employee)
            .Include(x => x.Position)
            .FirstOrDefaultAsync(x =>
                x.Username.Trim().ToLower() == request.Username.Trim().ToLower() && x.IsActive == true
            );

            if (account == null)
                throw new AppException("Account not found", "NOT_FOUND", 404);

            // So sánh password (bạn nên dùng BCrypt)
            if (_passwordService.Verify(account.PasswordHash, request.Password) != true)
                throw new AppException("Invalid password", "INVALID_PASSWORD", 401);

            var token = _jwtService.GenerateToken(account);

            return Ok(new ApiResponse<LoginRes>(new LoginRes
            {
                AccountId = account.Id,
                Info = _employeeMapping.ToEmployeeRes(account.Employee),
                Role = account.Position.Name,
                Token = token
            }));
        }
    }
}
