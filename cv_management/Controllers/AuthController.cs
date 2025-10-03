using cv_management.Models;
using CvManagementApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CvManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly CvManagementContext _context;

        public AuthController(CvManagementContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == req.Username && u.PasswordHash == req.Password);

            if (user == null)
            {
                return Unauthorized(new { message = "Sai username hoặc password" });
            }



            return Ok(new
            {
                message = "Đăng nhập thành công",
                user = new
                {
                    user.UserId,
                    user.Username,
                    user.FullName,
                    user.Email
                }
            });
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
