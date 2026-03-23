using AttendanceManagementApp.Configs;
using AttendanceManagementApp.DTOs.Request;
using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Services.Impl;
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

        public AuthController(AppDbContext context, IJwtService jwtService,
            IPasswordService passwordService)
        {
            _context = context;
            _jwtService = jwtService;
            _passwordService = passwordService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var account = await _context.Accounts
                .Include(x => x.Position)
                .Include(x => x.Employee)
                .FirstOrDefaultAsync(x => x.Username == request.Username && x.IsActive);

            if (account == null)
                return Unauthorized("Invalid username");

            // So sánh password (bạn nên dùng BCrypt)
            if (_passwordService.Verify(account.PasswordHash, request.Password))
                return Unauthorized("Invalid password");

            var token = _jwtService.GenerateToken(account);

            return Ok(new ApiResponse<LoginRes>(new LoginRes
            {
                AccountId = account.Id,
                EmployeeId = account.EmployeeId,
                Role = account.Position.Name,
                Token = token
            }));
        }
    }
}
